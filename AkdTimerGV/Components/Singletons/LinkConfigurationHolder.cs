// Singleton that holds all the information that was previously encoded into the OBS URLs
using AkdTimerGV.Components.Pages.OBS;

public sealed class LinkConfigurationHolder {
    private static LinkConfigurationHolder instance = null;
    private static readonly object padlock = new object();
    private static readonly Dictionary<Guid, AbstractLinkConfiguration> Configurations = [];
    private static Timer CleanupTimer;

    LinkConfigurationHolder() {
        CleanupTimer = new Timer(new TimerCallback(reference => {
            ((LinkConfigurationHolder) reference).Cleanup();
        }), this, 0, 24 * 60 * 60);
    }

    // Make it threadsafe to create the Singleton instance by locking a shared object
    public static LinkConfigurationHolder Instance {
        get {
            lock (padlock) {
                if (instance == null) {
                    instance = new LinkConfigurationHolder();
                }
                return instance;
            }
        }
    }

    public void Cleanup() {
        DateTime timeToConsider = DateTime.Now.AddHours(-6);
        foreach (var item in Configurations.Values.Where(config => timeToConsider > config.CreationTime)) {
            Configurations.Remove(item.ConfigId);
        }
    }

    public Object GetConfiguration(Guid Guid) { 
        return Configurations[Guid];
    }

    /// <summary>
    /// Get the user with the given Id from the Dictionary
    /// </summary>
    public Guid RegisterConfiguration(AbstractLinkConfiguration config) {
        Configurations.Add(config.ConfigId, config);
        return config.ConfigId;
    }
}