using Imagegram.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Imagegram.Tests;

public static class TestEnvironment
{
    private static readonly string SqlConnectionString;
    private const string ConfigFileName = "test-settings.json";
    
    static TestEnvironment()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(ConfigFileName)
            .Build();

        SqlConnectionString = config.GetSection("SqlServerConnectionString").Value;

        if (string.IsNullOrWhiteSpace(SqlConnectionString))
        {
            throw new InvalidOperationException(
                $"Connection string is not provided in configuration file '{ConfigFileName}'");
        }
    }

    public static DbContextOptions<ApplicationDbContext> DbOptions =>
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(SqlConnectionString)
            .Options;
    
    public static ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext(DbOptions);
    }
}