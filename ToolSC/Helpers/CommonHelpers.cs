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
    }
}
