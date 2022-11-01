using System.Text;
using Azure.Storage.Blobs;
using Imagegram.Features.Posts.Create;
using Imagegram.Features.Users.GetUserAccessToken;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// TODO: Move to config
const string blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=sovsh;AccountKey=4LzhU4xxGoRxWJ84P9X71WK5DLsoJ8+/FOOdBoIuZsX9F7AVbsaE1ZghIGMjd6XtpAbZurpqqwHT+AStO0eUjA==;EndpointSuffix=core.windows.net";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMediatR(typeof(Program));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ImageProcessor>();

builder.Services.AddSingleton(new BlobServiceClient(blobStorageConnectionString));

var accessTokenOptionsSection = builder.Configuration.GetSection("AccessTokenOptions");

var accessTokenOptions = accessTokenOptionsSection.Get<AccessTokenOptions>();

accessTokenOptions.ThrowIfNotValid();

builder.Services.Configure<AccessTokenOptions>(accessTokenOptionsSection);

builder.Services.AddAuthentication(ops =>
    {
        ops.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        ops.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        ops.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(ops =>
    {
        ops.SaveToken = true;
        ops.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(accessTokenOptions.GetSecretBytes()),
            ValidIssuer = accessTokenOptions.AppName,
            ValidAudience = accessTokenOptions.AppName,
            ValidateIssuer = true,
            ValidateAudience = true,
            RequireExpirationTime = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };;
    });


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