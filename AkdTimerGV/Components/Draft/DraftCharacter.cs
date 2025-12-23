namespace AkdTimerGV.Components.Draft {
    /// <summary>
    ///  Represents a draftable character
    /// </summary>
    public class DraftCharacter(String ShortName, String ImagePath) {
        /// <summary>
        /// So far unused, maybe for tooltip showing the actual name??
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// InternalName (e.g. awa-0 = chrom)
        /// </summary>
        public String InternalName { get; } = ShortName;

        /// <summary>
        /// Image path of where to find the picture to display
        /// </summary>
        public String ImagePath { get; } = ImagePath;
    }
}
