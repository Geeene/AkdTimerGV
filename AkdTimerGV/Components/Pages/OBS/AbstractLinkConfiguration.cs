namespace AkdTimerGV.Components.Pages.OBS {
    public class AbstractLinkConfiguration {
        public DateTime CreationTime { get; } = DateTime.Now;
        public Guid ConfigId { get; } = Guid.NewGuid();
    }
}
