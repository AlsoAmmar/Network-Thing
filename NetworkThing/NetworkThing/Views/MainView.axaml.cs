using Avalonia.Controls;
using Avalonia.Interactivity;
using NetworkThing.Models;
using NetworkThing.ViewModels;

namespace NetworkThing.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Download(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var file = button!.DataContext as FileModel;
        string name = file!.FileName;
        
        ((MainViewModel)DataContext!).DownloadCommand.Execute(name);
    }
}