using AkdTimerGV.Components.Models;

namespace AkdTimerGV.Components.Draft {
    /// <summary>
    /// All State logic for the Drafting Process
    /// </summary>
    public class DraftState(DraftFlowState InitialFlowState) {
        public DraftFlowState FlowState { get; set; } = DraftFlowState.NOT_STARTED;
        public DraftFlowState InitialFlowState { get;} = InitialFlowState;

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
        public Dictionary<String, bool> ChosenGameDict { get; set; } = InitializeChosenGameDict();

        /// <summary>
        /// The Draft Group containing all characters that are available, and not sorted in a tier yet
        /// </summary>
        public DraftGrouping AvailableCharacters { get; set; } = new DraftGrouping("Available", "Available");

        /// <summary>
        /// Action that will be used to notify subscribers of the state changes.
        /// </summary>
        public event Action? OnStateChange;

        /// <summary>
        /// Draft Order, not a list to conform to the inplace-shuffle API
        /// </summary>
        public String[] RandomizedDraftOrder = [];

        /// <summary>
        /// Last time the state has changed, used for cache cleanup
        /// </summary>
        public DateTime LastChange = DateTime.Now;

        /// <summary>
        /// If Auction Mode is enabled
        /// </summary>
        public bool AuctionMode { get; set; } = false;

        /// <summary>
        /// Current Nomination for Auction Mode
        /// </summary>
        public DraftGrouping Nomination { get; set; } = new DraftGrouping("Nomination", "Nomination");

        /// <summary>
        /// Update the State for the given Groupings
        /// </summary>
        /// <param name="draftGrouping"></param>
        /// <param name="AvailableCharacters"></param>
        public void UpdateState(List<DraftGrouping> draftGroupings, DraftGrouping AvailableCharacters, DraftGrouping Nomination) {
            this.AvailableCharacters = AvailableCharacters.clone();
            DraftGroupings.Clear();
            foreach (DraftGrouping grouping in draftGroupings) {
                DraftGroupings[grouping.Name] = grouping.clone();
                DraftGroupings[grouping.Name].RecalculateAuctionCost();
            }
            this.Nomination = Nomination;
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
            this.LastChange = DateTime.Now;
            OnStateChange?.Invoke();
        }

        /// <summary>
        /// To be used for initializing the free character pool
        /// </summary>
        /// <param name="NewCharacters"></param>
        public void SetAvailableCharacters(IEnumerable<DraftCharacter> NewCharacters) { 
            AvailableCharacters.Characters = new List<DraftCharacter>(NewCharacters.Select(chara => new DraftCharacter(chara.InternalName, chara.ImagePath)));
            NotifySubscribers();
        }

        /// <summary>
        /// Perform the first roll of the Draft Order, this also initialized the List of Teams
        /// </summary>
        public void InitializeDraftOrder(List<String> participants) {
            if (RandomizedDraftOrder.Count() == 0) {
                RandomizedDraftOrder = participants.ToArray();
                RerollOrder();
                AdvanceState();
            }

            NotifySubscribers();
        }
        /// <summary>
        /// Perform the first roll of the Draft Order, this also initialized the List of Teams
        /// </summary>
        public void SetDraftOrderManually(List<String> participants) {
            if (RandomizedDraftOrder.Count() == 0) {
                RandomizedDraftOrder = participants.ToArray();
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
            FlowState = InitialFlowState;
            DraftGroupings = [];
            Nomination = new DraftGrouping("Nomination", "Nomination");
            ResetSelectedGames();
            RandomizedDraftOrder = [];
            AuctionMode = false;
            NotifySubscribers();
        }

        /// <summary>
        /// Reset specifically the Chosen Games and Available characters.
        /// Used both for a full reset, and for loading from the tiermaker code
        /// </summary>
        public void ResetSelectedGames() {
            ChosenGameDict = InitializeChosenGameDict();
            AvailableCharacters = new DraftGrouping("Available", "Available");

        }

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<String, bool> InitializeChosenGameDict() {
            Dictionary<String, bool> dict = [];
            foreach (var item in DraftCharacterCache.DraftGroupings.Values) {
                dict[item.InternalName] = false;
            }
            return dict;
        }

        public void AddDraftGrouping(DraftGrouping grouping) {
            grouping.Order = this.DraftGroupings.Count;
            this.DraftGroupings.Add(grouping.Name, grouping);
        }

        /// <summary>
        /// Advance the Statemachine by one state
        /// </summary>
        public void AdvanceState() {
            switch (this.FlowState) {
                case DraftFlowState.NOT_STARTED:
                    this.FlowState = this.InitialFlowState;
                    break;
                case DraftFlowState.ENTER_PARTICIPANTS:
                    this.FlowState = DraftFlowState.INITIALIZE_DRAFT_ORDER;
                    break;
                case DraftFlowState.INITIALIZE_DRAFT_ORDER:
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
