using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;


namespace Project_ASP_NET.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<Group>? Groups { get; set; }
        public virtual ICollection<Membership>? Memberships { get; set; }

        public virtual ICollection<MemberRequest>? MemberRequests { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}
