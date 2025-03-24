using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stride.Data.Services
{
    public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        private readonly ILogger<CustomPasswordValidator<TUser>> _logger;

        public CustomPasswordValidator(ILogger<CustomPasswordValidator<TUser>> logger)
        {
            _logger = logger;
        }

       public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
{
    _logger.LogInformation("CustomPasswordValidator is validating a password");
    
    var errors = new List<IdentityError>();
    if (string.IsNullOrEmpty(password))
    {
        _logger.LogWarning("Password is empty or null");
        errors.Add(new IdentityError
        {
            Code = "PasswordEmpty",
            Description = "Password cannot be empty"
        });
        return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
    } 

            if (password.Length < 4)
            {
                _logger.LogWarning("Password is too short");
                errors.Add(new IdentityError
                {
                    Code = "PasswordTooShort",
                    Description = "Password must be at least 4 characters long"
                });
            }

            if (!password.Any(char.IsUpper))
            {
                _logger.LogWarning("Password requires uppercase letter");
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequiresUpper",
                    Description = "Password must contain at least one uppercase letter"
                });
            }

            if (!password.Any(char.IsDigit))
            {
                _logger.LogWarning("Password requires digit");
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequiresDigit",
                    Description = "Password must contain at least one number"
                });
            }

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                _logger.LogWarning("Password requires special character");
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequiresNonAlphanumeric",
                    Description = "Password must contain at least one special character"
                });
            }

            if (errors.Count == 0)
            {
                _logger.LogInformation("Password validation successful");
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                _logger.LogWarning("Password validation failed with {ErrorCount} errors", errors.Count);
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}