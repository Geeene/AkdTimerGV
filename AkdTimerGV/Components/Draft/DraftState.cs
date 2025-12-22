using AkdTimerGV.Components.Models;

namespace AkdTimerGV.Components.Draft {
    /// <summary>
    /// All State logic for the Drafting Process
    /// </summary>
    public class DraftState {
        public DraftFlowState FlowState { get; set; } = DraftFlowState.NOT_STARTED;

        /// <summary>
        /// All DraftGroupings, effectively representing tiers from the old tiermaker.
        /// Key = Game ShortName
        /// Value = DraftGrouping
        /// </summary>
        public Dictionary<String, DraftGrouping> DraftGroupings { get; set; } = [];

        /// <summary>
        ///  Dictionary for choosing the game.
        ///  Key = Game ShortName
        ///  Value = If the games characters should be added to the available character pool
        /// </summary>
        public Dictionary<String, bool> ChosenGameDict { get; set; } = [];

        /// <summary>
        /// The Draft Group containing all characters that are available, and not sorted in a tier yet
        /// </summary>
        public DraftGrouping AvailableCharacters { get; set; } = new DraftGrouping("Available");

        /// <summary>
        /// Action that will be used to notify subscribers of the state changes.
        /// </summary>
        public event Action? OnStateChange;

        /// <summary>
        /// Draft Order, not a list to conform to the inplace-shuffle API
        /// </summary>
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

        /// <summary>
        /// Perform the first roll of the Draft Order, this also initialized the List of Teams
        /// </summary>
        public void InitializeDraftOrder(List<Team> teams) {
            if (RandomizedDraftOrder.Count() == 0) {
                RandomizedDraftOrder = new List<String>(teams.Select(currentTeam => currentTeam.Name)).ToArray();
                RerollOrder();
                AdvanceState();
            }

            NotifySubscribers();
        }

        /// <summary>
        /// Reroll the draft order by shuffling the list in place
        /// </summary>
        public void RerollOrder() {
            Random.Shared.Shuffle(RandomizedDraftOrder);
            NotifySubscribers();
        }

        /// <summary>
        /// Reset the Draft to the original status
        /// </summary>
        public void Reset() {
            FlowState = DraftFlowState.START;
            DraftGroupings = [];
            ResetSelectedGames();
            RandomizedDraftOrder = [];
            NotifySubscribers();
        }

        /// <summary>
        /// Reset specifically the Chosen Games and Available characters.
        /// Used both for a full reset, and for loading from the tiermaker code
        /// </summary>
        public void ResetSelectedGames() {
            foreach (var item in DraftCharacterCache.DraftGroupings.Values) {
                ChosenGameDict[item.ShortName] = false;
            }
            AvailableCharacters = new DraftGrouping("Available");
        }

        /// <summary>
        /// Advance the Statemachine by one state
        /// </summary>
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
