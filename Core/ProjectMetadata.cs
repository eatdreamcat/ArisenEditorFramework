using System;

namespace ArisenEditorFramework.Core;

public class ProjectMetadata
{
    public string Name { get; set; } = "New Project";
    public string EngineVersionId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime LastModified { get; set; } = DateTime.Now;
    public string ProjectPath { get; set; } = string.Empty; // Full path to .arisenproj
    
    // UI Metadata
    public string PreviewImageURL { get; set; } = string.Empty;
    public string IconURL { get; set; } = string.Empty;
}
