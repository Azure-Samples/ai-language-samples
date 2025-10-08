# Overview

Azure AI Language is a cloud-based service that provides Natural Language Processing (NLP) features for understanding and analyzing text. Azure Text translation is a cloud-based REST API feature of the Translator service that uses neural machine translation technology to enable quick and accurate source-to-target text translation in real time across all supported languages.

In `LanguageAIMcp` solution there are a collection of MCP servers for many Azure.AI.Language and Azure.AI.Translator offerings. This solution contains 2 separate MCP servers

- `LanguageMcpServer` exposing the [Azure AI language](https://learn.microsoft.com/en-us/azure/ai-services/language-service/overview) capabilities as various MCP tools,
- `TranslatorMcpServer` exposing the [Azure Text translation](https://learn.microsoft.com/en-us/azure/ai-services/translator/text-translation/overview?tabs=windows) capabilities as MCP tools.

This solution also has a test client called `McpClient` which will allow users to test the MCP capabilities directly.

## LanguageMcpServer

`LanguageMcpServer` is a .NET 8 project that provides an MCP Server implementation which expose various Azure.AI.Language capabibilities as MCP tools. To use these tools, the developer will need an [Azure AI Foundry resource](https://learn.microsoft.com/en-us/azure/ai-services/multi-service-resource?pivots=azportal). 

Currecntly the following tools are supported:

- `PiiRedactionTool` - identifies entities in text and documents (PDF and Word) that are associated with individuals and redacts them.
- `ExtractEntitiesTool` - identifies different entries in text and categorizes them into predefined types.
- `ExtractHeathcareEntitiesTool` - Extracts and labels relevant health information from text.
- `ExtractKeyPhraseTool` - evaluates and returns the main concepts in unstructured text, and returns them as a list.
- `LanguageDetectionTool` - evaluates text and detects a wide range of languages and variant dialects.
- `SentimentAnalysisTool` - identifies positive or negative sentiments and can link them to specific elements within the text.
- `SummarizationTool` - condenses information for text and generates a summary.
- `ConversationalUnderstandingTool` - predicts the overall intention of an incoming utterance and extract important information from it based on custom natural language understanding models. To use this tool, the developer should already have a [Conversation Language Understanding (CLU) project trained and deployed](https://learn.microsoft.com/en-us/azure/ai-services/language-service/conversational-language-understanding/how-to/create-project?tabs=azure-ai-foundry) to their resource.
- `QuestionAnsweringTool` - identifies the most suitable answer for user inputs. To use this tool, the developer will need to have a [Custom Question Answering (CQA) project created and deployed](https://learn.microsoft.com/en-us/azure/ai-services/language-service/question-answering/how-to/create-test-deploy) to their language or Azure AI foundry resource

This MCP Server uses Standard Input/Output (stdio) transport to communicate with any MCP client.

## TranslatorMcpServer

`TranslatorMcpServer` is another .NET 8 project that provides the MCP Server implementation for Azure Text translation.

It supports one tool:

- `TranslationTool`: translates text into multiple languages

## Getting started

The MCP servers support two main usage paths:

Use the pre-built binaries – Ideal for users who want to integrate or run the tool without modifying the source.
Build and develop from source – For contributors or developers who want to customize or extend the functionality.

## Set up Instructions

### Prerequisites  

1. .NET 8 SDK installed on your system.  
1. Azure AI Foundry resource or separate Language and Translation resources (depending one your use case)
1. (Optional) MCP client of choice.

### Option 1: Use Pre-Built Binaries

You can download the latest binaries from the build artifacts.

### Option 2: Build and develop from source

1. Clone the repository: `git clone https://github.com/Azure-Samples/ai-language-samples.git`. It might be easier to download the whole repository as a zip file and extract.
1. Update configurations in `appsettings.json` for the Azure AI Foundry, Language or Translation resources
1. Build the `src\dotnet\mcp\LanguageMcp.sln` solution. `dotnet build .\src\dotnet\mcp\`
1. Test using an MCP client.

## Setting up the LanguageMcpServer

The `LanguageMcpServer` accepts one optional commandline argument which instructs which tools are needed. the accpeted value is a comma-separated string containing all the tools expected to be run. If nothing is specified all tools are run. You can use the `args` attrubute of the mcp server to pass this value.

Currently ths following values are supported for commandline parameter:

- `PiiRedactionTool`
- `ExtractEntitiesTool`
- `ExtractHeathcareEntitiesTool`
- `ExtractKeyPhraseTool`
- `LanguageDetectionTool`
- `SentimentAnalysisTool`
- `SummarizationTool`
- `ConversationalUnderstandingTool`
- `QuestionAnsweringTool`

### Required `appSettings` updates (Even When Using Binaries)

Even when using the pre-built binaries, the appsettings need to be updated in the `appsettings.json` file.

1. The `appsettings.json` file is located in the Server directory of the binary package.
1. When using the code, the `appsettings.json` is located in the individual MCP server folder. For `LanguageMcpServer`, it is in  `src\dotnet\mcp\LanguageMcpServer\`.

#### Required Updates for LanguageMcpServer

Before running the application, make sure to update the following settings in the appSettings.json:

```json
{
  "LanguageSettings": {
    "Endpoint": "YourLanguageEndpoint",
    "ApiKey": "YourLanguageApiKey",
    "QuestionAnsweringSettings": { // Optional configuration for configuring the custom Question Answering project.
      "ProjectName": "Your CQA project name.",
      "DeploymentName": "Your CQA deployment name."
    },
    "CLUSettings": { // Optional configuration for configuring the coversational understanding project.
      "ProjectName": "Your CLU project name.",
      "DeploymentName": "Your CLU deployment name"
    }
  }
}
```

| Parameter Name                         | Description                                                       |
|----------------------------------------|-------------------------------------------------------------------|
| `YourLanguageEndpoint`                 | Endpoint for the Azure AI Language / Azure AI Foundry resource to be used. (Required)  |
| `YourLanguageApiKey`                   | ApiKey for the Azure AI Language / Azure AI Foundry resource  to be used. (Required)|
| `QuestionAnsweringSettings.ProjectName` | The Custom Question Answering project name. (Required when using the `QuestionAnsweringTool`)|
| `QuestionAnsweringSettings.DeploymentName` | The Custom Question Answering deployment name. (Required when using the `QuestionAnsweringTool`)|
| `CLUSettings.ProjectName`              | The Conversational Language Understanding project name. (Required when using the `ConversationalUnderstandingTool`)|
| `CLUSettings.DeploymentName`            | The Conversational Language Understanding deployment name.(Required when using the `ConversationalUnderstandingTool`)|

## Setting up the TranslatorMcpServer

### Required `appSettings` updates in TranslatorMcpServer (Even When Using Binaries)

Even when using the pre-built binaries, the appsettings need to be updated in the `appsettings.json` file.

1. The `appsettings.json` file is located in the Server directory of the binary package.
1. When using the code, the `appsettings.json` is located in the individual MCP server folder. For `TranslatorMcpServer`, it is in  `src\dotnet\mcp\TranslatorMcpServer\`.

#### Required Updates for TranslatorMcpServer

Before running the application, make sure to update the following settings in the appSettings.json:

```json
{
  "TranslatorSettings": {
    "Endpoint": "https://api.cognitive.microsofttranslator.com",
    "ApiKey": "Your translator resource API key.",
    "Region": "Your translator resource region."
  }
}
```

| Parameter Name                         | Description                                                       |
|----------------------------------------|-------------------------------------------------------------------|
| `YourTranslatorEndpoint`               | Endpoint for the Azure AI Translator / Azure AI Foundry resource  to be used for Translation.               |
| `YourTranslatorApiKey`                 | ApiKey for the Translator/ Azure AI Foundry resource to be used for Translation          |
| `YourTranslatorRegion`                 | Region of the Translator / Azure AI Foundry resource to be used for Translation         |

## Testing / Integrating the MCP Servers

### Using Built-in MCP client

The `LanguagAIMcp` solution provides an MCP Console client called `McpClient` which can be used to integrate and tests the McpServers. To test using the provided MCP client, follow these steps:

1. Open the `src/dotnet/mcp/LanguageAIMcp.sln` in `Visual Studio`.
1. Navigate to the `McpClient` project in the solution and open the `appSettings.json` file.
1. Update the required settings as shown below. Pick the project you want to test - `LanguageMcpServer` or `TranslatorMcpServer`
1. Build the project
1. Run the project from Visual Studio
1. A terminal will be launched. Test the tools for Language capabilities and Language Translation using natural language.

### MCP Client Configuration Required

```json
{
  "OpenAISettings": {
    "Endpoint": "YourAzureOpenAIEndpoint",
    "ApiKey": "YourAzureOpenAIApiKey",
    "DeploymentName": "NameOfTheDeployment"
  },
  "Tools": "All",
  "ProjectPath": "Project Path for the Mcp Server" // Path of the MCP Server project
}
```

| Parameters                             | Description                                                                     |
|----------------------------------------|-------------------------------------------------------------------------------------|
| `YourAzureOpenAIEndpoint`              | Endpoint for the Azure OpenAI Resource.   |
| `YourAzureOpenAIApiKey`                | ApiKey for the Azure OpenAI Resource.     |
| `NameOfTheDeployment`                  | Name of the open AI model deployed for the resource (e.g., `gpt-4.1`) |
| `Tools`                                | Comma Separated list of tools to be tested |
| `ProjectPath`                          | Project Path for the Mcp Server            |

### Using VS Code - MCP Copilot extension

1. Install [nodejs](https://nodejs.org/en/download/)
1. Launch Visual Studio Code.
1. Install [MCP copilot extension](https://code.visualstudio.com/docs/copilot/chat/mcp-servers?wt.md_id=AZ-MVP-5004796#_enable-mcp-support-in-vs-code) for VS Code.
1. Click on the `Add Server` link to add the `LanguageMcpServer` or `TranslatorMcpServer` or both.
1. Open Copilot chat and switch to "Agent" mode from the drop down. Select the `gpt-4.1` model from the models drop down
1. Test the tools for PII redaction and Language Translation using natural language.

```json
"mcp": {
        "servers": {
            "language-mcp-server": {
                "type": "stdio",
                "command": "<path to MCP server binaries>/LanguageMcpServer.exe",
                "args": ["All"], // name of the tools to be run
            },
            "translator-mcp-server": {
                "type": "stdio",
                "command": "<path to MCP server binaries>/TranslatorMcpServer.exe",
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
