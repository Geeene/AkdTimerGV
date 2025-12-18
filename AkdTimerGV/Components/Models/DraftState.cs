namespace AkdTimerGV.Components.Models {
    public class DraftState {
        public DraftFlowState FlowState { get; set; } = DraftFlowState.NOT_STARTED;

        public Dictionary<String, DraftGrouping> DraftGroupings { get; set; } = [];

    }
}
