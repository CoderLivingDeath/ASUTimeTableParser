using ASUTimeTableParser;
using HtmlAgilityPack;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ASUTimeTableParserConsole
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            HttpClient client = new();
            HtmlPageLoader htmlLoader = new(client);
            ASUParser parser = new(htmlLoader);

            string category = "15";
            string gropuId = "2129441289";
            string dateRange = "20240923-20240929";

            var xcsid = await parser.ParseXCSIDAsync(category, gropuId, dateRange);
            var scheduleTableHtml = await parser.ParseScheduleTableHtmlAsync(category, gropuId, xcsid, dateRange);
            var table = parser.ParseASUScheduleTableClassFromHtml(scheduleTableHtml);

            foreach (var row in table)
            {
                Console.WriteLine("|---------------|");
                Console.Write("Номер: ");
                Console.WriteLine(row.Number);
                Console.Write("Дата: ");
                Console.WriteLine(row.Date);
                Console.Write("Время: ");
                Console.WriteLine(row.Time);
                Console.Write("Лектор: ");
                Console.WriteLine(row.Lecturer);
                Console.Write("Предмет: ");
                Console.WriteLine(row.Object);
                Console.Write("Аудитория: ");
                Console.WriteLine(row.Audience);
                Console.WriteLine("|---------------|");
                Console.WriteLine();
            }
        }
    }
}
