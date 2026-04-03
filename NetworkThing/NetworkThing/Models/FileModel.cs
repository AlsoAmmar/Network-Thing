using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NetworkThing.Models;

public partial class FileModel : ObservableObject
{
    public enum FileTypes
    {
        Image,
        Document,
        Other
    }
    
    [ObservableProperty] private string fileName;
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(IconBitmap))]
    private FileTypes fileType;
    
    public Bitmap IconBitmap => FileType switch
    {
        FileTypes.Image => new Bitmap(AssetLoader.Open(new Uri($"avares://NetworkThing/Assets/Images/Image.png"))),
        FileTypes.Document => new Bitmap(AssetLoader.Open(new Uri($"avares://NetworkThing/Assets/Images/Document.png"))),
        _ => new Bitmap(AssetLoader.Open(new Uri($"avares://NetworkThing/Assets/Images/Other.png")))
    };
}