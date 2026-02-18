namespace AkdTimerGV.Components.Draft {
    /// <summary>
    /// Represents a group of characters in the Draft
    /// </summary>
    public class DraftGrouping(String Name, String InternalName) {
        /// <summary>
        /// Name, primarily used for the game selection.
        /// Depending on context this may be the short name or a special display name (
        /// </summary>
        public String Name { get; set; } = Name;

        /// <summary>
        /// Short Name of the Game, this is primarily important for loading the characters from the JSON File
        /// </summary>
        public String InternalName { get; set; } = InternalName;

        /// <summary>
        /// Used for the Grid Layout on the Game selection screen
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Used for the Grid Layout on the Game selection screen
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Whether this is a grouping Created automatically by the timer
        /// </summary>
        public bool IsPlayer { get; set; } = false;

        /// <summary>
        /// The order to display the Draft Groupings in the Tiermaker
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The Characters grouped by this object
        /// </summary>
        public List<DraftCharacter> Characters { get; set; } = [];

        /// <summary>
        /// For each of these names the participant will get an additional row
        /// </summary>
        public List<String> AdditionalRowNames { get; set; } = [];

        /// <summary>
        /// Remaining Amount for the Auction
        /// </summary>
        public int RemainingAuctionCurrency { get; set; } = 100;

        /// <summary>
        /// Hex Code for the Background color of the name plate
        /// </summary>
        public String Color { get; set; } = "#1A1A1A";

        /// <summary>
        /// Clone this Draft Grouping
        /// </summary>
        public DraftGrouping clone() {
            var clone = new DraftGrouping(Name, InternalName);
            clone.Characters = new List<DraftCharacter>(Characters);
            clone.IsPlayer = this.IsPlayer;
            clone.Order = this.Order;
            clone.RemainingAuctionCurrency = this.RemainingAuctionCurrency;
            clone.Color = this.Color;
            return clone;
        }

        public void RecalculateAuctionCost() {
            RemainingAuctionCurrency = 100 - Characters.Select(chara => chara.AuctionCost).Sum();
        }
    }
}
