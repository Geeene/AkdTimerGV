namespace AkdTimerGV.Components.Models {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public class User(String name) {
        // Globaly unique ID for this User
        // The Guids MUST have setter, otherwise the de-serialization from the Local storage would give a new Guid from construction :)
        public Guid UserId { get; set; } = Guid.NewGuid();

        // Name that would actually be displayed
        public string Name { get; set; } = name;

        // Team this user belongs to
        public Team? Team { get; set; }

        public override bool Equals(object? obj) {
            return obj is User user &&
                   UserId.Equals(user.UserId);
        }

        public override int GetHashCode() {
            return HashCode.Combine(UserId);
        }
    }
}
