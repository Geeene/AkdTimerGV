using AkdTimerGV.Components.Models;

namespace AkdTimerGV.Components.Draft {
    public class DraftState {
        public DraftFlowState FlowState { get; set; } = DraftFlowState.NOT_STARTED;

        public Dictionary<String, DraftGrouping> DraftGroupings { get; set; } = [];

        public DraftGrouping AvailableCharacters { get; set; } = new DraftGrouping("Available");

        public List<Action> StateSubscribers = [];

        public String[] RandomizedDraftOrder = [];


        /// <summary>
        /// Update the State for the given Groupings
        /// </summary>
        /// <param name="draftGrouping"></param>
        /// <param name="AvailableCharacters"></param>
        public void UpdateState(List<DraftGrouping> draftGroupings, DraftGrouping AvailableCharacters) {
            AvailableCharacters = AvailableCharacters.clone();
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
            StateSubscribers.Add(() => subscriber.refreshFromDraftState(true));
        }

        /// <summary>
        /// Inform each subscriber that the state has changed
        /// </summary>
        public void NotifySubscribers() {
            foreach (var action in StateSubscribers) {
                action.Invoke();
            }
        }

        /// <summary>
        /// To be used for initializing the free character pool
        /// </summary>
        /// <param name="NewCharacters"></param>
        public void AddMoreCharacters(IEnumerable<DraftCharacter> NewCharacters) { 
            AvailableCharacters.AddCharacters(NewCharacters);
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

    }
}
