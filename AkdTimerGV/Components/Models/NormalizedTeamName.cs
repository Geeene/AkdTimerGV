namespace AkdTimerGV.Components.Models {
    /// <summary>
    /// Wrapper class around a string to show that it acts as a Normalized Team name. 
    /// Less confusing when used as a Map Key, which one it should actually be.
    /// 
    /// Normalized Team Name is used for some scenarios where having spaces is undesirable, 
    /// such as in the URL when generating the Stream Link
    /// </summary>
    /// <param name="Name"></param>
    public class NormalizedTeamName(String Name) {
        /// <summary>
        /// Team Name normalized to upper case and removing spaces
        /// </summary>
        public String NormalizedName { get; set; } = Name.ToUpper().Replace(" ", "");

        public override bool Equals(object? obj) {
            return obj is NormalizedTeamName name &&
                   NormalizedName == name.NormalizedName;
        }

        public override int GetHashCode() {
            return HashCode.Combine(NormalizedName);
        }

        /// <summary>
        /// Convenience Getter
        /// </summary>
        /// <returns></returns>
        public String get() {
            return NormalizedName;
        }
    }
}
