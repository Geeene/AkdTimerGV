namespace AkdTimerGV.Components.Models {
    public class RaceSplit {
        /// <summary>
        /// Chapter Number
        /// </summary>
        public String Chapter { get; set; }

        /// <summary>
        /// Split Name
        /// </summary>
        public String Name { get; set; }
        
        /// <summary>
        /// Split duration in Millisecond
        /// </summary>
        public long SplitDuration { get; set; }

        /// <summary>
        /// The Active time of the team at the point of splitting
        /// </summary>
        public long SplitTimestamp { get; set; }

        public bool Skipped { get; set; }

        /// <summary>
        /// Only Manually created Splits can be deleted by a user
        /// </summary>
        public bool ManuallyCreated { get; set; } = false;

        public override bool Equals(object? obj) {
            return obj is RaceSplit split &&
                   Chapter == split.Chapter &&
                   Name == split.Name;
        }


    }
}
