```csharp
// Pour générer les fichiers de projet.
var projectTemplate = new ProjectTemplate()
{
ProjectType = "Empty Project",
ProjectFile = "project.liara",
Folders = new List<string>() { ".Liara", "Content", "Scripts" }
};
Serializer.ToFile(projectTemplate, file);
```

// The icon and preview are stored in the same folder as the project template.
// The icon is a png file named "<project template name>.png" -> the path is "<project template name>.png"
// The preview is a png file named "<project template name>_Preview.png" -> the path is "<project template name>_Preview.png"
// The project file is a xml file named "<project template name>.xml" -> the path is "<project template name>.xml"

```csharp
// Créer le fichier de projet
var project = new Project(ProjectName, projectPath);
Serializer.ToFile(project, Path.Combine(projectPath, ProjectName + Project.ProjectFileExtension));
```


/*var projectXml = File.ReadAllText(template.ProjectFilePath);
projectXml = projectXml.Replace(template.ProjectType, ProjectName);
projectXml = projectXml.Replace(template.ProjectFilePath, fullProjectPath);
var projectFile = Path.GetFullPath(Path.Combine(fullProjectPath, ProjectName + Project.ProjectFileExtension));
File.WriteAllText(projectFile, projectXml);
return fullProjectPath;*/


Utiliser la version
OpenProject
ProjectBrowserDialog
Scene
Project
.liaraproj