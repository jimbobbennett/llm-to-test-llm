using Microsoft.Extensions.AI;
using Pieces.Extensions.AI;
using Pieces.OS.Client;

namespace YodaBot;

/// <summary>
/// A LLM client that imitates Yoda when answering a question
/// </summary>
public class YodaBotClient
{
    /// <summary>
    /// The PiecesOS client
    /// </summary>
    private readonly PiecesClient piecesClient = new();

    /// <summary>
    /// The chat client using the Pieces.Extensions.AI abstraction
    /// </summary>
    private IChatClient? chatClient;

    /// <summary>
    /// The system prompt that this chat client will use
    /// </summary>
    public string SystemPrompt {get;} = "You are a chatbot that talks in the style of Yoda from Star Wars. Be terse with your responses, and respond using Yoda's style and sentence structure";

    /// <summary>
    /// Send the question to the LLM and return the response
    /// </summary>
    /// <param name="question">The question to send</param>
    /// <returns>The response from the LLM</returns>
    public async Task<string> CompleteAsync(string question)
    {
        // If our chat client hasn't been created yet, create it
        if (chatClient is null)
        {
            // Load the GPT-4o model
            var model = await piecesClient.GetModelByNameAsync("GPT-4o chat").ConfigureAwait(false);

            // Use this model to create the chat client
            chatClient = new PiecesChatClient(piecesClient, "Chat with Yoda, you must", model: model);
        }

        // Build the chat messages using the system prompt and the question
        var chatMessages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, question)
        };

        // Send this to the LLM
        var response = await chatClient.CompleteAsync(chatMessages).ConfigureAwait(false);

        // Return the textual response
        return response.ToString();
    }
}