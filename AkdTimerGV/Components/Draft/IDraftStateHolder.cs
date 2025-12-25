namespace AkdTimerGV.Components.Draft {
    /// <summary>
    /// Interface to allow accessing the draftstate for both Lobby / Draft Lobby
    /// </summary>
    public interface IDraftStateHolder {

        public Guid GetId();

        public List<String> GetParticipants();
        public void SetParticipants(List<String> Participants);

        public DraftState GetDraftState();

        public void Reset();

    }
}
