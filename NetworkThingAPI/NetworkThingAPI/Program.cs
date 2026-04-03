using NetworkThingAPI.Models;

var builder = WebApplication.CreateBuilder(args);

string filePath = @"D:\Projects\Avalonia UI projects\Full App Projects\NetworkThing\Network Thing\NetworkThingAPI\NetworkThingAPI\FilesDir";
DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
FileInfo[] fileInfos = directoryInfo.GetFiles();

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

    foreach(FileInfo file in fileInfos)
    {
        files.Add(new FileModel(file));
        Console.WriteLine(file.Name);
    }

    Console.WriteLine("Got");
    return Results.Ok(files);
});

app.Run();