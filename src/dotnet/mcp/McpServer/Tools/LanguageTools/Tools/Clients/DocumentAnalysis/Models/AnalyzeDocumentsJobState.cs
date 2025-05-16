//Copyright(c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.Text;

namespace LanguageAgentTools.Tools.Clients.DocumentAnalysis.Models
{
    public class AnalyzeDocumentsJobState
    {
        /// <summary> display name. </summary>
        public string DisplayName { get; set; }
        
        /// <summary> Date and time job created. </summary>
        public DateTimeOffset CreatedDateTime { get; set; }
        
        /// <summary> Date and time job expires. </summary>
        public DateTimeOffset? ExpirationDateTime { get; set; }
        
        /// <summary> job ID. </summary>
        public Guid JobId { get; set; }
        
        /// <summary> last updated date and time. </summary>
        public DateTimeOffset LastUpdatedDateTime { get; set; }
        
        /// <summary> status. </summary>
        public string Status { get; set; }
        
        /// <summary> errors. </summary>
        public IReadOnlyList<AnalyzeDocumentsError> Errors { get; set; }

        /// <summary> error. </summary>
        public AnalyzeDocumentsError Error { get; set; }

        /// <summary> next link. </summary>
        public string NextLink { get; set; }

        /// <summary> List of tasks. </summary>
        public AnalyzeDocumentsJobResult Tasks { get; set; }

        /// <summary> if showStats=true was specified in the request this field will contain information about the request payload. </summary>
        public RequestStatistics Statistics { get; set; }
    }
}
