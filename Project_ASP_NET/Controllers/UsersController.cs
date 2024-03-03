using Microsoft.AspNetCore.Mvc;
using Project_ASP_NET.Data;

namespace Project_ASP_NET.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UsersController(ApplicationDbContext context)
        {
            _db = context;
        }
    }
}
