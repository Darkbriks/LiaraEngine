using LiaraEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using LiaraEditor.Commons;
using LiaraEditor.DataStructures;
using static LiaraEditor.Utilities.ProjectValidator;
using Version = LiaraEditor.DataStructures.Version;

namespace LiaraEditor.GameProject
{ 
    class NewProject : ViewModelBase
    {
        ////////// Private fields //////////
        // TODO: Get the path from the installation location
        private readonly string _projectTemplatesPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Liara\\LiaraEditor\\ProjectTemplates\\";
        private string _projectName = "NewProject";
        private string _projectPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Liara Projects\\";
        private ObservableCollection<ProjectTemplate> _templates = new ObservableCollection<ProjectTemplate>();
        private bool _isValidate;
        private string _errorMsg;

        private Version _liaraVersion = new Version(0, 0, 2, 1, ReleaseType.Experimental, "build+2023+11+24"); // TODO: Get the version from the settings
        
        ////////// Properties //////////
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    IsValidate = ValidateProject();
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }
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
        public ReadOnlyObservableCollection<ProjectTemplate> Templates { get; }
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
        
        ////////// Constructor //////////
        public NewProject()
        {
            Templates = new ReadOnlyObservableCollection<ProjectTemplate>(_templates);
            try
            {
                // get all the project templates in a form of a list of strings
                var projectTemplates = Directory.GetFiles(_projectTemplatesPath, "*.xml", SearchOption.AllDirectories);
                Debug.Assert(projectTemplates.Any());
                
                // for each project template, deserialize it and add it to the list of templates
                foreach (var file in projectTemplates)
                {
                    // Si le fichier est le fichier de valeurs par défaut, passer au suivant
                    if (file.Contains("DefaultValues")) continue;
                    
                    var projectTemplate = Serializer.FromFile<ProjectTemplate>(file);
                    projectTemplate.ProjectFilePath = Path.GetFullPath(file);
                    
                    // Copier l'icone et la preview dans le dossier Misc (si le dossier existe)
                    if (projectTemplate.Folders.Contains("Misc"))
                    {
                        projectTemplate.IconPath = Path.GetFullPath(file.Replace(".xml", ".png"));
                        projectTemplate.PreviewPath = Path.GetFullPath(file.Replace(".xml", "_Preview.png"));
                        projectTemplate.Icon = File.ReadAllBytes(projectTemplate.IconPath);
                        projectTemplate.Preview = File.ReadAllBytes(projectTemplate.PreviewPath);
                    }
                    // Sinon, utiliser l'icone et la preview par défaut
                    else
                    {
                        projectTemplate.Icon = File.ReadAllBytes(Path.Combine(_projectTemplatesPath, "DefaultValues", "Icon.png"));
                        projectTemplate.Preview = File.ReadAllBytes(Path.Combine(_projectTemplatesPath, "DefaultValues", "Preview.png"));
                        projectTemplate.IconPath = Path.Combine(_projectTemplatesPath, "DefaultValues", "Icon.png");
                        projectTemplate.PreviewPath = Path.Combine(_projectTemplatesPath, "DefaultValues", "Preview.png");
                    }

                    // Ajouter le template à la liste
                    _templates.Add(projectTemplate);
                    Console.WriteLine(projectTemplate.TemplateName);
                }
                IsValidate = ValidateProject();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                // TODO: Log the error
            }
        }
        
        ////////// Methods //////////
        private bool ValidateProject()
        {
            (ProjectNameError, string) nameResult = ValidateProjectName(ProjectName, ProjectPath);
            if (nameResult.Item1 != ProjectNameError.None)
            {
                ErrorMsg = string.Format("Error : {0} \"{1}\"", nameResult.Item1, nameResult.Item2); return false;
            }

            (ProjectPathError, string) pathResult = ProjectValidator.ValidateProjectPath(ProjectPath);
            if (pathResult.Item1 != ProjectPathError.None)
            {
                ErrorMsg = string.Format("Error : {0}", pathResult.Item1); return false;
            }
            ErrorMsg = string.Empty;
            return true;
        }

        public string CreateProject(ProjectTemplate template)
        {
            // If the project is not valid, return an empty string
            if (!ValidateProject()) return string.Empty;

            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += Path.DirectorySeparatorChar;
            var fullProjectPath = Path.Combine(ProjectPath, ProjectName);

            try
            {
                // Create the project folder and all the subfolders
                Directory.CreateDirectory(fullProjectPath);
                foreach (var folder in template.Folders) { Directory.CreateDirectory(Path.Combine(fullProjectPath, folder)); }
                
                // Create the hidden folders
                foreach (var folder in template.HiddenFolders)
                { 
                    Directory.CreateDirectory(Path.Combine(fullProjectPath, folder));
                    File.SetAttributes(Path.Combine(fullProjectPath, folder), FileAttributes.Hidden);
                }
                
                // Copier l'icone et la preview dans le dossier Misc (et créer le dossier si il n'existe pas)
                if (!template.Folders.Contains("Misc")) { Directory.CreateDirectory(Path.Combine(fullProjectPath, "Misc")); }
                File.WriteAllBytes(Path.Combine(fullProjectPath, "Misc\\Icons", "Icon.png"), template.Icon);
                File.WriteAllBytes(Path.Combine(fullProjectPath, "Misc\\Preview", "Preview.png"), template.Preview);
                
                // Create the project file
                string projectFile = Path.Combine(fullProjectPath, ProjectName + Project.ProjectFileExtension);
                Project project = Serializer.FromFile<Project>(template.ProjectFilePath.Replace(".xml", Project.ProjectFileExtension));
                project.Name = ProjectName;
                project.Author = Environment.UserName; // TODO: Get the author name from the settings
                project.Path = ProjectPath;
                project.LiaraVersion = _liaraVersion;
                Serializer.ToFile(project, projectFile);
                return fullProjectPath;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                // TODO: Log the error
            }
            return string.Empty;
        }
    }
}
