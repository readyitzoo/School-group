using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_ASP_NET.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(100, ErrorMessage = "Numele nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Numele trebuie sa aiba mai mult de 5 caractere")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descrierea grupului este obligatorie")]
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; }


        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<Membership>? Memberships { get; set; }

        public virtual ICollection<MemberRequest>? MemberRequests { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }




    }
}
