using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace VacationPlanner;

public class SerperSearchTool(HttpClient http, IConfiguration config)
{
    [Description("Searches the web (Google) for up-to-date information such as attractions, local food and hotel prices.")]
    public async Task<string> SearchWebAsync(
        [Description("What to search for, e.g. 'average hotel price per night Lisbon'")] string query)
    {
        Console.WriteLine($"🔎 Agent is searching: {query}");

        var apiKey = config["Serper:ApiKey"] ?? throw new ArgumentNullException("Serper:ApiKey is not set in configuration.");

        // 1. Build the request Serper expects
        var requestUrl = $"https://google.serper.dev/search";
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
        request.Headers.Add("X-API-KEY", apiKey);
        request.Content = JsonContent.Create(new { q = query, num = 5 });

        // 2. Send it
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // 3. Keep only what's useful: title, snippet and link of each result
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        var sb = new StringBuilder();
        if (json.RootElement.TryGetProperty("organic", out var results))
        {
            foreach (var r in results.EnumerateArray())
            {
                var title = r.GetProperty("title").GetString();
                var snippet = r.TryGetProperty("snippet", out var s) ? s.GetString() : "";
                var link = r.GetProperty("link").GetString();
                sb.AppendLine($"- {title}: {snippet} ({link})");
            }
        }

        return sb.Length > 0 ? sb.ToString() : "No results found.";
    }
}
