using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.User;
using api.Enums;
using api.Helpers;
using api.Interfaces;
using api.Models;
using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace api.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthRepository> _logger;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthRepository(
                ApplicationDbContext context,
                IMapper mapper,
                IConfiguration configuration,
                ILogger<AuthRepository> logger,
                IPasswordHasher passwordHasher,
                IOptions<JwtSettings> jwtSettings,
                IHttpContextAccessor httpContextAccessor,
                LinkGenerator linkGenerator,
                IEmailService emailService
                )
        {
            _context = context;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            _logger = logger;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<User?> CreateNewUser(CreateUserDto createUserDto)
        {
            // check if email is already in db
            var currentUser = await UserExists(createUserDto.Email);

            if (currentUser)
            {
                _logger.LogWarning("Attempt to create duplicate user with email: {Email}", createUserDto.Email);
                return null;
            }

            var hashedPassword = _passwordHasher.HashPassword(createUserDto.Password);

            // create new user
            var newUser = new User
            {
                UserName = createUserDto.Email,
                Email = createUserDto.Email,
                PasswordHash = hashedPassword,
                IsVerified = false,
                ConfirmationToken = GenerateConfirmationToken(),
            };

            string confirmationLink;
            if (_httpContextAccessor?.HttpContext != null)
            {
                // generate confirmation link
                confirmationLink = GenerateConfirmationLink(newUser.Email, newUser.ConfirmationToken, _httpContextAccessor.HttpContext);
            }
            else
            {
                _logger.LogError("HttpContext is not available");
                throw new InvalidOperationException("HttpContext is not available.");
            }


            try
            {
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                // send confirmation email
                await _emailService.SendConfirmationEmailAsync(newUser.Email, confirmationLink);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while creating user: {Email}", createUserDto.Email);
                throw;
            }

            // return new user
            return newUser;

        }

        public async Task<(LoginResult Result, string? Token)> LoginUser(LoginUserDto loginUserDto)
        {
            // check if user exists
            var currentUser = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginUserDto.Email);

            // if user does not exist, return conflict
            if (currentUser == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Email}", loginUserDto.Email);
                return (LoginResult.UserNotFound, null);
            }

            // Check if PasswordHash exists
            if (string.IsNullOrEmpty(currentUser.PasswordHash))
            {
                _logger.LogWarning("Login attempt for locked account: {Email}", loginUserDto.Email);
                return (LoginResult.InvalidCredentials, null);
            }

            // verify password
            if (!_passwordHasher.VerifyPassword(loginUserDto.Password, currentUser.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for user: {Email}", loginUserDto.Email);
                return (LoginResult.InvalidPassword, null);
            }

            // check account is not verified
            if (currentUser.IsVerified)
            {
                _logger.LogWarning("Current user is not verified: {Email}", loginUserDto.Email);
                return (LoginResult.AccountLocked, null);
            }

            // if user does exist and password is verified, generate jwt
            var jwt = GenerateJwtToken(currentUser);
            _logger.LogInformation("JWT was created: {token}", jwt);

            return (LoginResult.Success, jwt);
        }

        /// <summary>
        /// The function `GenerateJwtToken` creates a JWT token for a given user with a specified
        /// expiration time and signing credentials.
        /// </summary>
        /// <param name="User">The `GenerateJwtToken` method you provided is used to generate a JSON Web
        /// Token (JWT) for the given `User` object. In this method, the JWT is created with the user's
        /// email as a claim and is signed using a secret key.</param>
        /// <returns>
        /// The method `GenerateJwtToken` returns a JWT (JSON Web Token) generated for the current user,
        /// based on the provided user's email and a secret key.
        /// </returns>
        private string GenerateJwtToken(User currentUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Email, currentUser.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }
                ),
                Audience = _jwtSettings.Audience,
                Issuer = _jwtSettings.Issuer,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(securityToken);
            return jwt;
        }
        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email);
        }

        public async Task<LogoutResult> LogoutUser()
        {
            // Extract token
            var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Extract 'jti'
            var jti = jwtToken.Claims.FirstOrDefault(el => el.Type == JwtRegisteredClaimNames.Jti)?.Value;

            // Extract expiration date
            var expiration = jwtToken.ValidTo;

            // if jti is valid, add token to blacklisted token
            if (!string.IsNullOrEmpty(jti))
            {
                var newBlacklistedToken = new BlacklistedToken
                {
                    Jti = jti,
                    Expiration = expiration,
                };
                _context.BlacklistedTokens.Add(newBlacklistedToken);
                await _context.SaveChangesAsync();
            }

            return LogoutResult.Success;

        }

        private string GenerateConfirmationToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        private string GenerateConfirmationLink(string email, string token, HttpContext httpContext)
        {
            var confirmationLink = _linkGenerator.GetUriByAction(
                httpContext,
                action: "ConfirmEmail",
                controller: "Auth",
                values: new { email, token },
                scheme: httpContext?.Request.Scheme

            );

            if (confirmationLink == null)
            {
                return "";
            }

            return confirmationLink;
        }


    }
}