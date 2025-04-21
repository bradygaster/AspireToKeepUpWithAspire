namespace AspireToKeepUpWithAspire;

public class StubAiClient : IAiClient
{
    public Task<string> GetPullRequestSummaryAsync(string prompt)
    {
        return Task.FromResult("# .NET Aspire Updates: April 1, 2025 – April 20, 2025\n\nHere’s a roundup of improvements made to .NET Aspire during this period. Each change enhances developer experience, expands platform support, or improves stability.\n\n## 🐫 Docker / Container Tooling\n\n- **[Add properties to DockerComposeEnvironmentResource](https://github.com/dotnet/aspire/pull/8882)**  \n  Adds support for `depends_on`, container name, and network in Docker Compose environments.  \n  **Related issue(s):** [#8845](https://github.com/dotnet/aspire/issues/8845)\n");
    }
}
