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

namespace LiaraEditor.GameProject
{
    [DataContract]
    public class  ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProjectPath { get; set; }
        [DataMember]
        public DateTime LastOpened { get; set; }
        public string FullPath { get => $"{ProjectPath}{ProjectName}{Project.ProjectFileExtension}"; }
        public byte[] Icon { get; set; }
        public byte[] Preview { get; set; }
    }

    [DataContract]
    public class ProjectDataCollection
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }

    public class OpenProject
    {
        private static readonly string _applicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LiaraEditor");
        private static readonly string _projectDataPath;
        private static readonly ObservableCollection<ProjectData> _projectData = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> ProjectData
        { get; }

        static OpenProject()
        {
            try
            {
                if (!Directory.Exists(_applicationDataPath))
                {
                    Directory.CreateDirectory(_applicationDataPath);
                }
                _projectDataPath = Path.Combine(_applicationDataPath, "projects.xml");
                ProjectData = new ReadOnlyObservableCollection<ProjectData>(_projectData);
                ReadProjectData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: Log error
            }
        }

        private static void ReadProjectData()
        {
            if (File.Exists(_projectDataPath))
            {
                var projectDataCollection = Serializer.FromFile<ProjectDataCollection>(_projectDataPath).Projects.OrderByDescending(x => x.LastOpened).ToList();
                _projectData.Clear();
                foreach (var projectData in projectDataCollection)
                {
                    if (File.Exists(projectData.FullPath))
                    {
                        projectData.Icon = File.ReadAllBytes(Path.Combine(projectData.ProjectPath, "icon.png"));
                        projectData.Preview = File.ReadAllBytes(Path.Combine(projectData.ProjectPath, "preview.png"));
                        // TODO : Modify name of icon and preview
                    }
                }
            }
        }

        private static void WriteProjectData()
        {
            var projects = _projectData.OrderBy(x => x.LastOpened).ToList();
            Serializer.ToFile(new ProjectDataCollection() { Projects = projects }, _projectDataPath);
        }

        public static Project Open(ProjectData projectData)
        {
            ReadProjectData();
            var project = _projectData.FirstOrDefault(x => x.FullPath == projectData.FullPath);
            if (project != null)
            {
                project.LastOpened = DateTime.Now;
            }
            else
            {
                project = projectData;
                project.LastOpened = DateTime.Now;
                _projectData.Add(project);
            }

            WriteProjectData();

            return null;
        }
    }
}
