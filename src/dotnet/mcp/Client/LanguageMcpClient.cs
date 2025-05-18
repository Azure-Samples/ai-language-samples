// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Globalization;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using OpenAI.Chat;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

/// <summary>
/// The LanguageMcpClient class provides a console application that connects to a Model Context Protocol (MCP) server,
/// lists available language tools, and enables interactive chat with Azure OpenAI using those tools.
/// It loads configuration from `appsettings.json` files, initializes the MCP client and Azure OpenAI client, and
/// streams chat responses to the user, supporting function invocation and OpenTelemetry tracing.
/// </summary>

namespace Azure.AI.Language.MCP.Client
{
    public class LanguageMcpClient
    {
        public static async Task Main(string[] args)
        {
            // initialize settings
            IConfigurationRoot config = LoadConfiguration();

            // Define the transport for the client. This is where the MCP server will be running.
            var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
            {
                Name = "language-mcp-server",
                Command = "dotnet",
                Arguments = ["run", "--project", "../../../../Server", config["Tools"]]
            });

            // Configure your Azure OpenAI endpoint and deployment name here.
            using IChatClient chatClient = await InitializeChatClient(config);

            // Create a ModelContextProtocol client that will connect to the MCP server.
            IMcpClient client = await McpClientFactory.CreateAsync(clientTransport);

            // List all the tools available from the MCP server.
            IList<McpClientTool> tools = await ListMcpTools(client);

            // Start the chat with the user.
            await StartChatAsync(chatClient, tools);
        }

        internal static async Task StartChatAsync(IChatClient chatClient, IList<McpClientTool> tools)
        {
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

        internal static async Task<IList<McpClientTool>> ListMcpTools(IMcpClient client)
        {
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

            return tools;
        }

        internal static async Task<IChatClient> InitializeChatClient(IConfigurationRoot config)
        {
            if (!Uri.TryCreate(config["OpenAISettings:Endpoint"], UriKind.Absolute, out var endpoint))
            {
                throw new ArgumentException($"Invalid OpenAI endpoint. {config["OpenAISettings:Endpoint"]}. Please update the credentials in the appSettings.");
            }

            var deploymentName = config["OpenAISettings:DeploymentName"];
            var apiKey = config["OpenAISettings:ApiKey"];

            // Create an Azure OpenAI client.
            AzureOpenAIClient openAiClient = new(endpoint, new AzureKeyCredential(apiKey));
            ChatClient azureChatClient = openAiClient.GetChatClient(deploymentName);

            // Create an IChatClient that can use the tools.
            IChatClient chatClient = azureChatClient.AsIChatClient()
                .AsBuilder()
                .UseFunctionInvocation()
                    .UseOpenTelemetry(configure: o => o.EnableSensitiveData = true)
                    .Build();

            // test the openAI client.
            await chatClient.GetResponseAsync(new ChatMessage(ChatRole.User, "Hello! How are you?"));

            return chatClient;
        }

        internal static void PromptForCustomerInput()
        {
            Console.WriteLine();
            Console.Write("Q: ");
        }

        internal static IConfigurationRoot LoadConfiguration()
        {
            return new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
             .Build();
        }
    }
}