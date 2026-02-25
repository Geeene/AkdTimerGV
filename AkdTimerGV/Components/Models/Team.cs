namespace AkdTimerGV.Components.Models {
    /// <summary>
    /// Represents a Team participating in the race, or the spectator team
    /// </summary>
    /// <param name="name"></param>
    public class Team(String name, int index) {
        
        /// <summary>
        /// Constant for the name of the spectator team
        /// </summary>
        public static readonly string SPECTATOR = "Spectator";

        /// <summary>
        /// Name of the team that this timer belongs to
        /// </summary>
        public String Name { get; set; } = name;

        /// <summary>
        /// The Id of the team
        /// </summary>
        public Guid TeamId = Guid.NewGuid();

        /// <summary>
        /// The User that created the team, nullable as unnecessary for the Spectator team.
        /// May end up receiving some extra permission over the team members and such
        /// </summary>
        public User? Creator { get; set; }

        /// <summary>
        /// If this value is true, then users can not join this team
        /// </summary>
        public bool Locked { get; set; } = false;

        /// <summary>
        /// The Users that can maintain the timer
        /// </summary>
        public readonly List<User> members = [];

        /// <summary>
        /// wether this team actually has a timer, only false for the SPECTATOR Team
        /// </summary>
        public bool Participating { get; set; } = true;

        /// <summary>
        /// The Teams timer
        /// </summary>
        public TeamTimerData TimerData { get; } = new TeamTimerData();

        /// <summary>
        /// The order of teams joining the lobby
        /// </summary>
        public int Index { get; set; } = index;

        /// <summary>
        /// Race Category
        /// </summary>
        public RaceCategory RaceCategory { get; set; }
        
        /// <summary>
        /// Add the given user to the team
        /// </summary>
        /// <param name="user"></param>

        public void AddMember(User user) {
            if (!members.Contains(user)) {
                members.Add(user);
            }
            user.Team = this;
        }

        /// <summary>
        /// Removes the given user from the team. If that user was the creator, 
        /// and there are still team members, then make the next user the team creator
        /// </summary>
        /// <param name="user"></param>
        public void RemoveMember(User user) {
            members.Remove(user);
            user.Team = null;

            if (Creator != null && Creator.Equals(user) && members.Count > 0) {
                // Pass down the permissions to the next user, prefer users that already have been given permissions 
                List<User> MembersWithPermission = members.Where(member => member.HasTeamPermission).ToList();

                Creator = MembersWithPermission.Count > 0 ? MembersWithPermission[0] : members[0]; 
            }
        }
    }
}
