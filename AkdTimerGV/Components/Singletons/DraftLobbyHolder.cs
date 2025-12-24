using AkdTimerGV.Components.Models;
using AkdTimerGV.Components.Draft;

// Singleton to hold all the open Draft Lobbies
public sealed class DraftLobbyHolder {
    private static DraftLobbyHolder instance = null;
    private static readonly object padlock = new object();
    private static readonly Dictionary<Guid, DraftLobby> stateHolders = new Dictionary<Guid, DraftLobby>();
    private static Timer CleanupTimer;

    

    DraftLobbyHolder() {
        CleanupTimer = new Timer(new TimerCallback(reference => {
            ((DraftLobbyHolder) reference).CleanupLobbies();
        }), this, 0, 24 * 60 * 60);
    }

    private void CleanupLobbies() {
    }

    // Make it threadsafe to create the Singleton instance by locking a shared object
    public static DraftLobbyHolder Instance {
        get {
            lock (padlock) {
                if (instance == null) {
                    instance = new DraftLobbyHolder();
                }
                return instance;
            }
        }
    }

    public DraftLobby CreateDraftLobby() {
        DraftLobby newStateHolder = new DraftLobby();
        stateHolders.Add(newStateHolder.GetId(), newStateHolder);
        return newStateHolder; 
    }

    public DraftLobby GetById(Guid id) {
        stateHolders.TryGetValue(id, out DraftLobby value);
        return value;
    }

}