using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountProvider.Functions
{
    public class SignUp(UserManager<UserEntity> userManager, SignUpService signUpService, ILogger<SignUpService> logger)
    {
        private readonly ILogger<SignUpService> _logger = logger;
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly SignUpService _signUpService = signUpService;

        [Function("SignUp")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                if (body != null)
                {
                    var validationResult = _signUpService.ValidateSignUp(body);
                    if (validationResult.StatusCode == StatusCode.OK)
                    {
                        var signUpRequest = (SignUpRequest)validationResult.ContentResult!;
                        if (await _userManager.Users.AnyAsync(x => x.Email == signUpRequest.Email))
                            return new ConflictResult();

                        else if (await _signUpService.SignUpUserAsync(signUpRequest))
                            return new OkResult();

                        else
                            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                    }
                }

                return new BadRequestResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"SignUp.Run() error: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
