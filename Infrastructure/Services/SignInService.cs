using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

public class SignInService(ILogger<SignInService> logger)
{
    private readonly ILogger<SignInService> _logger = logger;

    public ResponseResult ValidateSignIn(string body)
    {
        try
        {
            var signInRequest = JsonSerializer.Deserialize<SignInRequest>(body);
            if (signInRequest != null)
            {
                var validator = new SignInRequestValidator();
                var validationResult = validator.Validate(signInRequest);
                if (validationResult.IsValid)
                {
                    return ResponseFactory.Ok(signInRequest);
                }
                else
                {
                    var errorString = "";
                    foreach(var error in validationResult.Errors)
                    {
                        errorString += $"{error.ErrorMessage}\n";
                    }
                    _logger.LogError(errorString);
                }
            }

            return ResponseFactory.Error();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error($"ValidateSignIn error: {ex.Message}");
        }
    }
}
