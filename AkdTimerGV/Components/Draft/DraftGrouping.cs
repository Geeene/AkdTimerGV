namespace AkdTimerGV.Components.Draft {
    public class DraftGrouping(String Name) {
        public String Name { get; set; } = Name;
        public List<DraftCharacter> Characters { get; set; } = [];

        public void AddCharacters(IEnumerable<DraftCharacter> NewCharacters) { 
            Characters.AddRange(NewCharacters);
        }
        public void UpdateCharacters(IEnumerable<DraftCharacter> Characters) { this.Characters = new List<DraftCharacter>(Characters); }

        public DraftGrouping clone() {
            var clone = new DraftGrouping(Name);
            clone.AddCharacters(new List<DraftCharacter>(Characters));
            return clone;
        }
        
    }
}
