﻿using FluentAssertions;
using Imagegram.Database.Models;
using Imagegram.Features.Users;
using Imagegram.Features.Users.GetUserAccessToken;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Imagegram.Tests;

public class PasswordManagerTests
{
    [Theory]
    [InlineData("someemail@gmail.com", "password123")]
    [InlineData("another@gmail.com", "another-password123")]
    public void Can_validate_user_password_when_provided_password_is_correct(string email, string rawPassword)
    {
        var passwordManager = CreatePasswordManager();
        
        var protectedPassword = passwordManager.ProtectUserPassword(email, rawPassword);

        var user = new User()
        {
            Email = email,
            Password = protectedPassword
        };
        
        passwordManager.IsUserPasswordValid(user, rawPassword)
            .Should()
            .BeTrue();
    }
    
    [Theory]
    [InlineData("someemail@gmail.com", "password123")]
    [InlineData("another@gmail.com", "another-password123")]
    public void Can_detect_invalid_password(string email, string rawPassword)
    {
        var passwordManager = CreatePasswordManager();
        
        var protectedPassword = passwordManager.ProtectUserPassword(email, rawPassword);

        var user = new User()
        {
            Email = email,
            Password = protectedPassword
        };

        var invalidRawPassword = rawPassword + "1";

        passwordManager.IsUserPasswordValid(user, invalidRawPassword)
            .Should()
            .BeFalse();
    }

    private static PasswordManager CreatePasswordManager()
    {
        var dataProtector = new ServiceCollection()
            .AddDataProtection()
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IDataProtectionProvider>();

        var passwordManager = new PasswordManager(dataProtector);
        return passwordManager;
    }
}