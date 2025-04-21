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

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, prompt),
            new ChatMessage(ChatRole.User, context)
        };

        CancellationTokenSource? currentResponseCancellation = new();
        ChatOptions chatOptions = new();
        var responseText = new TextContent("");
        var responseMessage = new ChatMessage(ChatRole.Assistant, [responseText]);

        await foreach (var chunk in chatClient.GetStreamingResponseAsync(messages, chatOptions, currentResponseCancellation.Token))
        {
            responseText.Text += chunk.Text;
        }

        return responseText.Text;
    }
}
