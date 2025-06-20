using AkdTimerGV.Components.Models;

// Singleton to hold all the open Lobbies
public sealed class UserHolder {
    private static UserHolder instance = null;
    private static readonly object padlock = new object();
    private static readonly Dictionary<Guid, User> Users = [];
    UserHolder() {
    }

    // Make it threadsafe to create the Singleton instance by locking a shared object
    public static UserHolder Instance {
        get {
            lock (padlock) {
                if (instance == null) {
                    instance = new UserHolder();
                }
                return instance;
            }
        }
    }

    /// <summary>
    /// Create a new User and save them in the User Dictionary
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static User createUser(String userId) {
        User user = new(userId);
        Users[user.UserId] = user;
        return user;
    }

    /// <summary>
    /// Get the user with the given Id from the Dictionary
    /// </summary>
    public static User? GetUserById(Guid userId) {
        if (!Users.ContainsKey(userId)) {
            return null;
        }

        return Users[userId];
    }
}