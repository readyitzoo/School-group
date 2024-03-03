using System.ComponentModel.DataAnnotations.Schema;

namespace Project_ASP_NET.Models
{
    public class MemberRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual bool IsInWait { get; set; }
        public DateTime DateRequested { get; set; }
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }

    }
}
