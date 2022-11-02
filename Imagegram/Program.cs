using Azure.Storage.Blobs;
using Imagegram.Database;
using Imagegram.Features.Posts.Create;
using Imagegram.Features.Users;
using Imagegram.Features.Users.GetUserAccessToken;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// TODO: Move to config
const string blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=sovsh;AccountKey=4LzhU4xxGoRxWJ84P9X71WK5DLsoJ8+/FOOdBoIuZsX9F7AVbsaE1ZghIGMjd6XtpAbZurpqqwHT+AStO0eUjA==;EndpointSuffix=core.windows.net";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var accessTokenOptionsSection = builder.Configuration.GetSection("AccessTokenOptions");
var accessTokenOptions = accessTokenOptionsSection.Get<AccessTokenOptions>();
accessTokenOptions.ThrowIfNotValid();

var connectionString = builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string is not provided in config file");
}

builder.Services
    .AddMediatR(typeof(Program))
    .AddControllers(x=> x.Filters.Add<ExceptionFilter>())
    .Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddExceptionToHttpStatusCodeMapping(x =>
    {
        x.Map<EntityNotFoundException>(HttpStatusCode.NotFound);
        x.Map<DuplicateEmailException>(HttpStatusCode.Conflict);
    })
    .Configure<AccessTokenOptions>(accessTokenOptionsSection)
    .AddSingleton(new BlobServiceClient(blobStorageConnectionString))
    .AddSingleton<ImageProcessor>()
    .AddSingleton<IAccessTokenGenerator, AccessTokenGenerator>()
    .AddSingleton<IPasswordManager, PasswordManager>()
    .AddDataProtection()
    .Services
    .AddDbContext<ApplicationDbContext>(ops =>
    {
        ops.UseSqlServer(connectionString);
    })
    .AddAuthentication(ops =>
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