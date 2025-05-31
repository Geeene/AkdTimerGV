using AkdTimerGV.Components.Models;

// Singleton to hold all the open Lobbies
public sealed class LobbyHolder {
    private static LobbyHolder instance = null;
    private static readonly object padlock = new object();
    private static readonly Dictionary<Guid, Lobby> lobbies = new Dictionary<Guid, Lobby>();
    LobbyHolder() {
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
    public static List<Lobby> getLobbies() {
        return new List<Lobby>(lobbies.Values);
    }

    // Create a new Lobby
    public static Lobby createLobby(String lobbyName, User owner, String password) {
        Lobby createdLobby = new Lobby(lobbyName, owner, password);
        lobbies.Add(createdLobby.LobbyId, createdLobby);
        return createdLobby;
    }

    public static void removeLobby(Lobby lobby) {
        lobbies.Remove(lobby.LobbyId);
    }

    public static Lobby getLobby(Guid lobbyId) {
        lobbies.TryGetValue(lobbyId, out Lobby value);
        return value;
    }

}