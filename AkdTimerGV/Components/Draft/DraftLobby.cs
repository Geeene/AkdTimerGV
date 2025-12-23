namespace AkdTimerGV.Components.Draft {
    /// <summary>
    /// Lobby for Drafting Only for using without having a timer lobby
    /// </summary>
    public class DraftLobby : IDraftStateHolder {

        private Guid _id = new Guid();

        private DraftState _draftState = new DraftState(DraftFlowState.ENTER_PARTICIPANTS);

        private String[] _participants = [];

        public DraftState GetDraftState() {
            return _draftState;
        }

        public string[] GetParticipants() {
            return _participants;
        }
        
        public void SetParticipants(String[] Participants) {
            this._participants = Participants;
        }

        public Guid GetId() {
            return _id;
        }
    }
}
