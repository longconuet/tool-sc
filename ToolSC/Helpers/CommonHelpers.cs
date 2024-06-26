﻿using System.Data.Common;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using ToolSC.Models;

namespace ToolSC.Helpers
{
    public static class CommonHelpers
    {
        public static int CountByteLength(string input)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("shift-jis");
            return encoding.GetByteCount(input);

            //// Chuyển chuỗi sang mảng byte sử dụng UTF-8 encoding
            //byte[] utf8Bytes = Encoding.UTF8.GetBytes(input);
            //return utf8Bytes.Length;
        }

        public static List<TableColumn> GetNameAndTypeOfColumn(string input)
        {
            var data = new List<TableColumn>();
            var sysVars = GetSystemVariables();

            // Biểu thức chính quy
            string pattern = @"<(.*?),\s*(varchar|int|bigint|datetime2|float|decimal)(?:\((\d+)\))?,>";

            // Sử dụng Regex.Matches để tìm kiếm tất cả các kết quả
            MatchCollection matches = Regex.Matches(input, pattern);

            // Kiểm tra xem có kết quả nào không
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (sysVars.Contains(match.Groups[1].Value))
                    {
                        continue;
                    }

                    data.Add(new TableColumn
                    {
                        Name = match.Groups[1].Value,
                        Type = match.Groups[2].Value,
                        Length = match.Groups[3].Value
                    });
                }

                data.AddRange(GenSystemColumn());
            }

            return data;
        }

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            Random random = new();
            return random.Next(minValue, maxValue + 1);
        }

        public static double GenerateRandomDoubleNumber(double minValue, double maxValue)
        {
            Random random = new();
            return Math.Round(minValue + (random.NextDouble() * (maxValue - minValue)), 2);
        }

        public static string GenerateRandomString(int length, bool includeHalfWidth = false)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            if (includeHalfWidth)
            {
                chars += "｡｢｣､･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ";
            }
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
                randomNumber = column.Type == "float" || column.Type == "decimal" ? GenerateRandomDoubleNumber(10, 100).ToString() : GenerateRandomNumber(10, 100).ToString();
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

            int counter = 0;
            do
            {
                if (counter == 10)
                {
                    return GenerateColumnDataFromRandomString(column, existList);
                }

                int endNumSubstr = GenerateRandomNumber(1, column.Name.Length);
                data = column.Name.Substring(0, endNumSubstr);
                counter++;
            } while (existList.Contains(data) || (CountByteLength(data) > int.Parse(column.Length)));

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
                if (column.Name.Contains(Const.DATE_CHAR) && (column.Length == Const.DATE_VAR_LENGTH_8 || column.Length == Const.DATE_VAR_LENGTH_4))
                {
                    data = DateTimeHelpers.GenerateRandomDate(column.Length);
                }
                else if (column.Name.Contains(Const.TIME_CHAR) && column.Length == Const.TIME_VAR_LENGTH)
                {
                    data = DateTimeHelpers.GenerateRandomTime();
                }
                else
                {
                    var range = GetRangeOfLengthForRandomString(int.Parse(column.Length));
                    int length = GenerateRandomNumber(range.Item1, range.Item2);
                    data = GenerateRandomString(length);
                }                
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

        public static string ConvertDataToColumn(List<string> stringList)
        {
            var wrappedItems = stringList.Select(item => $"{item}\n");

            return string.Join("", wrappedItems);
        }

        public static List<string> GetSystemVariables()
        {
            return new List<string>
            {
                "SYS作成日時",
                "SYS作成ログインアカウント名",
                "SYS作成サーバー名",
                "SYS作成機能ID",
                "SYS作成ルートジョブ実行ID",
                "SYS作成ジョブID",
                "SYS最終更新日時",
                "SYS最終更新ログインアカウント名",
                "SYS最終更新サーバー名",
                "SYS最終更新機能ID",
                "SYS最終更新ルートジョブ実行ID",
                "SYS最終更新ジョブID"
            };
        }

        public static List<TableColumn> GenSystemColumn(string kinoId = "", string sysName = "")
        {
            var dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            kinoId = !string.IsNullOrEmpty(kinoId) ? kinoId : "xxxxxxxxxx";
            sysName = !string.IsNullOrEmpty(sysName) ? sysName : "My-PC";
            string jobName = "CxcWmsBatch";

            return new List<TableColumn>
            {
                new() { Name = "SYS作成日時", Type = "datetime2", Length = "7", Data = dateNow },
                new() { Name = "SYS作成ログインアカウント名", Type = "varchar", Length = "50", Data = jobName },
                new() { Name = "SYS作成サーバー名", Type = "varchar", Length = "20", Data = sysName },
                new() { Name = "SYS作成機能ID", Type = "varchar", Length = "15", Data = kinoId },
                new() { Name = "SYS作成ルートジョブ実行ID", Type = "varchar", Length = "50", Data = kinoId },
                new() { Name = "SYS作成ジョブID", Type = "varchar", Length = "50", Data = kinoId },
                new() { Name = "SYS最終更新日時", Type = "datetime2", Length = "7", Data = dateNow },
                new() { Name = "SYS最終更新ログインアカウント名", Type = "varchar", Length = "50", Data = jobName },
                new() { Name = "SYS最終更新サーバー名", Type = "varchar", Length = "20", Data = sysName },
                new() { Name = "SYS最終更新機能ID", Type = "varchar", Length = "15", Data = kinoId },
                new() { Name = "SYS最終更新ルートジョブ実行ID", Type = "varchar", Length = "50", Data = kinoId },
                new() { Name = "SYS最終更新ジョブID", Type = "varchar", Length = "50", Data = kinoId },
            };
        }

        public static List<string> GenSystemData(string kinoId = "", string sysName = "")
        {
            var dateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            kinoId = !string.IsNullOrEmpty(kinoId) ? kinoId : "xxxxxxxxxx";
            sysName = !string.IsNullOrEmpty(sysName) ? sysName : "My-PC";
            string jobName = "CxcWmsBatch";

            return new List<string>
            {
                dateNow,
                jobName,
                sysName,
                kinoId,
                kinoId,
                kinoId,
                dateNow,
                jobName,
                sysName,
                kinoId,
                kinoId,
                kinoId
            };
        }

        public static string GenColumnKeyData(int length, int index, bool fullLength = false)
        {
            string data = "";
            string indexStr = $"0000000000000000000{index}";
            length = fullLength ? length : (length >= 8 ? 6 : length);

            if (length >= 5)
            {
                int startIndex = indexStr.Length - length + 3;
                string rightSubstring = indexStr.Substring(startIndex);
                data += "key" + rightSubstring;
            }
            else
            {
                int startIndex = indexStr.Length - length;
                data = indexStr.Substring(startIndex);
            }

            return data;
        }
    }
}
