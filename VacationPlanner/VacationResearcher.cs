using Microsoft.Extensions.AI;

namespace VacationPlanner;

public class VacationResearcher(IChatClient chat, SerperSearchTool search)
{
    private const string Instructions = """
        You are VacationResearcher, a travel research specialist.
        Your job is to gather facts, NOT to write the final itinerary.

        For the destination you are given, use the search_web tool to find:
        1. Top attractions and places to visit
        2. Local food and dishes worth trying
        3. Average hotel cost per night (budget, mid-range, luxury)

        Rules:
        - Make 3 to 6 focused searches.
        - Only use facts from the search results. Never invent anything.
        - Return short bullet-point notes under the headings
          Attractions, Food, Hotels - with the source link for each fact.
        """;

    public async Task<string> ResearchAsync(string destination, CancellationToken ct = default)
    {
        IReadOnlyList<ChatMessage> messages =
        [
            new(ChatRole.System, Instructions),
            new(ChatRole.User, $"Research a holiday in {destination}.")
        ];

        var options = new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(search.SearchWebAsync, name: "search_web")]
        };

        var response = await chat.GetResponseAsync(messages, options, ct);

        return response.Text;
    }
}
