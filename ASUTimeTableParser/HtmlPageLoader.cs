using HtmlAgilityPack;
using System.Xml.XPath;

namespace ASUTimeTableParser
{
    public class HtmlPageLoader
    {
        public readonly HttpClient httpClient;

        public HtmlPageLoader(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetHtml(HttpRequestMessage request, string? xpath = null)
        {
            var response = await httpClient.SendAsync(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException(response.ReasonPhrase + "\n" + "Необработанный статус код.");
            }
            var body = await response.Content.ReadAsStringAsync();

            string result = body;

            if (xpath != null)
            {
                HtmlDocument document = new();
                document.LoadHtml(body);

                var node = document.DocumentNode.SelectSingleNode(xpath);
                if(node == null)
                {
                    throw new XPathException(xpath + "\n" + "Не удалось найти запрашиваемый путь.");
                }
                result = node.OuterHtml;
            }

            return result;
        }
    }
}
