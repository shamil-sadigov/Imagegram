using System.Net;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Imagegram.Api.ExceptionHandling;
using Imagegram.Api.Features;
using Imagegram.Api.Features.Posts;
using Imagegram.Api.Features.Posts.CreatePost.Services;
using Imagegram.Api.Features.Users;
using Imagegram.Api.Features.Users.CreateUser;
using Imagegram.Api.Features.Users.CreateUserAccessToken;
using Imagegram.Api.Features.Users.CreateUserAccessToken.Services;
using Imagegram.Api.Extensions;
using Imagegram.Database;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Imagegram.Api;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var accessTokenOptions = GetAccessTokenOptions(builder);
        (string sqlConnectionString, string blobStorageConnectionString) = GetConnectionStrings(builder);

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
            .Configure<AccessTokenOptions>(builder.Configuration.GetSection("AccessTokenOptions"))
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
                ops.UseSqlServer(sqlConnectionString);
            })
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(ops =>
            {
                ops.TokenValidationParameters = new TokenValidationParameters
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
        
        EnsureDatabasesAreReady(app);

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
    }

    private static (string sqlConnectionString, string blobStorageConnectionString) 
        GetConnectionStrings(WebApplicationBuilder builder)
    {
        var sqlConnectionString = builder.Configuration.GetConnectionString("Database");
        var blobStorageConnectionString = builder.Configuration.GetConnectionString("BlobStorage");

        if (string.IsNullOrWhiteSpace(sqlConnectionString))
            throw new ConfigurationException("Connection string for database is not provided in configuration file");
        
        if (string.IsNullOrWhiteSpace(blobStorageConnectionString))
            throw new ConfigurationException("Connection string is not provided in config file");
        
        return (sqlConnectionString, blobStorageConnectionString);
    }

    private static AccessTokenOptions GetAccessTokenOptions(WebApplicationBuilder builder)
    {
        var accessTokenOptionsSection = builder.Configuration.GetSection("AccessTokenOptions");
        var accessTokenOptions = accessTokenOptionsSection.Get<AccessTokenOptions>();
        accessTokenOptions.ThrowIfConfigurationInvalid();
        return accessTokenOptions;
    }

    private static void EnsureDatabasesAreReady(IHost app)
    {
        using var scope = app.Services.CreateScope();

        // Ensure SqlServer Database is ready
        
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        
        // Ensure blob containers are ready
        
        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();

        var requiredContainers = new []
        {
            ImageStorage.OriginalImagesContainerName, 
            ImageStorage.ProcessedImagesContainerName
        };
        
        var existingBlobContainers = blobServiceClient.GetBlobContainers();

        foreach (var requiredContainer in requiredContainers)
        {
            CreateContainerIfDoesntExist(requiredContainer);
        }
        
        void CreateContainerIfDoesntExist(string requiredContainer)
        {
            if (existingBlobContainers is null)
            {
                blobServiceClient.CreateBlobContainer(requiredContainer, PublicAccessType.Blob);
                return;
            }
            
            if (!existingBlobContainers.Any(x=> x.Name.Equals(requiredContainer, StringComparison.InvariantCultureIgnoreCase)))
            {
                blobServiceClient.CreateBlobContainer(requiredContainer, PublicAccessType.Blob);
            }
        }
    }
}
