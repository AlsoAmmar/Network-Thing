using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using NetworkThing.Models;

namespace NetworkThing.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    HttpClient client = new();
    
    [ObservableProperty] private ObservableCollection<FileModel> fileList = new();
    [ObservableProperty] private string texty;
    [ObservableProperty] private Uri folderPath = new("avares://NetworkThing/Assets/Images");
    private HubConnection connection;

    public MainViewModel()
    {
        connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5080/filehub")
            .WithAutomaticReconnect()
            .Build();

        connection.On("FileUploaded", async () =>
        {
            Console.WriteLine("A file has been uploaded");
            await GetFiles();
        });
        
        _ = InitializeAsync();
    }

    [RelayCommand]
    public async Task Download(string name)
    {
        var topLevel = TopLevel.GetTopLevel(App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop 
            ? desktop.MainWindow 
            : (App.Current?.ApplicationLifetime as ISingleViewApplicationLifetime)?.MainView);
        
        var fileResult = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Choose where to save the file",
            SuggestedFileName = name
        });

        if (fileResult != null)
        {
            using var stream = await fileResult.OpenWriteAsync();
            
            var response = await client.GetStreamAsync($"http://localhost:5080/api/v1/files/{name}");
            await response.CopyToAsync(stream);
        }
    }

    [RelayCommand]
    public async Task Upload()
    {
        var topLevel = TopLevel.GetTopLevel(App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop 
            ? desktop.MainWindow 
            : (App.Current?.ApplicationLifetime as ISingleViewApplicationLifetime)?.MainView);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose the file",
            AllowMultiple = false
        });

        var file = files[0];
        
        using var fileStream = await file.OpenReadAsync();
        
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        
        content.Add(streamContent, "file", file.Name);
        
        await client.PostAsync("http://localhost:5080/api/v1/files", content);
    }

    public async Task InitializeAsync()
    {
        await connection.StartAsync();
        await GetFiles();
    }

    public async Task GetFiles()
    {
        using HttpResponseMessage response = await client.GetAsync("http://localhost:5080/api/v1/files");

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            Texty = json;
        
            List<FileModel> newFiles = new List<FileModel>();

            try
            {
                newFiles = JsonSerializer.Deserialize<List<FileModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            FileList.Clear();
            foreach(FileModel file in newFiles)
            {
                FileList.Add(file);
            }
        }
    }
}