using System.Text;

namespace AkdTimerGV.Components.Models {
    public class TimeUtil {

        public static String getTimeAsString(long timeAsLong) {
            long numberSeconds = timeAsLong % 60;

            long timeWithoutSeconds = timeAsLong - numberSeconds;

            long numberMinutes = (timeWithoutSeconds % (60 * 60)) / 60;

            long timeHoursOnly = timeWithoutSeconds - (timeWithoutSeconds % (60 * 60));
            long numberHours = timeHoursOnly / (60 * 60);

            StringBuilder stringBuilder = new StringBuilder();

            if (numberHours > 0) {
                stringBuilder.Append(numberHours + ":");
            }

            if (numberHours > 0 && numberMinutes < 10) {
                stringBuilder.Append('0');
            }
            if (numberMinutes > 0) { 
                stringBuilder.Append(numberMinutes + ":");
            }

            if (numberSeconds < 10 && (numberMinutes > 0 || numberHours > 0)) {
                stringBuilder.Append('0');
            }
            stringBuilder.Append(numberSeconds);

            return stringBuilder.ToString();
        }
    }
}
