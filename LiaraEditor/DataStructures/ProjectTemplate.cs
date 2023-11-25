using System.Runtime.Serialization;

namespace LiaraEditor.DataStructures;

[DataContract]
public class ProjectTemplate
{
    [DataMember] public string TemplateType { get; set; } = "ARCHIVE";
    [DataMember] public string TemplateName { get; set; } = "Blank";
    [DataMember] public string TemplateDescription { get; set; } = "A blank project template, for creating a new project from scratch.";
    [DataMember] public string TemplateAuthor { get; set; } = "Liara Team";
    [DataMember] public Version TemplateVersion { get; set; } = new Version(0, 1, 0, 0, ReleaseType.Experimental, "build+2023+11+25");
    [DataMember] public Version MinimumLiaraVersion { get; set; } = new Version(0, 0, 2, 1, ReleaseType.Experimental, "build+2023+11+??");
    [DataMember] public List<Version> RecommendedLiaraVersions { get; set; } = new List<Version>()
    {
        new Version(0, 0, 2, 0, ReleaseType.Experimental, "build+2023+11+22"),
        new Version(0, 0, 2, 1, ReleaseType.Experimental, "build+2023+11+??")
    };

    [DataMember] public bool IsBeta { get; set; } = false;
    [DataMember] public bool IsExperimental { get; set; } = true;
    [DataMember] public bool IsHidden { get; set; } = false;
    [DataMember] public bool IsObsolete { get; set; } = false;
    [DataMember] public List<string> Folders { get; set; } = new List<string>()
    {
        "Content", "Scripts", "Misc", "Misc\\Icons", "Misc\\Preview"
    };
    [DataMember] public List<string> HiddenFolders { get; set; } = new List<string>() { ".liara" };
    
    public byte[] Icon { get; set; }
    public byte[] Preview { get; set; }
    public string IconPath { get; set; }
    public string PreviewPath { get; set; }
    // The path of the folder containing the project file (absolute path, like C:\LiaraEditor\ProjectTemplates\GameTemplates\Blank)
    public string ProjectFilePath { get; set; }
    
    // TODO: The list of required packages or libraries or dependencies
}