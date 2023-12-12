namespace ToolSC.Helpers
{
    public class DateTimeHelpers
    {
        public static string GenerateRandomDate()
        {
            Random random = new();
            DateTime randomDateTime;

            do
            {
                int year = random.Next(2020, 2024);
                int month = random.Next(1, 13);
                int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);

                randomDateTime = new DateTime(year, month, day);

            } while (!IsValidDate(randomDateTime));

            return randomDateTime.ToString("yyyyMMdd");
        }

        public static bool IsValidDate(DateTime date)
        {
            try
            {
                DateTime dt = new(date.Year, date.Month, date.Day);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateRandomTime()
        {
            Random random = new();
            TimeSpan randomTimeSpan;

            do
            {
                // Tạo thời gian ngẫu nhiên trong khoảng từ 00:00:00 đến 23:59:59
                int hours = random.Next(0, 24);
                int minutes = random.Next(0, 60);
                int seconds = random.Next(0, 60);

                randomTimeSpan = new TimeSpan(hours, minutes, seconds);

            } while (!IsValidTime(randomTimeSpan));

            // Sắp xếp thời gian theo định dạng HHmmss
            string formattedTime = $"{randomTimeSpan.Hours:D2}{randomTimeSpan.Minutes:D2}{randomTimeSpan.Seconds:D2}";

            return formattedTime;
        }

        public static bool IsValidTime(TimeSpan time)
        {
            try
            {
                TimeSpan ts = new(time.Hours, time.Minutes, time.Seconds);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
