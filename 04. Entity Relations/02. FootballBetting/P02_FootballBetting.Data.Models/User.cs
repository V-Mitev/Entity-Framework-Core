using P02_FootballBetting.Common;
using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models
{
    public class User
    {
        public User()
        {
            Bets = new HashSet<Bet>();    
        }

        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(ValidationConstants.UserNameMaxLength)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(ValidationConstants.PasswordMaxLength)]
        public string Password { get; set; } = null!;

        [Required]
        [MaxLength(ValidationConstants.EmailMaxLength)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(ValidationConstants.NameMaxLength)]
        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

        public virtual ICollection<Bet> Bets { get; set; }
    }
}