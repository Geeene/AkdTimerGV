namespace AkdTimerGV.Components.Pages.OBS {
    public class OBSTimerOverlayConfiguration {
        public Guid LobbyId { get; set; }
        public Guid? TeamName { get; set; }
        public bool DisplayTable {  get; set; }
        public string? CustomOrder { get; set; }
        public bool ShowDraft { get; set; }

    }
}
