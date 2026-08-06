using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using HotelApplication.Models;
using HotelApplication.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HotelApplication.Services
{
    public class AuthService(IConfiguration configuration) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        public string HashPassword(string password, User user)
        {
            return new PasswordHasher<User>().HashPassword(user, password);
        }
        
        public bool VerifyPassword(User user, string password)
        {
            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password);
            return result != PasswordVerificationResult.Failed;
        }        

        public PasswordValidationResult PasswordChecker(string password)
        {
            var errors = new List<string>();
            //Check if password is null
            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password cannot be empty.");
                return new PasswordValidationResult
                {
                    IsValid = false,
                    Errors = errors
                };
            }

            //Other checks
            if (password.Length < 8) errors.Add("Password must be at least 8 characters");
            if (password.Length > 32) errors.Add("Password cannot be longer than 32 characters");
            if (!password.Any(char.IsLower) || !password.Any(char.IsUpper)) errors.Add("Password must contain at least one upper and lower case characters");
            if (!password.Any(c => !char.IsLetterOrDigit(c))) errors.Add("Password must contain at least one special character");

            return new PasswordValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }

        public string GenerateJsonWebToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new(ClaimTypes.UserData,user.FullName),
                new(ClaimTypes.Email, user.EmailAddress),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(ClaimTypes.Name, user.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AuthSettings:JWTKey") ?? throw new InvalidOperationException("AuthSettings:JWTKey is missing from configuration")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AuthSettings:Issuer"),
                audience: _configuration.GetValue<string>("AuthSettings:Audience"),
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>("AuthSettings:TokenExpiryInMinutes"))
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public bool IsValidEmail(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress)) return false;
            return MailAddress.TryCreate(emailAddress, out var mailAddress) && mailAddress.Address == emailAddress;
        }
    }
}