using LiaraEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static LiaraEditor.Utilities.ProjectValidator;

namespace LiaraEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }
        [DataMember]
        public List<string> HiddenFolders { get; set; }

        public byte[] Icon { get; set; }
        public byte[] Preview { get; set; }
        public string IconPath { get; set; }
        public string PreviewPath { get; set; }
        public string ProjectFilePath { get; set; }
    }

    class NewProject : ViewModelBase
    {
        // TODO: Get the path from the installation location
        private readonly string _projectTemplatesPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\source\\repos\\Liara\\LiaraEditor\\ProjectTemplates\\";
        private string _projectName = "NewProject";

        /// <summary>
        /// The name of the project
        /// </summary>
        public string ProjectName
        {
            get => _projectName; // get the value of the private field
            set // set the value of the private field
            {
                if (_projectName != value) // if the value is different
                {
                    _projectName = value; // set the value
                    ValidateProject(); // validate the project name
                    OnPropertyChanged(nameof(ProjectName)); // notify the UI that the value has changed
                }
            }
        }

        private string _projectPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Liara Projects\\";

        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    ValidateProject();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _templates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> Templates { get; }

        private bool _isValidate = false;
        public bool IsValidate
        {
            get => _isValidate;
            set
            {
                if (_isValidate != value)
                {
                    _isValidate = value;
                    OnPropertyChanged(nameof(IsValidate));
                }
            }
        }

        private string _errorMsg;
        public string ErrorMsg
        {
            get => _errorMsg;
            set
            {
                if (_errorMsg != value)
                {
                    _errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
                }
            }
        }
        private bool ValidateProject()
        {
            (ProjectNameError, string) NameResult = ProjectValidator.ValidateProjectName(ProjectName, ProjectPath);
            if (NameResult.Item1 != ProjectNameError.None)
            {
                ErrorMsg = string.Format("Error : {0} \"{1}\"", NameResult.Item1, NameResult.Item2);
                IsValidate = false;
                return false;
            }

            (ProjectPathError, string) PathResult = ProjectValidator.ValidateProjectPath(ProjectPath);
            if (PathResult.Item1 != ProjectPathError.None)
            {
                ErrorMsg = string.Format("Error : {0}", PathResult.Item1);
                IsValidate = false;
                return false;
            }
            ErrorMsg = string.Empty;
            IsValidate = true;
            Debug.WriteLine("No error");
            return true;
        }

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProject();
            if (!IsValidate)
            {
                return string.Empty;
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += Path.DirectorySeparatorChar;
            var projectPath = Path.Combine(ProjectPath, ProjectName);

            try
            {
                Directory.CreateDirectory(projectPath);
                foreach (var folder in template.Folders) { Directory.CreateDirectory(Path.Combine(projectPath, folder)); }
                foreach (var folder in template.HiddenFolders)
                { 
                    Directory.CreateDirectory(Path.Combine(projectPath, folder));
                    File.SetAttributes(Path.Combine(projectPath, folder), FileAttributes.Hidden);
                }
                // Debug
                var dir = new DirectoryInfo(projectPath);
                // Copier l'icone et la preview dans le dossier Misc
                File.Copy(template.IconPath, Path.Combine(projectPath, "Misc", Path.GetFileName(template.IconPath)));
                File.Copy(template.PreviewPath, Path.Combine(projectPath, "Misc", Path.GetFileName(template.PreviewPath)));

                // Créer le fichier de projet
                /*var project = new Project(ProjectName, projectPath);
                Serializer.ToFile(project, Path.Combine(projectPath, ProjectName + Project.ProjectFileExtension));*/

                var projectXml = File.ReadAllText(template.ProjectFilePath);
                projectXml = projectXml.Replace(template.ProjectType, ProjectName);
                var projectFile = Path.GetFullPath(Path.Combine(projectPath, ProjectName + Project.ProjectFileExtension));
                File.WriteAllText(projectFile, projectXml);
                return projectPath;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                // TODO: Log the error
                return string.Empty;
            }
        }

        public NewProject()
        {
            Templates = new ReadOnlyObservableCollection<ProjectTemplate>(_templates);
            try
            {
                var projectTemplates = Directory.GetFiles(_projectTemplatesPath, "*.xml", SearchOption.AllDirectories); // get all the project templates in a form of a list
                Debug.Assert(projectTemplates.Any()); // make sure there is at least one project template
                foreach (var file in projectTemplates)
                {
                    // Pour générer les fichiers de projet.
                    /*var projectTemplate = new ProjectTemplate()
                    {
                        ProjectType = "Empty Project",
                        ProjectFile = "project.liara",
                        Folders = new List<string>() { ".Liara", "Content", "Scripts" }
                    };

                    Serializer.ToFile(projectTemplate, file);*/

                    var projectTemplate = Serializer.FromFile<ProjectTemplate>(file); // deserialize the project template

                    // The icon and preview are stored in the same folder as the project template.
                    // The icon is a png file named "<project template name>.png" -> the path is "<project template name>.png"
                    // The preview is a png file named "<project template name>_Preview.png" -> the path is "<project template name>_Preview.png"
                    // The project file is a xml file named "<project template name>.xml" -> the path is "<project template name>.xml"
                    projectTemplate.IconPath = Path.Combine(Path.GetDirectoryName(file), $"{Path.GetFileNameWithoutExtension(file)}.png");
                    projectTemplate.PreviewPath = Path.Combine(Path.GetDirectoryName(file), $"{Path.GetFileNameWithoutExtension(file)}_Preview.png");
                    projectTemplate.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), projectTemplate.ProjectFile)); // TODO: Check if the path can be simplified
                    
                    projectTemplate.Icon = File.ReadAllBytes(projectTemplate.IconPath); // read the icon
                    projectTemplate.Preview = File.ReadAllBytes(projectTemplate.PreviewPath); // read the preview

                    _templates.Add(projectTemplate); // add the project template to the list
                }
                ValidateProject();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                // TODO: Log the error
            }
        }
    }
}
