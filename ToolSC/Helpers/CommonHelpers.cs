using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using ToolSC.Models;

namespace ToolSC.Helpers
{
    public static class CommonHelpers
    {
        public static int CountByteLength(string input)
        {
            // Chuyển chuỗi sang mảng byte sử dụng UTF-8 encoding
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(input);
            return utf8Bytes.Length;
        }

        public static List<TableColumn> GetNameAndTypeOfColumn(string input)
        {
            var data = new List<TableColumn>();

            // Biểu thức chính quy
            string pattern = @"<(.*?),\s*(varchar|int)(?:\((\d+)\))?,>";

            // Sử dụng Regex.Matches để tìm kiếm tất cả các kết quả
            MatchCollection matches = Regex.Matches(input, pattern);

            // Kiểm tra xem có kết quả nào không
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    data.Add(new TableColumn
                    {
                        Name = match.Groups[1].Value,
                        Type = match.Groups[2].Value,
                        Length = match.Groups[3].Value
                    });
                }
            }

            return data;
        }

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            Random random = new();
            return random.Next(minValue, maxValue + 1);
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new();

            // Use Linq to create a random string of the desired length
            string randomString = new(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomString;
        }

        public static string GenerateUniqueRandomString(int length, List<string> existList)
        {
            string randomString;
            do
            {
                randomString = GenerateRandomString(length);
            } while (existList.Contains(randomString));

            return randomString;
        }

        public static Tuple<int, int> GetRangeOfLengthForRandomString(int length)
        {
            if (length <= 6)
            {
                return Tuple.Create(length, length);
            }

            return Tuple.Create(4, 6);
        }

        public static string GenerateColumnDataNumber(TableColumn column, List<string> existList)
        {
            string data = "";
            if (column.Type == "varchar")
            {
                return data;
            }

            string randomNumber;
            do
            {
                randomNumber = GenerateRandomNumber(10, 100).ToString();
            } while (existList.Contains(randomNumber));

            return randomNumber;
        }

        public static string GenerateColumnDataFromColumnName(TableColumn column, List<string> existList)
        {
            string data = "";
            if (column.Type != "varchar")
            {
                return data;
            }

            int columnNameByteLength = CountByteLength(column.Name);
            if (columnNameByteLength <= int.Parse(column.Length))
            {
                return column.Name;
            }

            do
            {
                int endNumSubstr = GenerateRandomNumber(1, column.Name.Length / 2);
                data = column.Name.Substring(0, endNumSubstr);
            } while (existList.Contains(data));

            return data;
        }

        public static string GenerateColumnDataFromRandomString(TableColumn column, List<string> existList)
        {
            string data = "";
            if (column.Type != "varchar")
            {
                return data;
            }

            do
            {
                var range = GetRangeOfLengthForRandomString(int.Parse(column.Length));
                int length = GenerateRandomNumber(range.Item1, range.Item2);
                data = GenerateRandomString(length);
            } while (existList.Contains(data));

            return data;
        }

        public static string GenerateColumnDataFullLengthFromColumnName(TableColumn column, List<string> existList)
        {
            string data = "";
            if (column.Type != "varchar")
            {
                return data;
            }

            int columnLength = int.Parse(column.Length);
            int columnNameByteLength = CountByteLength(column.Name);
            if (columnNameByteLength <= columnLength)
            {
                data = DuplicateColumnName(column.Name, columnLength);
            }
            else
            {
                do
                {
                    data = GenerateRandomString(columnLength);
                } while (existList.Contains(data));
            }

            return data;
        }

        public static string DuplicateColumnName(string name, int length)
        {
            string dataName = "";
            int columnNameByteLength = CountByteLength(name);
            int duplicator = length / columnNameByteLength;

            for (int i = 0; i < duplicator; i++)
            {
                dataName += name;
            }

            int diff = length - CountByteLength(dataName);
            if (diff > 0)
            {
                dataName += GenerateRandomString(diff);
            }

            return dataName;
        }

        public static string CombineDataString(List<string> stringList)
        {
            // Sử dụng LINQ để bọc mỗi phần tử trong dấu ngoặc đơn
            var wrappedItems = stringList.Select(item => $"'{item}'\n");

            string combinedString = string.Join(",", wrappedItems);

            return $"({combinedString})";
        }
    }
}
