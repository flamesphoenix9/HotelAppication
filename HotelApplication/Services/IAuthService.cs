using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelApplication.Models;
using HotelApplication.Utilities;

namespace HotelApplication.Services
{
    public interface IAuthService
    {
        PasswordValidationResult PasswordChecker(string password);
        string HashPassword(string password, User user);
        bool VerifyPassword(User user, string password);
        string GenerateJsonWebToken(User user);
        bool IsValidEmail(string emailAddress);
    }
}