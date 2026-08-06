using System.Collections.ObjectModel;
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
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(EmailAddress), IsUnique = true)]
    [Index(nameof(Normalised_Email), IsUnique = true)]
    [Index(nameof(Normalised_Username),IsUnique =true)]
    public class User : EntityBase
    {
        private string _emailAddress = string.Empty;
        private string _userName = string.Empty;

        [Required]
        public string EmailAddress
        {
            get => _emailAddress; set
            {
                _emailAddress = value;
                Normalised_Email = value?.Trim().ToUpperInvariant() ?? string.Empty;
            }
        }
        [Required]
        public string Normalised_Email { get; private set; } = string.Empty;
        [Required]
        public string Username { get=>_userName; set{
                _userName = value;
                Normalised_Username = value?.Trim().ToUpperInvariant() ?? string.Empty;
            } } 
        [Required]
        public string Normalised_Username { get; private set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public Roles Role { get; set; } = Roles.Customer;

        public string FullName => $"{FirstName} {LastName}".Trim();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<CheckIn> CheckIns { get; set; } = new List<CheckIn>();
    }
}