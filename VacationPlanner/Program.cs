using Amazon;
using Amazon.BedrockRuntime;
using Microsoft.Extensions.AI;
using VacationPlanner;

var builder = WebApplication.CreateBuilder(args);

// 1. Create a Bedrock client, then wrap it as a standard IChatClient
var modelId = builder.Configuration["Bedrock:ModelId"] ?? throw new ArgumentNullException("ModelId is not set in configuration.");
IAmazonBedrockRuntime bedrock = new AmazonBedrockRuntimeClient(RegionEndpoint.EUWest1);

// 2. Register it so any part of the app can ask for an IChatClient
builder.Services.AddChatClient(bedrock.AsIChatClient(modelId));

// 3. Register the SerperSearchTool so it can be injected into any part of the app
builder.Services.AddHttpClient<SerperSearchTool>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/ask", async (string question, IChatClient chatClient) =>
{
    List<ChatMessage> messages =
    [
        new(ChatRole.System, "You are a cheerful travel expert. Answer in 3 bullet points."),
        new(ChatRole.User, question)
    ];

    var response = await chatClient.GetResponseAsync(messages);
    return response.Text;
});

app.MapGet("/search", async (string q, SerperSearchTool search) =>
    await search.SearchWebAsync(q));

app.Run();
