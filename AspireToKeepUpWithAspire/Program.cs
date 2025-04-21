using Markdig;
using System.Text;

namespace AspirePrSummary
{
    public interface IAiClient
    {
        Task<string> GetPullRequestSummaryAsync(string prompt);
    }

    public class StubAiClient : IAiClient
    {
        public Task<string> GetPullRequestSummaryAsync(string prompt)
        {
            return Task.FromResult("# .NET Aspire Updates: April 1, 2025 – April 20, 2025\n\nHere’s a roundup of improvements made to .NET Aspire during this period. Each change enhances developer experience, expands platform support, or improves stability.\n\n## 🐫 Docker / Container Tooling\n\n- **[Add properties to DockerComposeEnvironmentResource](https://github.com/dotnet/aspire/pull/8882)**  \n  Adds support for `depends_on`, container name, and network in Docker Compose environments.  \n  **Related issue(s):** [#8845](https://github.com/dotnet/aspire/issues/8845)\n");
        }
    }

    class Program
    {
        private const string Prompt = @"Please review all pull requests that have been merged into the https://github.com/dotnet/aspire repository from the first day of the current month until today.

At the top of the output, include a markdown-formatted title using this format:  
`# .NET Aspire Updates: {Month Day, Year} – {Month Day, Year}`  
Follow this with a short introductory paragraph like:  
`Here’s a roundup of improvements made to .NET Aspire during this period. Each change enhances developer experience, expands platform support, or improves stability.`

Then, summarize each pull request with a one-sentence description that focuses on how it improves .NET Aspire for customers using it to build applications. Include a link to the pull request.

Group the summaries by high-level SDK area or component (e.g., Dashboard, Kubernetes, Docker, CLI, Hosting, Testing). Use an appropriate emoji to label each section heading.

After each summary, include a line labeled “Related issue(s):” with links to any related GitHub issues. If there are no associated issues, write “None.”

The final output must be valid markdown-formatted text, suitable for saving to a `.md` file or displaying in markdown viewers. Use proper markdown headings, bolded PR titles, consistent indentation, and spacing for readability.

This summary is intended to fit on a single webpage and should be readable at a glance.";

        static async Task Main()
        {
            string endpoint = "https://YOUR-RESOURCE-NAME.openai.azure.com/";
            string apiKey = "YOUR_API_KEY";
            string deploymentName = "gpt-4-azure";

            var aiClient = new StubAiClient();
            var markdown = await aiClient.GetPullRequestSummaryAsync(Prompt);

            string outputDir = Directory.GetCurrentDirectory();
            string markdownPath = Path.Combine(outputDir, "Aspire-PR-Summary.md");
            string htmlPath = Path.Combine(outputDir, "Aspire-PR-Summary.html");

            await File.WriteAllTextAsync(markdownPath, markdown, new UTF8Encoding(false));

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var htmlBody = Markdown.ToHtml(markdown, pipeline);

            string htmlTemplate = File.ReadAllText("template.html");
            string fullHtml = htmlTemplate.Replace("{htmlBody}", htmlBody);

            await File.WriteAllTextAsync(htmlPath, fullHtml, Encoding.UTF8);
            Console.WriteLine($"Markdown summary saved to: {markdownPath}");
            Console.WriteLine($"HTML summary saved to: {htmlPath}");
        }
    }
}