using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AccountProvider.Functions
{
    public class Verify(ILogger<Verify> logger, UserManager<UserEntity> userManager, VerificationService verificationService)
    {
        private readonly ILogger<Verify> _logger = logger;
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly VerificationService _verificationService = verificationService;

        [Function("Verify")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                if (body != null)
                {
                    var validationResult = _verificationService.ValidateVerificationRequest(body);
                    if (validationResult.StatusCode == StatusCode.OK)
                    {
                        var verificationRequest = (VerificationRequest)validationResult.ContentResult!;
                        // verify code VERIFICATIONPROVIDER
                        var isVerified = true;

                        if (isVerified)
                        {
                            var userEntity = await _userManager.FindByEmailAsync(verificationRequest.Email);
                            if(userEntity != null)
                            {
                                userEntity.EmailConfirmed = true;
                                await _userManager.UpdateAsync(userEntity);

                                if (await _userManager.IsEmailConfirmedAsync(userEntity))
                                    return new OkResult();
                            }
                        }
                    }
                }

                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"SignUp.Run() error: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
