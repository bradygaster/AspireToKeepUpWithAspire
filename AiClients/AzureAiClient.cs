using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using System.ClientModel;

namespace AspireToKeepUpWithAspire;

public class AzureAiClient : IAiClient
{
    public async Task<string> GetPullRequestSummaryAsync(string prompt, string context)
    {
        var azureOpenAi = new AzureOpenAIClient(
            new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("Missing configuration: AZURE_OPENAI_ENDPOINT.")),
            new ApiKeyCredential(Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? throw new InvalidOperationException("Missing configuration: AZURE_OPENAI_API_KEY."))
            );

        var chatClient = azureOpenAi.AsChatClient(Environment.GetEnvironmentVariable("MODEL_NAME") ?? throw new InvalidOperationException("Missing configuration: MODEL_NAME."));

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
