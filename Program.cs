using Markdig;
using System.Text;

namespace AspireToKeepUpWithAspire;

public interface IAiClient
{
    Task<string> GetPullRequestSummaryAsync(string prompt, string context);
}

class Program
{
    static async Task Main()
    {
        string githubToken = Environment.GetEnvironmentVariable("GH_TOKEN") ?? throw new InvalidOperationException("Missing GH_TOKEN env var");

        var fetcher = new GitHubPullRequestFetcher(githubToken);
        var prContext = await fetcher.GetMergedPullRequestsThisMonthAsync("dotnet", "aspire");
        var prompt = await File.ReadAllTextAsync("prompt.txt");

        var aiClient = new OpenAiClient();
        //var aiClient = new AzureAiClient();
        var markdown = await aiClient.GetPullRequestSummaryAsync(prompt, prContext);

        string outputDir = Directory.GetCurrentDirectory();
        string markdownPath = Path.Combine(outputDir, "Aspire-PR-Summary.md");
        string htmlPath = Path.Combine(outputDir, "index.html");

        await File.WriteAllTextAsync(markdownPath, markdown, new UTF8Encoding(false));

        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        var htmlBody = Markdown.ToHtml(markdown, pipeline);

        string htmlTemplate = await File.ReadAllTextAsync("template.html");
        string fullHtml = htmlTemplate.Replace("{htmlBody}", htmlBody);

        await File.WriteAllTextAsync(htmlPath, fullHtml, Encoding.UTF8);
        Console.WriteLine($"Markdown summary saved to: {markdownPath}");
        Console.WriteLine($"HTML summary saved to: {htmlPath}");
    }
}