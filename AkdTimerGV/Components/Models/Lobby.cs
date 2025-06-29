namespace AkdTimerGV.Components.Models {
    /// <summary>
    /// A Lobby which has one or more people racing in it.
    /// 
    /// Can (must doesn't have to be) password protected.
    /// </summary>
    public class Lobby(String Description, User Owner, String Password) {
        
        /// <summary>
        /// The Guids MUST have setter, otherwise the de-serialization from the Local storage would give a new Guid from construction :)
        /// </summary>
        public Guid LobbyId { get; set; } = System.Guid.NewGuid();
        
        /// <summary>
        /// Simply the user friendly name for this lobby
        /// </summary>
        public string? Description { get; set; } = Description;
        
        /// <summary>
        /// The User who created this lobby
        /// </summary>
        public User Owner { get; set; } = Owner;
        
        /// <summary>
        /// The password a user has to enter when joining the Lobby, stored as plain text as it isn't exactly that relevant to be secure here
        /// </summary>
        public string? Password { get; } = Password;

        /// <summary>
        /// The Time Stamp when the lobby was created.
        /// </summary>
        public DateTime Created = DateTime.Now;
        /// <summary>
        /// Wether it is possible to join another team, by default is true
        /// </summary>
        public bool TeamMode { get; set; } = true;
        public readonly Dictionary<Guid, Team> Teams = [];
        private readonly Dictionary<User, Team> UserTeamMapping = [];

        /// <summary>
        /// Current value of the Starting Pistol countdown. If value <= 0, then it will not be shown.
        /// </summary>
        public double StartingPistolValue = 0;

        /// <summary>
        /// Used to determine wether the global pause button should be enabled or not
        /// </summary>
        public bool Started { get; set; } = false;

        /// <summary>
        /// Wether this race is resumed or not. This determines if the teams will have the ability to add previous time
        /// </summary>
        public bool ResumedRace { get; set; } = false;

        /// <summary>
        /// Switches the user to the Team with the given name, if there is no such Team yet, then it will be created.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newTeamName"></param>
        public void SwitchTeam(User user, String newTeamName) {

            // Then check if the Team they want to joins exists already
            Team? newTeam = GetTeamByName(newTeamName);

            // If the team already exists, and is locked, then do nothing
            if (newTeam != null && newTeam.Locked) {
                return;
            }

            // First up, remove the user from their current Team
            RemoveUserFromTeam(user);

            user.HasTeamPermission = false;
            // If there is no team yet with the chosen name, then create a new Team
            if (newTeam == null) {
                newTeam = new Team(newTeamName);
                if (Team.SPECTATOR.Equals(newTeamName)) {
                    newTeam.Participating = false;
                } else { 
                    newTeam.Creator = user;
                    user.HasTeamPermission = true;
                }
                Teams.Add(newTeam.TeamId, newTeam);
            }

            newTeam.AddMember(user);
            UserTeamMapping.Add(user, newTeam);
        }

        public Team? GetTeamByName(String TeamName) {
            return Teams.Values.Where(team => team.Name.Equals(TeamName)).FirstOrDefault();
        }

        /// <summary>
        /// If the password matches the password of the lobby, then join the lobby as a spectator
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool TryJoinLobby(User user, String? password) {
            // if the lobby has a password, and that password doesn't match what the user entered, then refuse
            if (this.Password != null && !this.Password.Equals(password)) {
                return false;
            }

            // Join was successful, add the user as a spectator by default
            SwitchTeam(user, Team.SPECTATOR);
            return true;
        }

        /// <summary>
        /// Returns all teams that are participating, i.e. not SPECTATOR
        /// </summary>
        /// <returns></returns>
        public List<Team> GetParticipatingTeams() {
            return Teams.Values.Where(testc => testc.Participating).ToList();
        }

        /// <summary>
        /// Returns a list of all teams
        /// </summary>
        /// <returns></returns>
        public List<Team> GetAllTeams() {
            return Teams.Values.ToList();
        }

        /// <summary>
        /// Have the given user leave the lobby by removing them from all teams
        /// </summary>
        /// <param name="user"></param>
        public void LeaveLobby(User user) {
            RemoveUserFromTeam(user);
            if (UserTeamMapping.Count == 0) {
                LobbyHolder.removeLobby(this);
            }
        }

        /// <summary>
        /// Remove the given user from their current team.
        /// If that team were to become empty and is not the spectating team, then the Team will be removed
        /// </summary>
        /// <param name="user"></param>
        private void RemoveUserFromTeam(User user) {
            Team? teamToRemoveFrom = user.Team;
            if (teamToRemoveFrom != null) {
                bool isSpectator = !teamToRemoveFrom.Participating;
                teamToRemoveFrom.RemoveMember(user);
                if (teamToRemoveFrom.members.Count == 0 && !isSpectator) {
                    this.Teams.Remove(teamToRemoveFrom.TeamId);
                }
            }
            UserTeamMapping.Remove(user);
        }

        /// <summary>
        /// Returns true if the given User is present in the UserTeamMapping
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsUserInLobby(User user) { 
            return this.UserTeamMapping.ContainsKey(user);
        }

        public Team GetSpectatorTeam() {
            return GetTeamByName(Team.SPECTATOR);
        }

        public Team GetTeamByTeamId(Guid teamId) {
            return Teams[teamId];
        }

        /// <summary>
        /// Starts all timers, used by the Starting Pistol
        /// </summary>
        public void StartAllTimers() {
            foreach (var Team in GetParticipatingTeams()) {
                Team.TimerData.Start();
            }
        }

        /// <summary>
        /// Pause all timers, can be used by the Host
        /// </summary>
        public void PauseAll() {
            Started = false;
            foreach (var Team in GetParticipatingTeams()) {
                Team.TimerData.Finish();
            }
        }

        /// <summary>
        /// Reset all timers to their initial values
        /// </summary>
        public void GlobalReset() {
            this.Started = false;
            foreach (var Team in GetParticipatingTeams()) {
                Team.TimerData.Reset();
            }
        }

        /// <summary>
        /// Sort all the tems by their final time (Active + Penalty, no break). Then their position into the Standings variable of the timer.
        /// </summary>
        /// <returns></returns>
        public bool DetermineStandings() {
            List<Team> FinishedTeams = GetParticipatingTeams()
                // Filter for teams that aren't active and have spent time playing
                .Where(team => !team.TimerData.Active && team.TimerData.GetElapsedActiveTime() > 0)
                // Sort those teams by the amount of time they would submit
                .OrderBy(team => team.TimerData.GetSubmittedTime())
                .ToList();
            
            for (int i = 0; i< FinishedTeams.Count; i++) {
                int standing = i + 1;
                if (i > 0 && FinishedTeams[i].TimerData.GetSubmittedTime() == FinishedTeams[i - 1].TimerData.GetSubmittedTime()) {
                    standing = (int) FinishedTeams[i - 1].TimerData.Standing;
                }

                Teams[FinishedTeams[i].TeamId].TimerData.Standing = standing;
            }
            return true;
        }

        /// <summary>
        /// Return a list of all users except the host. 
        /// </summary>
        /// <returns></returns>
        public List<User> GetUsersExceptHost() {
            return UserTeamMapping.Keys.Where(v => !v.Equals(Owner)).ToList();
        }

        /// <summary>
        /// Returns true if a user in this lobby has the given name; false otherwise
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public User? GetUserWithName(String name) { 
            return UserTeamMapping.Keys.Where(v => v.Name.Equals(name)).FirstOrDefault();
        }
    }
}
