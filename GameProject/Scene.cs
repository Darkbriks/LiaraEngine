using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LiaraEditor.GameProject
{
    [DataContract]
    public class Scene : ViewModelBase
    {
        private string _name;
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    if (string.IsNullOrWhiteSpace(value)) { throw new ArgumentException("Scene name cannot be null or whitespace", nameof(value)); }
                    if (value.Contains(" ")) { throw new ArgumentException("Scene name cannot contain spaces", nameof(value)); }
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [DataMember]
        public Project Project { get; private set; }

        public Scene(string name, Project project)
        {
            Debug.Assert(project != null, "Project cannot be null");
            Name = name;
            Project = project;
        }
    }
}