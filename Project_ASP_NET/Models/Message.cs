using System.ComponentModel.DataAnnotations;


namespace Project_ASP_NET.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul mesajului este obligatoriu")]
        public string Content { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public string? UserId { get; set; }
        public virtual User? User { get; set; }
        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }
    }
}
