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
    public class SignIn(ILogger<SignIn> logger, SignInManager<UserEntity> signInManager, SignInService signInService)
    {
        private readonly ILogger<SignIn> _logger = logger;
        private readonly SignInManager<UserEntity> _signInManager = signInManager;
        private readonly SignInService _signInService = signInService;

        [Function("SignIn")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                if (body != null)
                {
                    var validationResult = _signInService.ValidateSignIn(body);
                    if(validationResult.StatusCode == StatusCode.OK)
                    {
                        var signInRequest = (SignInRequest)validationResult.ContentResult!;
                        var signInResult = await _signInManager.PasswordSignInAsync(signInRequest.Email, signInRequest.Password, signInRequest.IsPersistent, false);

                        if(signInResult.Succeeded)
                        {
                            //Hämta token från TokenProvider

                            return new OkObjectResult("accesstoken");
                        }

                        return new UnauthorizedResult();
                    }
                }

                return new BadRequestResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"SignIn.Run() error: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
