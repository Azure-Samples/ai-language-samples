// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

namespace Azure.AI.Language.MCP.Server.Enums.Healthcare
{
    /// <summary>
    /// Enumeration representing different types of healthcare documents.
    /// </summary>
    public enum HealthcareDocumentTypeEnum
    {
        None = 0,
        ClinicalTrial,
        DischargeSummary,
        ProgressNote,
        HistoryAndPhysical,
        Consult,
        Imaging,
        Pathology,
        ProcedureNote,
    }
}
