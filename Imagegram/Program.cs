using System.Net;
using Azure.Storage.Blobs;
using Imagegram;
using Imagegram.Database;
using Imagegram.ExceptionHandling;
using Imagegram.Extensions;
using Imagegram.Features;
using Imagegram.Features.Posts;
using Imagegram.Features.Posts.CreatePost.Services;
using Imagegram.Features.Users;
using Imagegram.Features.Users.CreateUser;
using Imagegram.Features.Users.GetUserAccessToken;
using Imagegram.Features.Users.GetUserAccessToken.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// TODO: Extract it in method

var accessTokenOptionsSection = builder.Configuration.GetSection("AccessTokenOptions");
var accessTokenOptions = accessTokenOptionsSection.Get<AccessTokenOptions>();
accessTokenOptions.ThrowIfConfigurationInvalid();

var connectionString = builder.Configuration.GetConnectionString("Database");
var blobStorageConnectionString = builder.Configuration.GetConnectionString("BlobStorage");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new ConfigurationException("Connection string for database is not provided in configuration file");
}
if (string.IsNullOrWhiteSpace(blobStorageConnectionString))
{
    throw new ConfigurationException("Connection string is not provided in config file");
}

builder.Services
    .AddMediatR(typeof(Program))
    .AddControllers(x => x.Filters.Add<ExceptionFilter>())
    .Services
    .AddEndpointsApiExplorer()
    .AddSwaggerWithAuth()
    .AddExceptionToHttpStatusCodeMapping(x =>
    {
        x.Map<EntityNotFoundException>(HttpStatusCode.NotFound);
        x.Map<DuplicateEmailException>(HttpStatusCode.Conflict);
        x.Map<InvalidPasswordException>(HttpStatusCode.Conflict);
    })
    .Configure<AccessTokenOptions>(accessTokenOptionsSection)
    .Configure<ImageLimitOptions>(ops =>
    {
        // TODO: Extract hardcoded values to config
        ops.MaxAllowedLength = 104857600;
        ops.AllowedUploadFormats = new[] { "jpg", "bmp", "png" };
    })
    .AddSingleton(new BlobServiceClient(blobStorageConnectionString))
    .AddSingleton<IImageStorage, ImageStorage>()
    .AddSingleton<ImageProcessor>()
    .AddSingleton<IAccessTokenGenerator, AccessTokenGenerator>()
    .AddSingleton<IPasswordManager, PasswordManager>()
    .AddSingleton<ISystemTime, DefaultSystemTime>()
    .AddDataProtection()
    .Services
    .AddDbContext<ApplicationDbContext>(ops =>
    {
        ops.UseSqlServer(connectionString);
    })
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(ops =>
    {
        ops.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = accessTokenOptions.AppName,
            ValidAudience = accessTokenOptions.AppName,
            IssuerSigningKey = new SymmetricSecurityKey(accessTokenOptions.GetSecretBytes()),
            
            // Not for production
            RequireExpirationTime = false,
            ValidateLifetime = false 
        };
    });

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();