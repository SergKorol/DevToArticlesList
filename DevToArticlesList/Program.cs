using System.Text;
using HtmlAgilityPack;

namespace DevToArticlesList;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var username = GetArgumentValue(args, "--username");
        var templateFilePath = GetArgumentValue(args, "--template-file");
        var outputFilePath = GetArgumentValue(args, "--out-file");
        var limit = int.TryParse(GetArgumentValue(args, "--limit"), out var parsedLimit) ? parsedLimit : 5;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(templateFilePath) || string.IsNullOrEmpty(outputFilePath))
        {
            Console.WriteLine("Missing required arguments: --username, --template-file, --out-file");
            return;
        }

        var url = $"https://dev.to/{username}";
        await ScrapeDevToLatestArticles(url, templateFilePath, outputFilePath, limit);
    }

    private static string? GetArgumentValue(string[] args, string key)
    {
        return (from arg in args where arg.StartsWith(key) select arg.Split('=')[1].Trim('\'')).FirstOrDefault();
    }

    private static async Task ScrapeDevToLatestArticles(string url, string? templateFilePath, string? outputFilePath, int limit)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");
        
        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var pageContent = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(pageContent);

            var articles = document.DocumentNode.SelectNodes("//h2[@class='crayons-story__title']/a");

            if (articles != null)
            {
                var articlesTableContent = new StringBuilder();

                var count = 0;
                foreach (var article in articles)
                {
                    if (count >= limit) break;

                    var title = article.InnerText.Trim();
                    var link = article.GetAttributeValue("href", string.Empty);
                    var preloadImg = article.GetAttributeValue("data-preload-image", string.Empty) ?? "No image";

                    articlesTableContent.AppendLine("<tr>");
                    articlesTableContent.AppendLine($"<td width=\"300px\"><a href=\"https://dev.to{link}\"><img src=\"{preloadImg}\" alt=\"thumbnail\"></a></td>");
                    articlesTableContent.AppendLine($"<td><a href=\"https://dev.to{link}\">{title}</a></td>");
                    articlesTableContent.AppendLine("</tr>");

                    count++;
                }

                if (templateFilePath != null)
                {
                    var templateContent = await File.ReadAllTextAsync(templateFilePath);

                    var updatedContent = templateContent
                        .Replace("{{ArticlesTable}}", articlesTableContent.ToString())
                        .Replace("{{UpdatedDateTime}}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

                    if (outputFilePath != null) await File.WriteAllTextAsync(outputFilePath, updatedContent);
                }

                Console.WriteLine($"Template file created at {outputFilePath}");
            }
            else
            {
                Console.WriteLine("No articles found.");
            }
        }
        else
        {
            Console.WriteLine($"Failed to fetch the page. Status Code: {response.StatusCode}");
        }
    }
}
