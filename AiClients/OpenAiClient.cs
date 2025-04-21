using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

namespace AspireToKeepUpWithAspire;

public class OpenAiClient : IAiClient
{
    public async Task<string> GetPullRequestSummaryAsync(string prompt, string context)
    {
        var openAiClient = new OpenAIClient(
            new ApiKeyCredential(Environment.GetEnvironmentVariable("OPEN_AI_KEY") ?? throw new InvalidOperationException("Missing configuration: OPEN_AI_KEY."))
            );

        var chatClient = openAiClient.AsChatClient(Environment.GetEnvironmentVariable("MODEL_NAME") ?? throw new InvalidOperationException("Missing configuration: MODEL_NAME."));

        return await chatClient.GetPullRequestSummary(prompt, context);
    }
}
