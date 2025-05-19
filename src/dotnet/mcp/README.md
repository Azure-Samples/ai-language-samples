# LanguageTools  

`LanguageTools` is a .NET 8 project that provides an MCP Server implementation with built-in support to use PII (Personally Identifiable Information) redaction in text and documents and text translation tools.  
This MCP Server uses Standard Input/Output (stdio) transport to communicate with any MCP client.

## Features

- **PII Redaction**: [Tool to automatically detect and redact sensitive information from text or documents.](https://learn.microsoft.com/en-us/azure/ai-services/language-service/personally-identifiable-information/overview?tabs=text-pii)
- **Translation**: [Tool to translate text into multiple languages](https://learn.microsoft.com/en-us/azure/ai-services/translator/overview).

## Getting started

This project supports two main usage paths:

Use the pre-built binaries – Ideal for users who want to integrate or run the tool without modifying the source.
Build and develop from source – For contributors or developers who want to customize or extend the functionality.

## Set up Instructions

### Prerequisites  

1. .NET 8 SDK installed on your system.  
1. Azure AI service resource or separate Language and Translation resources (depending one your use case)
1. (Optional) MCP client of choice.

### Option 1: Use Pre-Built Binaries

You can download the latest binaries from the build artifacts.

### Option 2: Build and develop from source

1. Clone the repository: `git clone https://github.com/Azure-Samples/ai-language-samples.git`. It might be easier to download the whole repository as a zip file and extract.
1. Update configurations in `appsettings.json` for the language/translation resources
1. Build the `src\dotnet\mcp\LanguageMcp.sln` solution. `dotnet build .\src\dotnet\mcp\`
1. Test using an MCP client.

#### Commandline arguments

The MCP Server can accept an optional command-line parameter which instructs which tools are needed. Values:

1. All - for all tools
1. PiiRedactionTool - for the PII tools
1. TranslatorTool - for the translator tools

### Language MCP Server Configuration Required (Even When Using Binaries)

Even if you're using the pre-built binaries, you must configure the application by updating the `appsettings.json` file.

#### Location

1. The `appsettings.json` file is located in the Server directory of the binary package.
1. When using the code, the `appsettings.json` is located in the `src\dotnet\mcp\Server\` folder

#### Required Updates

Before running the application, make sure to update the following settings:

```json
{
  "LanguageSettings": {
    "Endpoint": "YourLanguageEndpoint",
    "ApiKey": "YourLanguageApiKey"
  },
  "TranslatorSettings": {
    "Endpoint": "https://api.cognitive.microsofttranslator.com",
    "ApiKey": "YourTranslatorApiKey",
    "Region": "YourTranslatorRegion"
  }
}
```

| Parameter Name                         | Description                                                       |
|----------------------------------------|-------------------------------------------------------------------|
| `YourLanguageEndpoint`                 | Endpoint for the Azure AI Language / Azure AI Aervices resource to be used for PII detection.  |
| `YourLanguageApiKey`                   | ApiKey for the Azure AI Language / Azure AI services resource  to be used for PII detection.|
| `YourTranslatorEndpoint`               | Endpoint for the Azure AI Translator / Azure AI services resource  to be used for Translation.               |
| `YourTranslatorApiKey`                 | ApiKey for the Translator/ Azure AI service resource to be used for Translation          |
| `YourTranslatorRegion`                 | Region of the Translator / Azure AI service resource to be used for Translation         |

If you only want to perform only one operation, you can update just those settings.

## Testing the Language MCP Server

You can test the MCP Server by connecting it to any MCP Client as an stdio transport. The stdio transport enables communication through standard input and output streams.

### Using the binaries

```dotnetcli

command: "<path to MCP server binaries>/LanguageMcpServer.exe"
args: ["All"] // You can use "All" to register all tools or the name of the individual tools you want to test

```

### Configuring the built-in MCP client

The LanguagMcp solution provides an MCP Console client called `LanguageMcpClient` which can be used to integrate with the McpServer. To test using the provided MCP client, follow these steps:

1. Open the `src/dotnet/mcp/LanguageTools.sln` in `Visual Studio`.
1. Navigate to the `LanguageMcpClient` project in the solution and open the `appSettings.json` file.
1. Update the required settings as shown below.
1. Build the project
1. Run the project from Visual Studio
1. A terminal will be launched. Test the tools for PII redaction and Language Translation using natural language.

#### Language MCP Client Configuration Required

```json
{
  "OpenAISettings": {
    "Endpoint": "YourAzureOpenAIEndpoint",
    "ApiKey": "YourAzureOpenAIApiKey",
    "DeploymentName": "NameOfTheDeployment"
  },
  "Tools": "All" //Allowed Values: All, PiiRedactionTool, TranslatorTool
}
```

| Parameters                             | Description                                                         |
|----------------------------------------|---------------------------------------------------------------------|
| `YourAzureOpenAIEndpoint`              | Endpoint for the Azure OpenAI Resource or Azure AI Service resource |
| `YourAzureOpenAIApiKey`                | ApiKey for the Azure OpenAI Resource or Azure AI Service resource.  |
| `NameOfTheDeployment`                  | Name of the open AI model deployed for the resource (e.g., `gpt-4o`)|

### Using VS Code - MCP Copilot extension

1. Install [nodejs](https://nodejs.org/en/download/)
1. Launch Visual Studio Code.
1. Install [MCP copilot extension](https://code.visualstudio.com/docs/copilot/chat/mcp-servers?wt.md_id=AZ-MVP-5004796#_enable-mcp-support-in-vs-code) for VS Code.
1. Click on the `Add Server` link to add the `LanguageMcpServer`.
1. Open Copilot chat and switch to "Agent" mode from the drop down. Select the `gpt-4o` model from the models drop down
1. Test the tools for PII redaction and Language Translation using natural language.

```json
"mcp": {
        "servers": {
            "language-mcp-server": {
                "type": "stdio",
                "command": "<path to MCP server binaries>/LanguageMcpServer.exe",
                "args": ["All"],
            }
        }
    }
```

## Example Agent Interaction

### PII redaction text

User: Can you help redact PII information

Agent: Yes, I can help redact Personally Identifiable Information (PII) from text. Please provide the text you'd like me to process, and I will redact any PII present.

User: Here is the text "My full name is John Doe often listed as (John D.). I was born on January 7, 1985, and my Social Security Number (SSN)is 123-45-6789. You can reach me at (555) 223-4567 or via email at john.doe@example.com. I currently live at 1234 Elm Street, Redwood City, CA 94063. I’d also like to provide my driver’s license number, which is D1234567, issued by the state of California."

Agent: Returns a response containing redacted text (and entities optionally).

User: Can you only redact name and email address?

Agent: Returns only name and email addresses redacted.

### Translation

User: Can you translate the text to the French and German, "My full name is John Doe often listed as (John D.). I was born on January 7, 1985".

Agent: Here is the translation of the text:

    **French:** Mon nom complet est John Doe, souvent répertorié comme (John D.). Je suis né le 7 janvier 1985

    **German:** Mein vollständiger Name ist John Doe, oft als (John D.) aufgeführt. Ich bin am 7. Januar 1985 geboren 

### Document redaction

User: Can you help redact PII information from this PDF "https://my-azure-blob-location/input/Content.pdf"

Agent: Sure where do you want to save the redacted doc?

User: Save it in the same storage container but in the "output" container instead.

Agent: The PII information within your PDF document has been successfully redacted. The redacted files have been saved in the "output" container. You can access the files using the following links:

1. **Redacted PDF**: [Redacted Document](https://my-azure-blob-location/output/113f7244-0a91-49cc-9b69-2728b24778f5/PiiEntityRecognition/0001/Content.pdf)
2. **Redaction Results (JSON)**: [Redaction Details](https://my-azure-blob-location/output/113f7244-0a91-49cc-9b69-2728b24778f5/PiiEntityRecognition/0001/Content.json)
