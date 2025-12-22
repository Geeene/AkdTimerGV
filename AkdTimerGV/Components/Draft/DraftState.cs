using AkdTimerGV.Components.Models;

namespace AkdTimerGV.Components.Draft {
    public class DraftState {
        public DraftFlowState FlowState { get; set; } = DraftFlowState.NOT_STARTED;

        public Dictionary<String, DraftGrouping> DraftGroupings { get; set; } = [];
        public Dictionary<String, bool> ChosenGameDict { get; set; } = [];

        public DraftGrouping AvailableCharacters { get; set; } = new DraftGrouping("Available");

        public event Action? OnStateChange;

        public String[] RandomizedDraftOrder = [];

        public DraftState() {
            ResetSelectedGames();
        }

        /// <summary>
        /// Update the State for the given Groupings
        /// </summary>
        /// <param name="draftGrouping"></param>
        /// <param name="AvailableCharacters"></param>
        public void UpdateState(List<DraftGrouping> draftGroupings, DraftGrouping AvailableCharacters) {
            this.AvailableCharacters = AvailableCharacters.clone();
            foreach (DraftGrouping grouping in draftGroupings) {
                DraftGroupings[grouping.Name] = grouping.clone();
            }
            NotifySubscribers();
        }

        /// <summary>
        /// Allows a Page Bean to subscribe to state changes, so that anytime an action is commited in the draft all other users see it aswell.
        /// </summary>
        /// <param name="subscriber"></param>
        public void Subscribe(DraftFlow subscriber) {
            OnStateChange += () => subscriber.refreshFromDraftState(true);
        }

        /// <summary>
        /// Allows a Page Bean to unsubscribe from state changes
        /// </summary>
        /// <param name="subscriber"></param>
        public void Unsubscribe(DraftFlow subscriber) {
            OnStateChange -= () => subscriber.refreshFromDraftState(true);
        }

        /// <summary>
        /// Inform each subscriber that the state has changed
        /// </summary>
        public void NotifySubscribers() {
            OnStateChange?.Invoke();
        }

        /// <summary>
        /// To be used for initializing the free character pool
        /// </summary>
        /// <param name="NewCharacters"></param>
        public void SetAvailableCharacters(IEnumerable<DraftCharacter> NewCharacters) { 
            AvailableCharacters.Characters = new List<DraftCharacter>(NewCharacters);
            NotifySubscribers();
        }

        public void InitializeDraftOrder(List<Team> teams) {
            if (RandomizedDraftOrder.Count() == 0) {
                RandomizedDraftOrder = new List<String>(teams.Select(currentTeam => currentTeam.Name)).ToArray();
                RerollOrder();
                FlowState = DraftFlowState.ORDER_RANDOMIZED;
            }
            NotifySubscribers();
        }

        public void RerollOrder() {
            Random.Shared.Shuffle(RandomizedDraftOrder);
            NotifySubscribers();
        }

        public void Reset() {
            FlowState = DraftFlowState.START;
            DraftGroupings = [];
            AvailableCharacters = new DraftGrouping("Available");
            RandomizedDraftOrder = [];
            NotifySubscribers();
        }

        public void ResetSelectedGames() {
            foreach (var item in DraftCharacterCache.GetGroupingsOrdered()) {
                ChosenGameDict[item.ShortName] = false;
            }
            AvailableCharacters = new DraftGrouping("Available");
        }

        public void AdvanceState() {
            switch (this.FlowState) {
                case DraftFlowState.NOT_STARTED:
                    this.FlowState = DraftFlowState.START;
                    break;
                case DraftFlowState.START:
                    this.FlowState = DraftFlowState.ORDER_RANDOMIZED;
                    break;
                case DraftFlowState.ORDER_RANDOMIZED:
                    this.FlowState = DraftFlowState.CHOOSE_GAME;
                    break;
                case DraftFlowState.CHOOSE_GAME:
                    this.FlowState = DraftFlowState.DRAFTING_STARTED;
                    break;
                case DraftFlowState.DRAFTING_STARTED:
                    this.FlowState = DraftFlowState.DRAFTING_FINISHED;
                    break;
            }
            NotifySubscribers();
        }
    }
}
