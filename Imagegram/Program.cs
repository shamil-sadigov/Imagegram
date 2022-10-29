using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Imagegram.Controllers;

// TODO: Move to config
const string BlobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=sovsh;AccountKey=4LzhU4xxGoRxWJ84P9X71WK5DLsoJ8+/FOOdBoIuZsX9F7AVbsaE1ZghIGMjd6XtpAbZurpqqwHT+AStO0eUjA==;EndpointSuffix=core.windows.net";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ImageProcessor>();

builder.Services.AddSingleton(new BlobServiceClient(BlobStorageConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();