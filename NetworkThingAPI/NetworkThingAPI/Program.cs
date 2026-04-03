using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NetworkThingAPI.Models;

var builder = WebApplication.CreateBuilder(args);

string folderPath = builder.Configuration.GetValue<string>("FolderPath")!;


builder.Services.AddOpenApi();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapHub<FileHub>("/filehub");

var fileApi = app.MapGroup("/api/v1/files");

fileApi.MapGet("/", () =>
{
    List<FileModel> files = new List<FileModel>();

    DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
    FileInfo[] fileInfos = directoryInfo.GetFiles();

    foreach(FileInfo file in fileInfos)
    {
        files.Add(new FileModel(file));
        Console.WriteLine(file.Name);
    }

    Console.WriteLine("Got");
    return Results.Ok(files);
});

fileApi.MapGet("/{name}", (string name) =>
{
    var filePath = Path.Combine(folderPath, name);

    return Results.File(filePath, contentType: "application/octet-stream", fileDownloadName: name);
});

fileApi.MapPost("/", async (IFormFile file, IHubContext<FileHub> hubContext) =>
{
    if (file.Length > 0)
    {
        var filePath = Path.Combine(folderPath, file.FileName);

        using var stream = File.Create(filePath);
        await file.CopyToAsync(stream);
        await hubContext.Clients.All.SendAsync("FileUploaded");

        return;
    }
    Console.WriteLine("Error");
}).DisableAntiforgery();

app.Run();