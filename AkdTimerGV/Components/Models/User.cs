namespace AkdTimerGV.Components.Models {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public class User(String name) {
        /// <summary>
        /// Globaly unique ID for this User
        /// The Guids MUST have setter, otherwise the de-serialization from the Local storage would give a new Guid from construction :)
        /// </summary>
        public Guid UserId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Name that would actually be displayed
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// Team this user belongs to
        /// </summary>
        public Team? Team { get; set; }

        /// <summary>
        /// True if the user is able to control the timer of their team
        /// </summary>
        public bool HasTeamPermission { get; set; }

        /// <summary>
        /// Returns true if the User is in a Team and is the creator of that team
        /// </summary>
        /// <returns></returns>
        public bool IsCreatorOfTeam() { 
            return Team !=null && this.Equals(Team.Creator);
        }

        public override bool Equals(object? obj) {
            return obj is User user &&
                   UserId.Equals(user.UserId);
        }

        public override int GetHashCode() {
            return HashCode.Combine(UserId);
        }
    }
}
