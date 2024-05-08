using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

public class SignUpService(ILogger<SignUpService> logger, SignUpFactory signUpFactory, UserManager<UserEntity> userManager)
{
    private readonly ILogger<SignUpService> _logger = logger;
    private readonly SignUpFactory _signUpFactory = signUpFactory;
    private readonly UserManager<UserEntity> _userManager = userManager;

    public ResponseResult ValidateSignUp(string body)
    {
        try
        {
            var signUpRequest = JsonSerializer.Deserialize<SignUpRequest>(body);
            if (signUpRequest != null)
            {
                var validator = new SignUpRequestValidator();
                var validationResult = validator.Validate(signUpRequest);
                if (validationResult.IsValid)
                {
                    return ResponseFactory.Ok(signUpRequest);
                }
                else
                {
                    var errorString = "";
                    foreach (var error in validationResult.Errors)
                    {
                        errorString += $"{error}\n";
                    }
                    _logger.LogError(errorString);
                }
            }

            return ResponseFactory.Error();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error($"ValidateSignUp error: {ex.Message}");
        }
    }

    public async Task<bool> SignUpUserAsync(SignUpRequest signUpRequest)
    {
        try
        {
            var userEntity = _signUpFactory.PopulateUserEntity(signUpRequest);
            var createResult = await _userManager.CreateAsync(userEntity, signUpRequest.Password);

            if (createResult.Succeeded)
            {
                //Skicka verifieringskod-metod
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"SignUpUserAsync error: {ex.Message}");
            return false;
        }
    }
}
