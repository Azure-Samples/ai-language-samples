// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using OpenAI.Chat;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

internal class Program
{
    public static async Task Main(string[] args)
    {
        // initialize settings
        IConfigurationRoot config = InitializeConfiguration();

        // Define the transport for the client. This is where the MCP server will be running.
        var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "language-tools",
            Command = "dotnet",
            Arguments = ["run", "--project", "../../../../McpServer/Tools/LanguageTools", config["Tools"]]
        });

        // Configure your Azure OpenAI endpoint and deployment name here.
        var endpoint = new Uri(config["OpenAISettings:Endpoint"]);
        var deploymentName = config["OpenAISettings:DeploymentName"];
        var apiKey = config["OpenAISettings:ApiKey"];

        // Create an Azure OpenAI client.
        AzureOpenAIClient openAiClient = new(endpoint, new AzureKeyCredential(apiKey));
        ChatClient azureChatClient = openAiClient.GetChatClient(deploymentName);

        // Create a ModelContextProtocol client that will connect to the MCP server.
        IMcpClient client = await McpClientFactory.CreateAsync(clientTransport);

        // Print the list of tools available from the server.
        IList<McpClientTool> tools = await client.ListToolsAsync();

        Console.WriteLine("Welcome to the Language MCP client!");
        Console.WriteLine("\n");

        Console.Write("List of tools connected to the client: ");
        Console.WriteLine("\n");

        foreach (var tool in tools)
        {
            Console.WriteLine($"{tool.Name}: ({tool.Description})");
            Console.WriteLine();
        }
        Console.WriteLine("--------------------------------------------------");

        // Create an IChatClient that can use the tools.
        using IChatClient chatClient = azureChatClient.AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
                .UseOpenTelemetry(configure: o => o.EnableSensitiveData = true)
                .Build();

        IList<ChatMessage> messages = [];

        Console.WriteLine("You can start asking questions now. Type `exit` to exit");
        while (Console.ReadLine() is string query && !"exit".Equals(query, StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                PromptForCustomerInput();
                continue;
            }

            messages.Add(new(ChatRole.User, query));

            List<ChatResponseUpdate> updates = [];
            await foreach (var update in chatClient.GetStreamingResponseAsync(messages, new() { Tools = [.. tools] }))
            {
                Console.Write(update);
                updates.Add(update);
            }
            PromptForCustomerInput();

            messages.AddMessages(updates);
        }
    }

    private static void PromptForCustomerInput()
    {
        Console.WriteLine();
        Console.Write("Q: ");
    }
 
    private static IConfigurationRoot InitializeConfiguration()
    {
        return new ConfigurationBuilder()
         .SetBasePath(AppContext.BaseDirectory)
         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
         .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
         .Build();
    }
}