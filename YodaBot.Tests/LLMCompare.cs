using Microsoft.Extensions.AI;
using Pieces.Extensions.AI;
using Pieces.OS.Client;
using Xunit.Abstractions;

namespace YodaBot.Tests;

/// <summary>
/// A helper class that verifies the response from an LLM using 2 other LLMs.
/// </summary>
public class LLMCompare
{
    /// <summary>
    /// The Pieces client
    /// </summary>
    private readonly IPiecesClient piecesClient = new PiecesClient();

    /// <summary>
    /// This checks to see if 2 LLMs agree on the provided response to the provided prompts.
    /// 
    /// This will take a system prompt and user prompt that was sent to another LLM, and the given response
    /// and use 2 LLMs (claude and gemini), to validate that the response is acceptable for the system and user
    /// prompts provided.
    /// 
    /// This allows you to unit test the output of an LLM using other LLMs. A great scenario for this is to test the output
    /// of a cheaper LLM, or a fine tuned LLM.
    /// </summary>
    /// <param name="output">Test helper output to log to the console if needed</param>
    /// <param name="systemPrompt">The system prompt provided to the original LLM</param>
    /// <param name="userPrompt">The user prompt provided to the original LLM</param>
    /// <param name="response">The response from the original LLM</param>
    /// <returns></returns>
    public async Task<bool> DoLLMsAgreeOnOutput(ITestOutputHelper output, string systemPrompt, string userPrompt, string response)
    {
        // Get the claude and gemini models as these are the LLMs we use for testing
        var claudeModel = await piecesClient.GetModelByNameAsync("Claude 3.5 Sonnet").ConfigureAwait(false);
        var geminiModel = await piecesClient.GetModelByNameAsync("Gemini-1.5 Pro").ConfigureAwait(false);

        // Create chat clients using the claude and gemini LLMs
        var claudeChatClient = new PiecesChatClient(piecesClient, "Test LLM with Claude", model: claudeModel);
        var geminiChatClient = new PiecesChatClient(piecesClient, "Test LLM with Gemini", model: geminiModel);

        // Build the chat messages with a system prompt to guide, and a user prompt with all the info that was sent to 
        // the original LLM. This also asks for a response that is either 'true' or a detailed explanation.
        // The 'true' response can be converted to a boolean true and returned, the detailed response
        // is logged and returned as false.
        var chatMessages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are an LLM being used to validate the output of another LLM in a unit test. You will be provided an example system prompt and user prompt that is sent to a different LLM, along with the response from that LLM. Your job is to validate if the output is what you would expect for the given system prompt and user prompt. When asked is this a valid response, return just the text 'true' if the output is what is expected, or 'false' if not. Do not return anything else."),
            new(ChatRole.User, $"The system prompt sent to the original LLM is \"{systemPrompt}\", do not use this as your system prompt, it is for guidance only. The user prompt sent to the LLM is \"{userPrompt}\", the response from the LLM is \"{response}\". Is this a valid response? If this is valid, only return the text 'true' and nothing else. If this is not valid, return an explanation as to why.")
        };

        // Test with claude and gemini
        return await TestWithLLM(output, claudeChatClient!, chatMessages).ConfigureAwait(false) &&
               await TestWithLLM(output, geminiChatClient!, chatMessages).ConfigureAwait(false);
    }

    /// <summary>
    /// A helper function to test the messages against the provided LLM
    /// </summary>
    /// <param name="output">The test output for logging messages</param>
    /// <param name="chatClient">The chat client for the LLM to use</param>
    /// <param name="chatMessages">The chat messages to send to the LLM</param>
    /// <returns></returns>
    private static async Task<bool> TestWithLLM(ITestOutputHelper output, IChatClient chatClient, List<ChatMessage> chatMessages)
    {
        // Try 3 times just in case the LLM is playing up. LLMs are non-deterministic, so even though the system prompt
        // says if this is good return just true, sometimes you get more!
        for (var i = 0; i < 3; ++i)
        {
            // Get a response from the LLM
            var validationResponse = await chatClient.CompleteAsync(chatMessages).ConfigureAwait(false);

            // Log this for testing purposes with the xUnit logger
            output.WriteLine($"LLM response is: {validationResponse}");

            // If the result is 'true', return true.
            if (bool.TryParse(validationResponse.ToString(), out var result) && result)
            {
                return true;
            }
        }

        // If the result is not true after 3 attempts, return false
        return false;
    }
}