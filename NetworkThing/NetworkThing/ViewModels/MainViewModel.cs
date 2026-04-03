using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NetworkThing.Models;

namespace NetworkThing.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<FileModel> fileList = new();

    public MainViewModel()
    {
        FileList.Add(new FileModel("file1.txt"));
        FileList.Add(new FileModel("file2.txt"));
        FileList.Add(new FileModel("file3.txt"));
    }
}