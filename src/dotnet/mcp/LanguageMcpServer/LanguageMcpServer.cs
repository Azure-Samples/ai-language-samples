// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Azure.AI.Language.MCP.Server.Language.Tools;
using Azure.AI.Language.MCP.Server.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Azure.AI.Language.MCP.Server
{
    /// <summary>
    /// Entry point and config for the Language MCP Server.
    /// Initializes and configures services and tools for language processing, such as PII redaction and translation.
    /// </summary>
    public class LanguageMcpServer
    {
        private static readonly ISet<string> SupportedToolTypes = new HashSet<string>()
        {
            nameof(PiiRedactionTool),
            nameof(ExtractEntitiesTool),
            nameof(LanguageDetectionTool),
            nameof(SummarizationTool),
            nameof(ConversationalUnderstandingTool),
            nameof(QuestionAnsweringTool),
            nameof(ExtractHeathcareEntitiesTool),
            nameof(SentimentAnalysisTool),
            nameof(ExtractKeyPhraseTool),
        };

        private static readonly IDictionary<string, Type?> ToolNameToTypeMap = SupportedToolTypes
            .ToDictionary(
                toolName => toolName,
                toolName => Type.GetType($"Azure.AI.Language.MCP.Server.Language.Tools.{toolName}"));

        /// <summary>
        /// Main entry point for the Language MCP Server application.
        /// Configures logging, parses tool arguments, initializes settings, and registers required services and tools.
        /// </summary>
        /// <param name="args">
        /// Command-line arguments. The first argument can specify which tool to enable: "PiiRedactionTool", "TranslatorTool", or both by default.
        /// </param>
        public static async Task Main(string[] args)
        {
            // Get the tools requested from the command line arguments
            ISet<string> tools = ParseRequestedTools(args);

            HostApplicationBuilder builder = GetBuilder(args);

            builder.ConfigureLogging();

            LanguageMcpServer server = new LanguageMcpServer();

            // Configure the MCP server with the requested tools.
            server.ConfigureLanguageMcpServer(builder, tools);

            // start server.
            await builder.Build().RunAsync();
        }

        /// <summary>
        /// Configures the Language MCP Server with the specified tools and registers them with the MCP server builder.
        /// Initializes required clients for each tool, dynamically registers tool types, and adds them to the server.
        /// </summary>
        /// <param name="builder">The <see cref="HostApplicationBuilder"/> used to configure services and the application host.</param>
        /// <param name="tools">A set of tool names to enable and register with the server.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if a specified tool is not supported or its type cannot be found.
        /// </exception>
        public void ConfigureLanguageMcpServer(
            HostApplicationBuilder builder,
            ISet<string> tools)
        {
            IMcpServerBuilder mcpServerBuilder = builder.Services
                .AddMcpServer()
                .WithStdioServerTransport();

            MethodInfo? withToolsMethod = typeof(McpServerBuilderExtensions)
                            .GetMethods()
                            .FirstOrDefault(m => m.Name == "WithTools" && m.IsGenericMethod);

            foreach (string toolName in tools)
            {
                if (!SupportedToolTypes.Contains(toolName))
                {
                    throw new ArgumentException($"Tool '{toolName}' is not supported. Supported tools are: {string.Join(", ", SupportedToolTypes)}");
                }

                if (ToolNameToTypeMap.TryGetValue(toolName, out Type? toolType) && toolType != null)
                {
                    MethodInfo genericMethod = withToolsMethod?.MakeGenericMethod(toolType);
                    genericMethod?
                        .Invoke(
                            mcpServerBuilder,
                            [mcpServerBuilder, new JsonSerializerOptions() { TypeInfoResolver = new DefaultJsonTypeInfoResolver() }]);
                }
                else
                {
                    throw new ArgumentException($"Tool type for '{toolName}' could not be found.");
                }
            }

            builder.ConfigureClientsForTools(tools);
        }

        private static ISet<string> ParseRequestedTools(string[] args)
        {
            // Parse tools requested argument
            ISet<string> tools;
            if (args.Length == 0 || string.Equals(args[0], "All", StringComparison.InvariantCultureIgnoreCase))
            {
                tools = SupportedToolTypes;
            }
            else
            {
                string[]? toolNames = args[0]?.Trim().Split(",");
                if (toolNames == null || toolNames.Length == 0)
                {
                    throw new ArgumentException("No tools specified. Please provide a comma-separated list of tools to enable.");
                }

                tools = toolNames.Select(tool => tool.Trim()).ToHashSet();
            }

            return tools;
        }

        private static HostApplicationBuilder GetBuilder(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            return builder;
        }
    }
}