namespace AkdTimerGV.Components.Draft {
    public class DraftGroupingColors {
        private static readonly List<String> DefaultColors = ["#FF7F7F", "#FFBF7F", "#FFDF7F", 
            "#FFFF7F", "#BFFF7F", "#7FFF7F", "#7FFFFF", 
            "#7FBFFF", "#7F7FFF", "#FF7FFF", "#BF7FBF", 
            "#3B3B3B", "#858585", "#CFCFCF", "#F7F7F7"];

        private int NextIndex = 0;


        /// <summary>
        /// Get the next Color from the DefaultColor Sequence
        /// </summary>
        /// <returns></returns>
        public String GetNextDefaultColor() {
            String NextColor = DefaultColors[NextIndex];
            if (NextIndex < DefaultColors.Count() -1) { 
                NextIndex++;
            }
            return NextColor;
        }
    }
}
