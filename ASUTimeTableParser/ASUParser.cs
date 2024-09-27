using ASUTimeTableParser.Data;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace ASUTimeTableParser
{
    public class ASUParser
    {
        private const string SCHEMA = "https";
        private const string HOST = "www.asu.ru";
        private const int PORT = -1;

        public readonly HtmlPageLoader PageLoader;

        public ASUParser(HtmlPageLoader pageLoader)
        {
            PageLoader = pageLoader;
        }

        public IEnumerable<ParsedASUScheduleTableClass> ParseASUScheduleTableClassFromHtml(string ScheduleTableHTML)
        {
            // results
            string date = string.Empty;
            string number = string.Empty;
            string time = string.Empty;
            string @object = string.Empty;
            string lecturer = string.Empty;
            string audience = string.Empty;

            HtmlDocument sheduleTableDocument = new HtmlDocument();
            sheduleTableDocument.LoadHtml(ScheduleTableHTML);

            var tableBodyNode = sheduleTableDocument.DocumentNode.SelectSingleNode("//div[@class='schedule_table-body']");
            var DaysNodes = tableBodyNode.SelectNodes("div[@class='schedule_table-body-rows_group']");

            foreach (var dayNode in DaysNodes)
            {
                date = Regex.Replace(dayNode.SelectSingleNode("div[1]").InnerText, @"\s+", " ").Trim();

                var dayBodyNode = dayNode.SelectSingleNode("div[2]");

                var classesNodes = dayBodyNode.SelectNodes("div[@class='schedule_table-body-row']");
                foreach (var classNode in classesNodes)
                {
                    number = Regex.Replace(classNode.SelectSingleNode("div[1]").InnerText, @"\s+", " ").Trim();
                    time = Regex.Replace(classNode.SelectSingleNode("div[2]").InnerText, @"\s+", " ").Trim();
                    @object = Regex.Replace(classNode.SelectSingleNode("div[3]").InnerText, @"\s+", " ").Trim();
                    lecturer = Regex.Replace(classNode.SelectSingleNode("div[4]").InnerText, @"\s+", " ").Trim();
                    audience = Regex.Replace(classNode.SelectSingleNode("div[5]").InnerText, @"\s+", " ").Trim();

                    yield return new ParsedASUScheduleTableClass(date, number, time, @object, lecturer, audience);
                }
            }
        }

        public async Task<string> ParseXCSIDAsync(string category, string GroupId, string? dateRange = null)
        {
            string path = $"timetable/students/{category}/{GroupId}/";
            UriBuilder builder = new UriBuilder(SCHEMA, HOST, PORT, path);

            if (dateRange != null)
            {
                builder.Query = $"date={dateRange}";
            }

            var ParseXCSIDRequest = new HttpRequestMessage(HttpMethod.Get, builder.Uri);

            ParseXCSIDRequest.Headers.Add("Referer", $"{builder.Uri}");
            ParseXCSIDRequest.Headers.Add("Cookie", "__asu__=2mdrld2atl1vb3ujojgq107acf");

            string htmlString = await PageLoader.GetHtml(ParseXCSIDRequest, "/html/body/div/div/div/div[1]/script[2]");

            HtmlDocument document = new();
            document.LoadHtml(htmlString);

            Regex regex = new Regex(@"request\.setRequestHeader\('X-CS-ID', '(?<val>.*?)'\);");
            Match match = regex.Match(document.DocumentNode.InnerHtml);

            string? key = null;
            if (match.Success)
            {
                key = match.Groups["val"].Value;
            }
            else
            {
                throw new Exception();
            }

            return key;
        }

        public async Task<string> ParseScheduleTableHtmlAsync(string category, string GroupId, string XCSID, string? dateRange = null)
        {
            string path = $"timetable/students/{category}/{GroupId}/";
            UriBuilder builder = new UriBuilder(SCHEMA, HOST, PORT, path);

            if (dateRange != null)
            {
                builder.Query = $"date={dateRange}";
            }

            var ParseTimeTableRequest = new HttpRequestMessage(HttpMethod.Get, builder.Uri);

            ParseTimeTableRequest.Headers.Add("Referer", $"{builder.Uri}");
            ParseTimeTableRequest.Headers.Add("Cookie", "__asu__=2mdrld2atl1vb3ujojgq107acf");
            ParseTimeTableRequest.Headers.Add("X-CS-ID", XCSID);

            var htmlString = await PageLoader.GetHtml(ParseTimeTableRequest, "//div[@class='l-box schedule_table' and @data-type='students']");
            return htmlString;
        }
    }
}
