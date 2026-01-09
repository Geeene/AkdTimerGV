// Singleton that holds all the information that was previously encoded into the OBS URLs
public sealed class LinkConfigurationHolder {
    private static LinkConfigurationHolder instance = null;
    private static readonly object padlock = new object();
    private static readonly Dictionary<Guid, Object> Configurations = [];
    LinkConfigurationHolder() {
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

    public Object GetConfiguration(Guid Guid) { 
        return Configurations[Guid];
    }

    /// <summary>
    /// Get the user with the given Id from the Dictionary
    /// </summary>
    public Guid RegisterConfiguration(Object config) {
        Guid ConfigId = Guid.NewGuid();
        Configurations.Add(ConfigId, config);
        return ConfigId;
    }
}