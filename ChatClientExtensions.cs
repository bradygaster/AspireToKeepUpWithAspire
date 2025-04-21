using Microsoft.Extensions.AI;

namespace AspireToKeepUpWithAspire;

internal static class ChatClientExtensions
{
    internal static async Task<string> GetPullRequestSummary(this IChatClient chatClient, string prompt, string context)
    {
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
