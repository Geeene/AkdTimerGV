namespace AkdTimerGV.Components.Models {
    public class DraftGrouping(String Name) {
        public String Name { get; set; } = Name;
        public List<DraftCharacter> Characters { get; set; } = [];
    }
}
