// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Azure.AI.Language.MCP.Server.Translator.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Azure.AI.Language.MCP.Server.Translator
{
    /// <summary>
    /// Entry point and config for the Language MCP Server.
    /// Initializes and configures services and tools for language processing, such as PII redaction and translation.
    /// </summary>
    public class TranslatorMcpServer
    {
        private static readonly ISet<string> SupportedToolTypes = new HashSet<string>()
        {
            nameof(TranslatorTool),
        };

        private static readonly IDictionary<string, Type?> ToolNameToTypeMap = SupportedToolTypes
            .ToDictionary(
                toolName => toolName,
                toolName => Type.GetType($"Azure.AI.Language.MCP.Server.Translator.Tools.{toolName}"));

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

            var server = new TranslatorMcpServer();

            // Configure the MCP server with the requested tools.
            server.ConfigureTranslatorMcpServer(builder, tools);

            // start server.
            await builder.Build().RunAsync();
        }

        /// <summary>
        /// Configures the Translator MCP Server with the specified tools and registers them with the MCP server builder.
        /// </summary>
        /// <param name="builder">The <see cref="HostApplicationBuilder"/> used to configure services and the application host.</param>
        public void ConfigureTranslatorMcpServer(
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

                if (ToolNameToTypeMap.TryGetValue(toolName, out var toolType) && toolType != null)
                {
                    MethodInfo genericMethod = withToolsMethod.MakeGenericMethod(toolType);
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
                var toolNames = args[0]?.Trim().Split(",");
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