using System.ComponentModel.DataAnnotations.Schema;

namespace Project_ASP_NET.Models
{
    public class Membership
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual bool IsModerator { get; set; }
        public DateTime DateJoined { get; set; }
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }

    }
}
