using System.Text;
using System.Text.Json;

namespace DevToArticlesList;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var apiKey = GetArgumentValue(args, "--api-key");
        var templateFilePath = GetArgumentValue(args, "--template-file");
        var outputFilePath = GetArgumentValue(args, "--out-file");
        var limit = int.TryParse(GetArgumentValue(args, "--limit"), out var parsedLimit) ? parsedLimit : 5;

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(templateFilePath) || string.IsNullOrEmpty(outputFilePath))
        {
            Console.WriteLine("Missing required arguments: --username, --template-file, --out-file");
            return;
        }

        await ScrapeDevToLatestArticles(apiKey, templateFilePath, outputFilePath, limit);
    }

    private static string? GetArgumentValue(string[] args, string key)
    {
        return (from arg in args where arg.StartsWith(key) select arg.Split('=')[1].Trim('\'')).FirstOrDefault();
    }

    private static async Task ScrapeDevToLatestArticles(string apiKey, string? templateFilePath, string? outputFilePath, int limit)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");
        client.DefaultRequestHeaders.Add("api_key", apiKey);
        var response = await client.GetAsync($"https://dev.to/api/articles/me?page=1&&per_page={limit}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var articles = JsonSerializer.Deserialize<List<Article>>(content);
            var articlesTableContent = new StringBuilder();
            if (articles != null)
            {
                foreach (var article in articles)
                {
                    articlesTableContent.AppendLine("<tr>");
                    articlesTableContent.AppendLine(
                        $"<td width=\"300px\"><a href=\"{article.Url}\"><img src=\"{article.CoverImage}\" alt=\"thumbnail\"></a></td>");
                    articlesTableContent.AppendLine(
                        $"<td><a href=\"{article.Url}\">{article.Title}</a><hr><p>{article.Description}</p><br><i>Published by: {article.User.Name}</i><br><b>{article.PublishedAt}</b></td>");
                    articlesTableContent.AppendLine("</tr>");
                    
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
