using System.ComponentModel;
using System.Runtime.Serialization;

namespace LiaraEditor.Commons
{
    /// <summary>
    /// Cette classe est la classe de base de tous les ViewModels.
    /// Les ViewModels sont des classes qui contiennent les données et la logique de l'application.
    /// </summary>
    /// <remarks>
    /// Cette classe implémente l'interface <see cref="INotifyPropertyChanged"/> qui permet de déclencher un évènement lorsqu'une propriété du ViewModel est modifiée.
    /// </remarks>
    [DataContract(IsReference = true)]
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Cet évènement est déclenché lorsqu'une propriété du ViewModel est modifiée.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Cette méthode permet de déclencher l'évènement PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Le nom de la propriété qui a été modifiée.</param>
        /// <remarks>
        /// Cette méthode est appelée par les setters des propriétés du ViewModel.
        /// </remarks>
        /// <example>
        /// <code>
        /// public string MyProperty
        /// {
        ///     get => _myProperty;
        ///     set
        ///     {
        ///         _myProperty = value;
        ///         OnPropertyChanged(nameof(MyProperty));
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="PropertyChanged"/>
        /// <seealso cref="INotifyPropertyChanged"/>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}