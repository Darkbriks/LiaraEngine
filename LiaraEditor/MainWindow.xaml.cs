using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiaraEditor.DataStructures;
using LiaraEditor.GameProject;
using Version = LiaraEditor.DataStructures.Version;

namespace LiaraEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Version _liaraVersion = new Version(0, 0, 2, 1, ReleaseType.Experimental, "build+2023+11+25"); // TODO: Get the version from the settings
    
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnMainWindowLoaded;
    }

    private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnMainWindowLoaded;
        OpenProjectBrowserDialog();
    }

    private void OpenProjectBrowserDialog()
    {
        var projectBrowser = new ProjectBrowserDialog();
        if (projectBrowser.ShowDialog() == false)
        {
            Application.Current.Shutdown();
        }
        else
        {
            // TODO: Open project
        }
    }
}