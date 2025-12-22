namespace AkdTimerGV.Components.Draft {
    public class DraftCharacter(String ShortName, String ImagePath) {
        public String Name { get; }
        public String ShortName { get; } = ShortName;
        public String ImagePath { get; } = ImagePath;
    }
}
