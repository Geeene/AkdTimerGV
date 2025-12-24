namespace AkdTimerGV.Components.Draft {
    /// <summary>
    /// Lobby for Drafting Only for using without having a timer lobby
    /// </summary>
    public class DraftLobby : IDraftStateHolder {

        private Guid _id = Guid.NewGuid();

        private DraftState _draftState = new DraftState(DraftFlowState.ENTER_PARTICIPANTS);

        private List<String> _participants = [];

        public DraftState GetDraftState() {
            return _draftState;
        }

        public List<String> GetParticipants() {
            return _participants;
        }
        
        public void SetParticipants(List<String> Participants) {
            this._participants = Participants;
        }

        public Guid GetId() {
            return _id;
        }
    }
}
