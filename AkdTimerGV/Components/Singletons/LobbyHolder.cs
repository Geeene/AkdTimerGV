using AkdTimerGV.Components.Models;

// Singleton to hold all the open Lobbies
public sealed class LobbyHolder {
    private static LobbyHolder instance = null;
    private static readonly object padlock = new object();
    private static readonly Dictionary<Guid, Lobby> lobbies = new Dictionary<Guid, Lobby>();
    private static Timer CleanupTimer;

    

    LobbyHolder() {
        CleanupTimer = new Timer(new TimerCallback(reference => {
            ((LobbyHolder) reference).CleanupLobbies();
        }), this, 0, 24 * 60 * 60);
    }

    private void CleanupLobbies() {
        DateTime timeToConsider = DateTime.Now.AddDays(-1);

        foreach (var item in lobbies) {
            // Check for each lobby if it has been open for more than 2 days, and remove the lobby if it is. 
            if (timeToConsider > item.Value.Created) {
                // Remove the Lobby from our dictionary so it is garbage collected.
                // Within the lobbies, there are circular references between teams / users / lobby, but those don't matter.
                // It's cleaned up regardless
                lobbies.Remove(item.Key);
            }
        }
    }

    // Make it threadsafe to create the Singleton instance by locking a shared object
    public static LobbyHolder Instance {
        get {
            lock (padlock) {
                if (instance == null) {
                    instance = new LobbyHolder();
                }
                return instance;
            }
        }
    }

    // Getter for showing the Lobbies in the main page
    public List<Lobby> getLobbies() {
        return new List<Lobby>(lobbies.Values);
    }

    // Create a new Lobby
    public Lobby createLobby(String lobbyName, User owner, String password) {
        Lobby createdLobby = new Lobby(lobbyName, owner, password);
        lobbies.Add(createdLobby.LobbyId, createdLobby);
        return createdLobby;
    }

    public void removeLobby(Lobby lobby) {
        lobbies.Remove(lobby.LobbyId);
    }

    public Lobby getLobby(Guid lobbyId) {
        lobbies.TryGetValue(lobbyId, out Lobby value);
        return value;
    }

}