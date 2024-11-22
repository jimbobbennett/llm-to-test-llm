using Xunit.Abstractions;

namespace YodaBot.Tests;

public class YodaBotTests(ITestOutputHelper output)
{
    private const string V = @"Turn the LEGO® Star Wars™ universe upside down with The Dark Falcon buildable toy vehicle set for kids (75389), which features a dark version of the Millennium Falcon, as seen in the LEGO Star Wars: Rebuild the Galaxy Disney+ special. A thrilling birthday gift for boys, girls and any fans aged 10 and up, this LEGO Star Wars brick-built starship toy has flip-up panels for easy access to the detailed interior, including Darth Jar Jar’s throne, the command center, hyperdrive, entertainment area and jail cell. Spring-loaded shooters, rotating cannons and a removable gunner post add to the action-play possibilities.

        This buildable starship playset also includes 6 LEGO Star Wars minifigures as you’ve never seen them before, including Darth Jar Jar, Beach Luke and Jedi Vader.

        LEGO® Star Wars™ buildable toy vehicle set for kids – Build The Dark Falcon, a dark version of the Millennium Falcon as seen in the Rebuild the Galaxy Disney+ special, to turn the universe upside down
        6 LEGO® Star Wars™ minifigures – Darth Jar Jar, Bounty Hunter C-3PO, Darth Dev, Darth Rey, Beach Luke and Jedi Vader, plus accessories including lightsabers and a blue milk carton
        LEGO® brick Dark Falcon – Features a removable cockpit for 2 LEGO minifigures, 2 spring-loaded shooters, 2 rotating cannons, a gunner post for 2 LEGO minifigures and a radar dish
        Easy access – Flip up the top panels to play with the detailed interior, which includes a removable throne for Darth Jar Jar, command center, hyperdrive, bunk beds, entertainment area and a jail cell
        Fun gift for kids aged 10 and up – Give this LEGO® brick-built starship playset as a holiday or birthday gift to boys, girls and any Star Wars™ fan
        Collectible building toys for all ages – LEGO® Star Wars™ sets enable kids and adult Star Wars fans to recreate classic scenes, create their own adventures or simply display the buildable models
        Measurements – The buildable Star Wars™ starship toy in this 1,579-piece set measures over 5 in. (12 cm) high, 17 in. (43 cm) long and 12.5 in. (32 cm) wide";

    private readonly YodaBotClient yodaBotClient= new();
    private readonly LLMCompare llmCompare = new();
    private readonly ITestOutputHelper output = output;

    [Fact]
    public async Task TestASummarizedDescriptionFromTheLLMMatchesWhatIsExpected()
    {
        var legoDarkFalconProductDetails = V;

        string question = $"Summarize this product description: {legoDarkFalconProductDetails}";
        var completion = await yodaBotClient.CompleteAsync(question);
        var result = await llmCompare.DoLLMsAgreeOnOutput(output, yodaBotClient.SystemPrompt, question, completion.ToString());

        Assert.True(result);
    }

    [Fact]
    public async Task TestAFakeDescriptionDoesntMatchWhatIsExpected()
    {
        var legoDarkFalconProductDetails = V;

        string question = $"Summarize this product description: {legoDarkFalconProductDetails}";
        var result = await llmCompare.DoLLMsAgreeOnOutput(output, yodaBotClient.SystemPrompt, question, "This is a made up response");

        Assert.False(result);
    }
}