# LanguageTools  

`LanguageTools` is a .NET 8 project that provides MCP tools for PII (Personally Identifiable Information) redaction in text and documents and text translation using Azure AI Language solutions.  

## Features

- **PII Redaction**: Automatically detect and redact sensitive information from text or documents.  
- **Translation**: Translate text into multiple languages.  

## Set up Instructions

### Prerequisites  

1. .NET 8 SDK installed on your system.  
2. Visual Studio or any other IDE supporting .NET development.
3. Azure AI multi-service resource or separate Language and Translation resources (depending one your use case)
4. (Optional) For using the provided MCP client, you will need an Azure Open AI resource with key based auth enabled and an open AI model already deployed (preferably gpt-4o).
5. If using the VS Code copilot for testing, you can skip step 4 and use Copilot models.

### Building the Mcp Server

1. Clone the repository: https://github.com/Azure-Samples/ai-language-samples.git. It might be easier to download the whole repository as a zip file and extract.
1. Update configurations in `appsettings.json` file and build solution.
1. Test using an MCP client.

#### Updating MCP server configurations

1. Nativate to the `src\dotnet\mcp\LanguageTools` folders
1. Open the `appSettings.json`
1. Update the required settings

##### Configuration guide

| Parameter Name                         | Description                                                       |
|----------------------------------------|-------------------------------------------------------------------|
| `LanguageSettings:Endpoint`                     | Endpoint for the Azure AI Language Resource                                      |
| `LanguageSettings:ApiKey`                           | ApiKey for the Azure AI Language Resource                             |
| `TranslatorSettings:Endpoint`    | Endpoint for the Azure AI Translator Resource. Prefer to use a `global` resource          |
| `TranslatorSettings:ApiKey`                        | ApiKey for the translator resource    |
| `TranslatorSettings:Region`                     | Region of the translator resource,                                  |

## Testing  the Mcp Server

### Using the provided MCP client

To test using the provided MCP client, follow these steps:

1. Open the `src/dotnet/mcp/LanguageTools.sln` in `Visual Studio`.
1. Navigate to the `McpClient` project in the solution and open the `appSettings.json` file.
1. Update the required settings as shown below.
1. Build the project
1. Run the project from Visual Studio
1. A terminal will be launched. Test the tools for PII redaction and Language Translation using natural language.

#### McpClient Configuration guide

| Parameter Name                         | Description                                                       |
|----------------------------------------|-------------------------------------------------------------------|
| `OpenAISettings:Endpoint`                     | Endpoint for the Azure OpenAI Resource                                      |
| `OpenAISettings:ApiKey`                           | ApiKey for the Azure OpenAI Resource                             |
| `OpenAISettings:DeploymentName`    | Name of the open AI model deployed for the resource (e.g., `gpt-4o`)        |

### Using VS Code - MCP Copilot extension

1. Install [nodejs](https://nodejs.org/en/download/)
1. Launch Visual Studio Code.
1. Install [MCP copilot extension](https://code.visualstudio.com/docs/copilot/chat/mcp-servers?wt.md_id=AZ-MVP-5004796#_enable-mcp-support-in-vs-code) for VS Code.
1. Click on the `Add Server` link to add the `LanguageTools` project as an STDIO server.

```json
"mcp": {
        "servers": {
            "language-tools": {
                "type": "stdio",
                "command": "dotnet",
                "args": ["run", "--project", "C:\\<workspace>\\ai-language-samples\\src\\dotnet\\mcp\\McpServer\\Tools\\LanguageTools\\LanguageTools.csproj", "All"],
            }
        }
    }
```

5. Open Copilot chat and switch to "Agent" mode from the drop down. Select the `gpt-4o` model from the models drop down
1.  Test the tools for PII redaction and Language Translation using natural language.

## Example Agent Interaction

### PII redaction text

User: Can you help redact PII information

Agent: Yes, I can help redact Personally Identifiable Information (PII) from text. Please provide the text you'd like me to process, and I will redact any PII present.

User: Here is the text "My full name is John Doe often listed as (John D.). I was born on January 7, 1985, and my Social Security Number (SSN)is 123-45-6789. You can reach me at (555) 223-4567 or via email at john.doe@example.com. I currently live at 1234 Elm Street, Redwood City, CA 94063. I’d also like to provide my driver’s license number, which is D1234567, issued by the state of California."

Agent: Returns a response containing redacted text (and entities optionally).

User: Can you only redact name and email address. 

Agent: Returns only name and email addresses redacted, etc.

### Translation

User: Can you translate the text to the French and German, "My full name is John Doe often listed as (John D.). I was born on January 7, 1985".

Agent: Here is the translation of the text:

    French: Mon nom complet est John Doe, souvent répertorié comme (John D.). Je suis né le 7 janvier 1985

    German: Mein vollständiger Name ist John Doe, oft als (John D.) aufgeführt. Ich bin am 7. Januar 1985 geboren 

