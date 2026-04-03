using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
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
        _ = InitializeAsync();
        
        connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5080/filehub")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task InitializeAsync()
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