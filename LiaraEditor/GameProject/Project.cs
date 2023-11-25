using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiaraEditor.Commons;
using LiaraEditor.DataStructures;
using Version = LiaraEditor.DataStructures.Version;

namespace LiaraEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string ProjectFileExtension { get; } = ".liaraproj";
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Path { get; set; }
        public string FullPath => $"{Path}\\{Name}{ProjectFileExtension}";
        
        [DataMember]
        public string Author { get; set; }
        
        [DataMember]
        public Version LiaraVersion { get; set; }


        [DataMember (Name = "Scenes")]
        private ObservableCollection<Scene> _scene = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            _scene.Add(new Scene("Default_Scene", this));
        }
    }
}
