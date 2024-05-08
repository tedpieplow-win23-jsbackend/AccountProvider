using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

public class VerificationService(ILogger<VerificationService> logger)
{
    private readonly ILogger<VerificationService> _logger = logger;

    public ResponseResult ValidateVerificationRequest(string body)
    {
        try
        {
            var verificationRequest = JsonSerializer.Deserialize<VerificationRequest>(body);
            if (verificationRequest != null)
            {
                var validator = new VerificationRequestValidator();
                var validationResult = validator.Validate(verificationRequest);
                if (validationResult.IsValid)
                {
                    return ResponseFactory.Ok(verificationRequest);
                }
                else
                {
                    var errorString = "";
                    foreach (var error in validationResult.Errors)
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
            return ResponseFactory.Error(ex.Message);
        }
    }
}
