using System.Text;

namespace AkdTimerGV.Components.Models {
    public class TimeUtil {

        public static String getTimeAsString(long timeInMillis) {
            return getTimeAsString(timeInMillis, false);
        }
        public static String getTimeAsString(long timeInMillis, bool includingMillis) {
            long timeInSeconds = timeInMillis / 1000;
            long numberSeconds = timeInSeconds % 60;

            long timeWithoutSeconds = timeInSeconds - numberSeconds;

            long numberMinutes = (timeWithoutSeconds % (60 * 60)) / 60;

            long timeHoursOnly = timeWithoutSeconds - (timeWithoutSeconds % (60 * 60));
            long numberHours = timeHoursOnly / (60 * 60);

            StringBuilder stringBuilder = new();

            if (numberHours > 0) {
                stringBuilder.Append(numberHours + ":");
            }

            if (numberHours > 0 && numberMinutes < 10) {
                stringBuilder.Append('0');
                if (numberMinutes == 0) {
                    stringBuilder.Append("0:");
                }
            }
            if (numberMinutes > 0) { 
                stringBuilder.Append(numberMinutes + ":");
            }

            if (numberSeconds < 10 && (numberMinutes > 0 || numberHours > 0)) {
                stringBuilder.Append('0');
            }
            stringBuilder.Append(numberSeconds);

            if (includingMillis) {
                stringBuilder.Append(".").Append((timeInMillis % 1000).ToString("000"));

            }

            return stringBuilder.ToString();
        }
    }
}
