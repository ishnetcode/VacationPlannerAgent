using Microsoft.Extensions.AI;

namespace VacationPlanner;

public class TripPlanner (IChatClient chat)
{
    private const string Instructions = """
        You are TripPlanner, an expert trip planner.
        You will receive research notes about a destination. Using ONLY those notes,
        write a travel report in Markdown with this structure:

        # Your trip to <destination>

        A numbered list of 10 to 15 points covering:
        - Places to visit, spread sensibly across the days of the trip
        - Food to try (dish names, and where to try them if the notes say)
        - Average hotel cost per night: budget / mid-range / luxury, with currency

        ## Practical tips
        2-3 short tips.

        Rules:
        - Never invent facts that are not in the research notes.
        - Mark all prices as approximate.
        - Keep each point to 1-2 sentences.
        """;
    public async Task<string> PlanAsync(
        string destination, 
        int days, 
        string researchNotes,
        CancellationToken ct = default)
    {
        List<ChatMessage> messages =
        [
            new(ChatRole.System, Instructions),
            new(ChatRole.User, $"""
                Destination: {destination}
                Trip length: {days} days

                Research notes:
                {researchNotes}
                """)
        ];

        var response = await chat.GetResponseAsync(messages, cancellationToken: ct);
        return response.Text;
    }
}
