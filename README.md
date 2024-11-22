# LLMs to test LLMs!

This repo contains a demo that shows how to use an LLM to test LLMs.

## Why use an LLM to test LLMs?

LLMs are non-deterministic. If you ask "How can I create a hello world app in C#?" multiple times, the answer will vary every time:

```output
You can create a hello world app in C# with the following code:

Console.WriteLine("Hello world");
```

```output
To create a hello world app in C#, use the following:

class Program
{
    public static void Main()
    {
        Console.WriteLine("Hello world");
    }
}
```

And so on.

So how can you test this? When you write a unit test, you are assuming that the code you are calling is deterministic - I pass in these parameters, and will always get this result. LLMs just don't work like this.

## So how can I test an LLM?

You could manually check the results - run the test and read the output yourself. But this is not ideal. A better option is to use AI. You can ask the LLM to act like the human in the loop and verify the results.

Now it doesn't make sense to use the same LLM that your code is using to verify the results - if you are using an LLM that is producing bad results (for example, a LLM that is the wrong choice for your problem, or one you are fine tuning and have made errors with the fine tuning), then it may well make mistakes testing its own output.

It's better to use other LLMs to test your LLM. A good example might be:

- You want to test if a cheaper LLM (e.g. GPT-3.5) is good enough for your use case
- You then use a more expensive, and theoretically better LLM (e.g. GPT-4o) to test the output.

## What is in this repo

This repo contains a C# example, using the [Pieces for Developers](https://pieces.app)[C# SDK](https://github.com/pieces-app/pieces-os-client-sdk-for-csharp) as an abstraction layer for selecting different models.

There are 2 projects here:

- [YodaBot](./YodaBot/) - a library containing an example class, `YodaBotClient`, that provides LLM access to answer questions in the style of Yoda from Star Wars. This uses GPT-4o.
- [YodaBot.Tests](./YodaBot.Tests/) - an xUnit project to unit test the `YodaBot` using Claude 3.5 Sonnet and Gemini 1.5 Pro.

The tests project contains 2 tests:

- A test that send the description of the [Lego Dark Falcon](https://www.lego.com/en-us/product/the-dark-falcon-75389) to the YodaBot to ask for a summary in the style of Yoda. It then validates the response with Claude and Gemini
- A test that validates a made up response against the same prompts to validate that Claude and Gemini report that this is not valid

## Run the code

To run this code, navigate to the [YodaBot.Tests](./YodaBot.Tests/) folder and run:

```bash
dotnet test --logger "console;verbosity=detailed"
```
