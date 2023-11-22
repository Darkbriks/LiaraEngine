using System;
using System.Collections.Generic;
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
        static public OpenProject()
        {

        }
    }
}
