using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

namespace AspireToKeepUpWithAspire;

public class OpenAiClient : IAiClient
{
    public async Task<string> GetPullRequestSummaryAsync(string prompt)
    {
        var openAiClient = new OpenAIClient(
            new ApiKeyCredential(Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("Missing configuration: OPENAI_API_KEY."))
            );

        var chatClient = openAiClient.AsChatClient(Environment.GetEnvironmentVariable("MODEL_NAME") ?? throw new InvalidOperationException("Missing configuration: MODEL_NAME."));

        List<ChatMessage> messages = new();
        CancellationTokenSource? currentResponseCancellation = new();
        ChatOptions chatOptions = new();
        var message = new ChatMessage(ChatRole.System, prompt);
        messages.Add(message);
        var responseText = new TextContent("");
        var responseMessage = new ChatMessage(ChatRole.Assistant, [responseText]);

        await foreach (var chunk in chatClient.GetStreamingResponseAsync(messages, chatOptions, currentResponseCancellation.Token))
        {
            responseText.Text += chunk.Text;
        }

        return responseText.Text;
    }
}
