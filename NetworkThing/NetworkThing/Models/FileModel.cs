using CommunityToolkit.Mvvm.ComponentModel;

namespace NetworkThing.Models;

public partial class FileModel : ObservableObject
{
    [ObservableProperty]
    private string fileName;

    public FileModel(string name)
    {
        FileName = name;
    }
}