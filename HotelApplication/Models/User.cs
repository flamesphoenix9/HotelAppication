using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace HotelApplication.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Roles
    {
        Admin = 1,
        Manager = 2,
        Receptionist = 3,
        Customer = 4
    };
    [Index(nameof(User.EmailAddress), IsUnique = true , Name ="EmailIndex") ]
    public class User : EntityBase
    {
        [Required]
        public string EmailAddress { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public Roles Role { get; set; } = Roles.Customer;

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}