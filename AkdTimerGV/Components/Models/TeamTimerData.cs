using AkdTimerGV.Components.Draft;
namespace AkdTimerGV.Components.Models {
    
    /// <summary>
    /// Timer for a Team. Does all the calculations of how much time has passed / break time is remaining etc.
    /// </summary>
    public class TeamTimerData {

        /// <summary>
        /// Wether the timer is currently active
        /// </summary>
        public Boolean Active = false;

        /// <summary>
        /// Wether the timer was DNFd
        /// </summary>
        public Boolean dnf = false;

        public int? Standing { get; set; }

        /// <summary>
        /// If the team is currently on break
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// Time the last active segment was started
        /// </summary>
        private DateTime? StartTime { get; set; }

        /// <summary>
        /// The time when the current break was started
        /// </summary>
        private DateTime? StartCurrentBreak { get; set; }
        
        /// <summary>
        /// The time when the player last finished the race
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// Total that the Team was actively running in MILLISECONDS
        /// </summary>
        private long ActiveMilliseconds { get; set; }

        /// <summary>
        /// how much of the break / penalty time has already been consumed in MILLISECONDS
        /// </summary>
        public long ConsumedBreakTime { get; set; }

        /// <summary>
        /// how much break time the team has in MILLISECONDS
        /// </summary>
        public long BreakTime { get; set; }

        /// <summary>
        /// How much penalty time the team has in MILLISECONDS
        /// </summary>
        public long PenaltyTime { get; set; }

        /// <summary>
        /// How much time the team has from a previous session of the race in MILLISECONDS
        /// </summary>
        public long PreviousTime { get; set; } = 0;

        /// <summary>
        /// The Characters this team drafted
        /// </summary>
        public List<DraftCharacter> DraftedCharacters { get; set; } = [];


        /// <summary>
        /// Start the break, mainly sets the StartCurrentBreak to now
        /// </summary>
        public void StartBreak() {
            // already paused, do nothing
            if (Paused || !Active) {
                return;
            }

            // If no more break time remains, then jump out
            if (GetRemainingExtraTime() <= 0) return;

            // Set makers for calculation
            StartCurrentBreak = DateTime.Now;
            ActiveMilliseconds = GetElapsedActiveTime();
            Paused = true;
            StartTime = null;
        }

        /// <summary>
        /// Used to end the break. Adds the time that has passed since the last time a break was started to the consumedBreak time
        /// </summary>
        public void EndBreak() {
            if (StartCurrentBreak == null || !Active) {
                return;
            }
            DateTime endOfBreak = DateTime.Now;
            ConsumedBreakTime += ((long) endOfBreak.Subtract((DateTime) StartCurrentBreak).TotalMilliseconds);
            Paused = false;
            StartCurrentBreak = null;
            StartTime = endOfBreak;
        }

        /// <summary>
        /// The Total time spent while not paused.
        /// More specifically: 
        /// End time / Current time - StartTime - consumedBreakTime
        /// </summary>
        public long GetElapsedActiveTime() {
            if (!Active || Paused) {
                return ActiveMilliseconds;
            }

            if (StartTime == null) {
                return 0;
            }


            return ((long) DateTime.Now.Subtract((DateTime) StartTime).TotalMilliseconds) + ActiveMilliseconds + PreviousTime;
        }

        /// <summary>
        /// Absolute total time, i.e. TIme actually raced + Break + Penalty
        /// </summary>
        /// <returns></returns>
        public long GetTotalTime() {
            return GetElapsedActiveTime() + BreakTime + PenaltyTime;
        }

        /// <summary>
        /// The remaining time in seconds that the team still has for Breaks.
        /// 
        /// If the remaining extra time would reach 0, then this will automatically end the teams break.
        /// </summary>
        /// <returns></returns>
        public long GetRemainingExtraTime() {
            long CurrentConsumedBreakTime = ConsumedBreakTime;
            if (Paused) {
                CurrentConsumedBreakTime += ((long) DateTime.Now.Subtract((DateTime) StartCurrentBreak).TotalMilliseconds);
            }
            long remainingExtraTime = BreakTime - CurrentConsumedBreakTime;
            // If the timer is currently paused, and we just reached no remaining time, then end the break automatically
            if (Paused && remainingExtraTime <= 0) EndBreak();
            return remainingExtraTime;
        }

        /// <summary>
        /// Add active time to this timer
        /// </summary>
        /// <param name="ActiveTimeToAdd"></param>
        public void AddActiveTime(long ActiveTimeToAdd) {
            ActiveMilliseconds += ActiveTimeToAdd;
        }

        /// <summary>
        /// Adds the given number of Seconds to the penalty time
        /// </summary>
        /// <param name="penaltyTimeToAdd">Must be given in SECONDS</param>
        public void AddPenaltyTime(long penaltyTimeToAdd) {
            PenaltyTime += penaltyTimeToAdd;
            if (PenaltyTime < 0) {
                PenaltyTime = 0;
            }
        }

        /// <summary>
        /// Adds the given number of Seconds to the break time
        /// </summary>
        /// <param name="breakTimeToAdd">Must be given in SECONDS</param>
        public void AddBreakTime(long breakTimeToAdd) {
            BreakTime += breakTimeToAdd;
            if (BreakTime < 0) {
                BreakTime = 0;
            }
        }

        /// <summary>
        /// Adds the given number of Seconds to the Previous time
        /// </summary>
        /// <param name="previousTimeToAdd"></param>
        public void AddPreviousTime(long previousTimeToAdd) { 
            PreviousTime += previousTimeToAdd;
        }

        /// <summary>
        /// Start the timer for this Team
        /// </summary>
        public void Start() {
            StartTime = DateTime.Now;
            Active = true;

            if (ActiveMilliseconds > 0 && PreviousTime > 0) {
                ActiveMilliseconds -= PreviousTime;
            }
        }

        /// <summary>
        /// To be pressed when finishing the race
        /// </summary>
        public void Finish() {
            EndBreak();
            ActiveMilliseconds = GetElapsedActiveTime();
            Active = false;
            FinishTime = DateTime.Now;
        }

        /// <summary>
        /// To be pressed when finishing the race
        /// </summary>
        public void UndoFinish() {
            ActiveMilliseconds += ((long)DateTime.Now.Subtract((DateTime) FinishTime).TotalMilliseconds);
            Active = true;
            FinishTime = null;
        }

        /// <summary>
        /// To be pressed when finishing the race
        /// </summary>
        public void confirmDNF() {
            Finish();
            dnf = true;
        }


        /// <summary>
        /// Returns true if the timer is not paused, needed because the html is a bit wacky with adding an ! into the disabled
        /// </summary>
        /// <returns></returns>
        public bool IsNotPaused() {
            return !Paused;
        }


        /// <summary>
        /// Get the sum of breaktime + penalty
        /// </summary>
        /// <returns></returns>
        public long GetExtraTime() {
            return BreakTime;
        }

        /// <summary>
        /// Get the Sum of Active time + Penalty + Previous Time. This specifically does not include break
        /// </summary>
        /// <returns></returns>
        public long GetSubmittedTime() {
            return GetElapsedActiveTime() + PenaltyTime + PreviousTime;
        }

        /// <summary>
        /// Reset all values
        /// </summary>
        public void Reset() {
            Active = false;
            ActiveMilliseconds = 0;
            BreakTime = 0;
            PenaltyTime = 0;
            StartCurrentBreak = null;
            StartTime = null;
            Paused = false;
            ConsumedBreakTime = 0;
            PreviousTime = 0;
            dnf = false;
        }
    }
}
