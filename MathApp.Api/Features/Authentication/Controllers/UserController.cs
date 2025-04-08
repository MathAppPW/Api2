using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.DataStorages;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using MathAppApi.Shared.Emails.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace MathAppApi.Features.Authentication.Controllers;

//TODO: keep cookie names as constants;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUserRepo _userRepo;
    private readonly IUserProfileRepo _userProfileRepo;
    private readonly IUserDataValidator _userDataValidator;
    private readonly ICookieService _cookieService;
    private readonly IEmailService _emailService;
    private readonly IPasswordResetDataStorage _passwordResetDataStorage;
    private readonly ILogger<UserController> _logger;

    private readonly string _frontendUrl;
    private readonly string _resetPasswordEndpoint;
    
    public UserController(IPasswordHasher<User> passwordHasher, ITokenService tokenService, IUserRepo userRepo, IUserProfileRepo userProfileRepo,
        IUserDataValidator userDataValidator, ICookieService cookieService, ILogger<UserController> logger,
        IEmailService emailService, IPasswordResetDataStorage passwordResetDataStorage, IConfiguration config)
    {
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _userRepo = userRepo;
        _userProfileRepo = userProfileRepo;
        _userDataValidator = userDataValidator;
        _cookieService = cookieService;
        _logger = logger;
        _emailService = emailService;
        _passwordResetDataStorage = passwordResetDataStorage;

        _frontendUrl = config["Frontend:FrontendUrl"] ?? "NO_FRONTEND_URL_SET";
        _resetPasswordEndpoint = config["Frontend:ResetPasswordEndpoint"] ?? "NO_RESET_ENDPOINT_PROVIDED";
    }
    
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var waitTime = TimeSpan.FromSeconds(2);
        await Task.Delay(waitTime);
        
        var user = await _userRepo.FindOneAsync(u => u.Username == dto.Username);
        if (user == null)
            return Unauthorized(new MessageResponse("Invalid credentials"));
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        //TODO: implement rehashing
        if (verificationResult != PasswordVerificationResult.Failed)
        {
            var refreshToken = await _tokenService.GetRefreshToken(user);
            var response = await GetTokenResponse(refreshToken);
            _cookieService.SetCookie(Response, "RefreshToken", await _tokenService.GetRefreshToken(user));
            return Ok(response);
        }

        return Unauthorized(new MessageResponse("Invalid credentials"));
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var doesEmailExist = await _userRepo.AnyAsync(u => u.Email == dto.Email);
        if (doesEmailExist)
            return Conflict(new MessageResponse("User with this email already exists!"));
        var doesUsernameExist = await _userRepo.AnyAsync(u => u.Username == dto.Username);
        if (doesUsernameExist)
            return Conflict(new MessageResponse("User with this username already exists!"));
        var (isUserValid, message) = _userDataValidator.IsUserDataValid(dto);
        if (!isUserValid)
            return BadRequest(message);

        var user = new User()
        {
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = _passwordHasher.HashPassword(null!, dto.Password),
        };
        await _userRepo.AddAsync(user);
        var userProfile = new Models.UserProfile()
        {
            Id = user.Id,
            Level = 1,
            Experience = 0,
            Streak = 0,
            Lives = 10,
            LastLivesUpdate = DateTime.UtcNow,
            RocketSkin = 0,
            ProfileSkin = 0,
            History = []
        };
        await _userProfileRepo.AddAsync(userProfile);

        var refreshToken = await _tokenService.GetRefreshToken(user);
        var response = await GetTokenResponse(refreshToken);
        if (response == null)
            return Unauthorized();
        _cookieService.SetCookie(Response, "RefreshToken", refreshToken);
        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = _cookieService.GetCookie(Request, "RefreshToken");
        if (refreshToken == null)
        {
            _logger.LogInformation("Refresh attempt without refresh token.");
            return Unauthorized();
        }
        var isTokenValid = await _tokenService.IsRefreshTokenValid(refreshToken);
        if (!isTokenValid)
        {
            _logger.LogInformation("Refresh attempt with invalid token");
            return Unauthorized();
        }
        var response = await GetTokenResponse(refreshToken);
        return Ok(response);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("logout")]
    public async Task<IActionResult> LogOut()
    {
        var refreshToken = _cookieService.GetCookie(Request, "RefreshToken");
        if (refreshToken == null)
            return Ok();
        await _tokenService.RemoveRefreshToken(refreshToken);
        _cookieService.DeleteCookie(Response, "RefreshToken");
        return Ok();
    }

    //If given mail address is NOT in db it still returns ok 
    //but does not send the message
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var waitTime = TimeSpan.FromSeconds(2);
        await Task.Delay(waitTime);
        
        var email = dto.Email;
        var user = await _userRepo.FindOneAsync(u => u.Email == email);
        if (user == null)
            return Ok();
        var resetId = _passwordResetDataStorage.RegisterPasswordReset(user.Id);
        //TODO: fix this to correctly display address
        var emailContent = $"Hi {user.Username}, someone have requested a password reset from your email!\n" +
                           $"If it wasn't you contact us immediately.\n" +
                           $"To reset your email click the link below: ${_frontendUrl}/{resetId}";
        await _emailService.SendEmail(email, emailContent);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("resetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var userId = _passwordResetDataStorage.GetUserId(dto.ResetId);
        if (userId == null)
            return Unauthorized();
        var (isValid, message) = _userDataValidator.IsPasswordValid(dto.Password);
        if (!isValid)
            return BadRequest(message);
        var user = await _userRepo.GetAsync(userId);
        if (user == null)
        {
            _logger.LogError($"User with id {userId} not find in password reset!");
            return StatusCode(500);
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        await _userRepo.UpdateAsync(user);
        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveAccount([FromBody] ResetPasswordDto dto)
    {
        var userId = User.FindFirst("sub")?.Value;
        foreach (var claim in User.Claims)
            Console.WriteLine($"TYPE, VALUE: {claim.Type} {claim.Value}");

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("Valid token has no sub value!");
            throw new InvalidOperationException("'sub' value has not been found in auth token");
            _logger.LogError("User with valid token has not been found in RemoveAccount");
            return Ok();
        }

        var user = await _userRepo.GetAsync(userId);
        if (user == null)
        {
            _logger.LogError($"User with id {userId} not find in remove account!");
            return StatusCode(500);
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) ==
            PasswordVerificationResult.Failed)
        {
            return Unauthorized();
        }
        
        await _userRepo.RemoveAsync(user);
        
        return Ok();
    }


    [Authorize]
    [ProducesResponseType<EmailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("email")]
    public async Task<IActionResult> GetEmail()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
        {
            _logger.LogInformation("User email fetch attempt with no userId.");
            return Unauthorized();
        }

        var user = await _userRepo.GetAsync(userId);
        if (user == null)
        {
            _logger.LogError($"User with id {userId} not find in email fetch!");
            return BadRequest();
        }

        return Ok(new EmailResponse()
        {
            Email = user.Email,
            Username = user.Username
        });
    }

            

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok();
    }
    
    private async Task<TokenResponse?> GetTokenResponse(string refreshToken)
    {
        var accessToken = await _tokenService.GetAccessToken(refreshToken);
        if (accessToken == null)
            return null;
        return new TokenResponse()
        {
            AccessToken = accessToken
        };
    }
}
