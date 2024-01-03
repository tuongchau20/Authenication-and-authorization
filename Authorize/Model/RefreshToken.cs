using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authorize.Model
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }  
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime IssueAt { get; set; }

        public DateTime ExpiredAt { get; set; }


    }
}
