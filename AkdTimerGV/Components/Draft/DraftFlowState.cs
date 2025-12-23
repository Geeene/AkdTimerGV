namespace AkdTimerGV.Components.Draft {
    /// <summary>
    /// The states for the DraftState machine
    /// </summary>
    public enum DraftFlowState {
        NOT_STARTED, ENTER_PARTICIPANTS, INITIALIZE_DRAFT_ORDER, ORDER_RANDOMIZED, CHOOSE_GAME, DRAFTING_STARTED, DRAFTING_FINISHED
    }
}
