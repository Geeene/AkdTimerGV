namespace AkdTimerGV.Components.Models {
    public class RaceCategory {
        /// <summary>
        /// The Name of the Category
        /// </summary>
        public String Name {  get; set; }
        /// <summary>
        /// Whether the players will be able to change the splits (for Paralogues in 3DS for example)
        /// </summary>
        public bool AllowChanges { get; set; }

        /// <summary>
        /// The List of Splits in this Category
        /// </summary>
        public List<RaceSplit> Splits { get; set; }

        private int CurrentSplit { get; set; } = 0;

        /// <summary>
        /// Clone the current RaceCategory
        /// </summary>
        public RaceCategory clone() {
            RaceCategory clone = new RaceCategory();
            clone.AllowChanges = AllowChanges;
            clone.Splits = new List<RaceSplit>(Splits.Select(split => split.Clone()));
            clone.Name = Name;
            return clone;
        }

        /// <summary>
        /// Returns the next upcoming split, or the last split if there are no more that are upcoming
        /// </summary>
        public RaceSplit GetCurrentSplit() {
            if (CurrentSplit == Splits.Count) {
                return Splits[Splits.Count-1];
            }
            return Splits[CurrentSplit];
;
        }

        public void DoSplit(long NewActiveTime) {
            DoSplit(CurrentSplit, NewActiveTime);
            CurrentSplit++;

        }

        public void DoSplit(int relevantSplit, long NewActiveTime) {
            long previousActiveTime = 0;
            if (CurrentSplit > 0) {
                // To match the way LiveSplit handles Skipped splits (and to properly handle the case
                // that like in New Mystery, some teams might take different gaiden chapters. 
                // Whenever a split is skipped, just base the reference on the next previous split
                int PreviousSplitNumber = relevantSplit - 1;
                RaceSplit Previous = Splits[PreviousSplitNumber];
                while (Previous.Skipped) {
                    PreviousSplitNumber--;

                    if (PreviousSplitNumber < 0) {
                        break;
                    }

                    Previous = Splits[PreviousSplitNumber];
                }

                if (!Previous.Skipped) {
                    previousActiveTime = Previous.SplitTimestamp;
                } else {
                    previousActiveTime = NewActiveTime;
                }
            }

            RaceSplit Current = Splits[relevantSplit];
            Current.SplitTimestamp = NewActiveTime;
            Current.SplitDuration = NewActiveTime - previousActiveTime;
            Current.Skipped = false;
        }

        public void UndoSplit() {
            if (CurrentSplit <= 0) {
                return;
            }

            CurrentSplit--;
            Splits[CurrentSplit].SplitTimestamp = 0;
            Splits[CurrentSplit].SplitDuration = 0;
            Splits[CurrentSplit].Skipped = false;
        }

        public void SkipSplit() {
            if (CurrentSplit > Splits.Count - 1) {
                return;
            }

            Splits[CurrentSplit].SplitTimestamp = 0;
            Splits[CurrentSplit].SplitDuration = 0;
            Splits[CurrentSplit].Skipped = true;
            CurrentSplit++;
        }

    }
}
