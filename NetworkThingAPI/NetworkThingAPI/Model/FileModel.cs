using System.Text.Json.Serialization;

namespace NetworkThingAPI.Models;

public class FileModel
{   
    public enum FileTypes
    {
        Image,
        Document,
        Other
    }
    
    public string FileName { get; set; }
    public FileTypes FileType { get; set; }

    public FileModel(FileInfo info)
    {
        FileName = info.Name;
        FileType = info.Extension.ToLower() switch
        {
            ".txt" => FileTypes.Document,
            ".png" or ".jpg" => FileTypes.Image,
            _ => FileTypes.Other
        };
    }
}