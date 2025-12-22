namespace AkdTimerGV.Components.Draft {
    public class DraftGrouping(String Name) {
        public String Name { get; set; } = Name;

        public String ShortName { get; set; }

        public int Column { get; set; }
        public int Row { get; set; }
        public bool IsPlayer { get; set; } = false;

        public List<DraftCharacter> Characters { get; set; } = [];

        public void UpdateCharacters(IEnumerable<DraftCharacter> Characters) { this.Characters = new List<DraftCharacter>(Characters); }

        public DraftGrouping clone() {
            var clone = new DraftGrouping(Name);
            clone.Characters = new List<DraftCharacter>(Characters);
            return clone;
        }
        
    }
}
