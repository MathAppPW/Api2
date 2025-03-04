using MathApp.Dal.Interfaces;
using MathAppApi.Features.Authentication.Dtos;
using MathAppApi.Features.Authentication.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MathAppApi.Features.Authentication.Controllers;

//TODO: keep cookie names as constants;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private const string RefreshCookieName = "refresh_token";
    
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUserRepo _userRepo;
    private readonly IUserDataValidator _userDataValidator;
    private readonly ICookieService _cookieService;
    private readonly ILogger<UserController> _logger;

    public UserController(IPasswordHasher<User> passwordHasher, ITokenService tokenService, IUserRepo userRepo,
        IUserDataValidator userDataValidator, ICookieService cookieService, ILogger<UserController> logger)
    {
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _userRepo = userRepo;
        _userDataValidator = userDataValidator;
        _cookieService = cookieService;
        _logger = logger;
    }
    
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<TokenResponse>(StatusCodes.Status200OK)]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
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
