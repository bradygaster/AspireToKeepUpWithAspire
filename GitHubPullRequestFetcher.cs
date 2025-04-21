using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AspireToKeepUpWithAspire;

public class GitHubPullRequestFetcher
{
    private readonly HttpClient _httpClient;

    public GitHubPullRequestFetcher(string githubToken)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AspirePrSummary", "1.0"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", githubToken);
    }

    public async Task<string> GetMergedPullRequestsThisMonthAsync(string owner = "dotnet", string repo = "aspire")
    {
        var firstOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var url = $"https://api.github.com/search/issues?q=repo:{owner}/{repo}+is:pr+is:merged+merged:>={firstOfMonth}&per_page=100";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var sb = new StringBuilder();

        foreach (var pr in json.RootElement.GetProperty("items").EnumerateArray())
        {
            string title = pr.GetProperty("title").GetString();
            string urlStr = pr.GetProperty("html_url").GetString();
            string body = pr.TryGetProperty("body", out var b) ? b.GetString() : "";

            sb.AppendLine($"- **[{title}]({urlStr})**");
            sb.AppendLine($"  {body?.Split('\n')[0]}");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
