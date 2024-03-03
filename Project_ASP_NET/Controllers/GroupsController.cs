using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Project_ASP_NET.Data;
using Project_ASP_NET.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static Humanizer.On;
using Group = Project_ASP_NET.Models.Group;

namespace Project_ASP_NET.Controllers
{
    struct ParsedUser
    {
        public string UserName;
        public string Id;
        public bool isGroupMod;
    };

    struct ParsedUserReq
    {
        public string UserName;
        public string Id;
        public bool isWaiting;
    };



    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public GroupsController(ApplicationDbContext context)
        {
            _db = context;
        }

        // Show all groups
        public IActionResult Index()
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            // take the current user from the database and the ids of the groups the current user is in
            var user = _db.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userGroupIds = _db.Memberships.Where(ms => ms.UserId == user.Id);

            var userGroups = _db.Groups?.Include("Category").Where(g => userGroupIds.Any(ug => ug.GroupId == g.Id));

            ViewBag.UserGroups = userGroups;
            ViewBag.UserGroupsLength = userGroups.ToArray().Length;

            return View();
        }

        // Show groups that the current user can join
        public IActionResult Other()
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            var user = _db.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userGroupIds = _db.Memberships.Where(ms => ms.UserId == user.Id);

            var otherGroups = _db.Groups?.Include("Category").Where(g => !userGroupIds.Any(ug => ug.GroupId == g.Id));

            var groupsIn = _db.MemberRequests.Where(rs => rs.UserId == user.Id);
            ViewBag.OtherGroups = otherGroups;
            ViewBag.OtherGroupsLength = otherGroups?.Count();
            ViewBag.GroupsIn = groupsIn;
            ViewBag.User = user.Id;
            ViewBag.Lengthreq = groupsIn.Count();
            ViewBag.IsAdmin = User.IsInRole("Admin");

            return View();
        }

        public IActionResult SearchOther()
        {
            var search = "";
         

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                

            }

            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            var user = _db.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userGroupIds = _db.Memberships.Where(ms => ms.UserId == user.Id);

            var otherGroups = _db.Groups?.Include("Category").Where(g => !userGroupIds.Any(ug => ug.GroupId == g.Id) && g.Name.Contains(search));

            var groupsIn = _db.MemberRequests.Where(rs => rs.UserId == user.Id);
            ViewBag.OtherGroups = otherGroups;
            ViewBag.OtherGroupsLength = otherGroups?.Count();
            ViewBag.GroupsIn = groupsIn;
            ViewBag.User = user.Id;
            ViewBag.Lengthreq = groupsIn.Count();
            ViewBag.IsAdmin = User.IsInRole("Admin");



            ViewBag.SearchString = search;



            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Groups/SearchOther/?search=" + search;
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Groups/SearchOther";
            }

            return View();

        }

        // Show a group
        public IActionResult Show(int? id)
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            if (id == null)
            {
                return View("Index");
            }

            // get the group with given id
            Models.Group? group = _db.Groups?.Include("Category").Include("Messages").Where(grp => grp.Id == id).First();

            // get all users from the current group
            var userGroupIds = _db.Memberships.Where(ms => ms.GroupId == id);

            var userGroupIdsReq = _db.MemberRequests.Where(ms => ms.GroupId == id);

            // get all users knowing their id
            var users = _db.Users?.Include("Memberships").Where(usr => userGroupIds.Any(ug => ug.UserId == usr.Id));
            var parsedUsers = new List<ParsedUser>();

            foreach (var usr in users)
            {
                var parsedUser = new ParsedUser
                {
                    UserName = usr.UserName,
                    Id = usr.Id,
                    isGroupMod = _db.Memberships.Where(ms => ms.GroupId == id && ms.UserId == usr.Id).First().IsModerator,
                };
                parsedUsers.Add(parsedUser);
            }
            var currentUser = users.Where(usr => usr.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value).First();

            

            ViewBag.Group = group;
            ViewBag.Category = group?.Category;
            ViewBag.Messages = group?.Messages;
            ViewBag.Members = parsedUsers;
            ViewBag.MembersCount = users.Count();
            ViewBag.CurrentUser = currentUser;
            ViewBag.IsModOrAdmin = User.IsInRole("Admin") || _db.Memberships.Where(ms => ms.UserId == currentUser.Id && ms.GroupId == id).First().IsModerator;


            var users_req = _db.Users?.Include("MemberRequests").Where(usr => userGroupIdsReq.Any(ug => ug.UserId == usr.Id));
            var parsedUsersReq = new List<ParsedUserReq>();

            foreach (var usr in users_req)
            {
                var parsedUserReq = new ParsedUserReq
                {
                    UserName = usr.UserName,
                    Id = usr.Id,
                    isWaiting = true
                };
                parsedUsersReq.Add(parsedUserReq);
            }

            ViewBag.UsersReq= parsedUsersReq;
            ViewBag.UserReqLen = parsedUsersReq.Count();

            return View();
        }

        // Delete a group
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Models.Group? group = _db.Groups?.Find(id);

            if (group != null)
            {
                var messages = _db.Messages.Where(msg => msg.GroupId == id).ToList();
                var memberships = _db.Memberships.Where(ms => ms.GroupId == id).ToList();
                _db.Groups?.Remove(group);

                if (messages != null)
                {
                    foreach (Message message in messages)
                    {
                        _db.Messages?.Remove(message);
                    }

                    foreach (Membership membership in memberships)
                    {
                        _db.Memberships?.Remove(membership);
                    }
                }

                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Show add form ( also select category )
        public IActionResult New()
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            //List<Category> categories = _db.Categories.ToList();

            //ViewBag.Categories = categories;

            //return View();

            Group group = new Group();
            group.Categ= GetAllCategories();
            return View(group);
        }

        // Add a group
        [HttpPost]
        public IActionResult New(Group group)
        {
            
                group.DateCreated = DateTime.UtcNow;
                group.Categ = GetAllCategories();
                _db.Groups.Add(group);
                

                

                if (ModelState.IsValid)
                {
                    _db.Groups.Add(group);
                    _db.SaveChanges();

                    Membership membership = new Membership();
                    membership.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    membership.GroupId = group.Id;
                    membership.DateJoined = DateTime.UtcNow;
                    membership.IsModerator = true;
                    _db.Memberships.Add(membership);
                    _db.SaveChanges();
                    return RedirectToAction("Index");

                }
               


                return View(group);
            
        }

        // Show edit form
        public IActionResult Edit(int? id)
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            if (id == null)
            {
                return View("Index");
            }

            Group group = _db.Groups?.Include("Category").Where(grp => grp.Id == id).First();
            List<Category> categories = _db.Categories.ToList();
            group.Categ = GetAllCategories();

            ViewBag.Group = group;
            ViewBag.Category = group?.Category;
            ViewBag.Categories = categories;

            return View(group);
        }

        
        // Edit a group
        [HttpPost]
        public IActionResult Edit(int id, Models.Group editedGroup)
        {
            //Models.Group? group = _db.Groups.Find(id);

            //if (group == null)
            //{
            //    return RedirectToAction("Edit", id);
            //}

            //try
            //{
            //    group.Name = editedGroup.Name;
            //    group.Description = editedGroup.Description;
            //    group.CategoryId = editedGroup.CategoryId;

              

            //    _db.SaveChanges();

            //    return RedirectToAction("Index");
            //}
            //catch (Exception)
            //{
            //    return RedirectToAction("Edit", id);
            //}

            Group group = _db.Groups.Find(id);
            editedGroup.Categ = GetAllCategories();

            if (ModelState.IsValid)
            {
                group.Name = editedGroup.Name;
                group.Description = editedGroup.Description;
                group.CategoryId = editedGroup.CategoryId;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(editedGroup);
            }
        }

        // Leave a group
        [HttpPost]
        public IActionResult Leave(int id)
        {
            var user = _db.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var membership = _db.Memberships.Where(ms => ms.UserId == user.Id && ms.GroupId == id).First();

            _db.Memberships.Remove(membership);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Join a group
        [HttpPost]
        public IActionResult Join(int id)
        {
            var user = _db.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var memberrequest = new MemberRequest();

            memberrequest.UserId = user.Id;
            memberrequest.GroupId = id;
            memberrequest.DateRequested = DateTime.UtcNow;
            memberrequest.IsInWait = true;

            _db.MemberRequests.Add(memberrequest);
            _db.SaveChanges();

            return RedirectToAction("Other", "Groups");
        }

        // Join Admin in all groups
        [HttpPost]
        public IActionResult JoinAdmin(int id)
        {
            var user = _db.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var membership = new Membership();

            membership.UserId = user.Id;
            membership.GroupId = id;
            membership.DateJoined = DateTime.UtcNow;
            membership.IsModerator = false;

            _db.Memberships.Add(membership);
            _db.SaveChanges();

            return RedirectToAction("Show", "Groups", new { id });
        }


        public IActionResult Cancel(int id)
        {
            var user = _db.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var memberrequest = _db.MemberRequests.Where(ms => ms.UserId == user.Id && ms.GroupId == id);

            foreach(var mrq in memberrequest)
            {
                _db.MemberRequests.Remove(mrq);
                
            }
            _db.SaveChanges();



            return RedirectToAction("Other", "Groups");
        }

        // Add user as mod
        [HttpPost]
        public IActionResult AddMod(int groupId, string userId)
        {
            var membership = _db.Memberships.Where(ms => ms.GroupId == groupId && ms.UserId == userId).First();
            membership.IsModerator = true;

            _db.SaveChanges();

            return RedirectToAction("Show", "Groups", new { id = groupId });
        }

        // Remove user as mod
        [HttpPost]
        public IActionResult RemoveMod(int groupId, string userId)
        {
            var membership = _db.Memberships.Where(ms => ms.GroupId == groupId && ms.UserId == userId).First();
            membership.IsModerator = false;

            _db.SaveChanges();

            return RedirectToAction("Show", "Groups", new { id = groupId });
        }

        [HttpPost]
        public IActionResult RemoveUsr(int groupId, string userId)
        {
            var membership = _db.Memberships.Where(ms => ms.GroupId == groupId && ms.UserId == userId).First();
            membership.IsModerator = false;

            _db.Memberships.Remove(membership);
            _db.SaveChanges();

            return RedirectToAction("Show", "Groups", new { id = groupId });
        }

        [HttpPost]

        public IActionResult Accept(int groupId, string userId)
        {
            var mb = new Membership();
            mb.DateJoined = DateTime.UtcNow;
            mb.UserId = userId;
            mb.GroupId = groupId;
            mb.IsModerator = false;

            _db.Memberships.Add(mb);

            var req = _db.MemberRequests.Where(rq => rq.GroupId == groupId && rq.UserId == userId).First();
            _db.MemberRequests.Remove(req);
            _db.SaveChanges();

            return RedirectToAction("Show", "Groups", new { id = groupId });
        }


        public IActionResult Reject(int groupId, string userId)
        {

            var req = _db.MemberRequests.Where(rq => rq.GroupId == groupId && rq.UserId == userId).First();
            _db.MemberRequests.Remove(req);
            _db.SaveChanges();

            return RedirectToAction("Show", "Groups", new { id = groupId });
        }


        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in _db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name.ToString()
                });
            }
            /* Sau se poate implementa astfel: 
             * 
            foreach (var category in categories)
            {
                var listItem = new SelectListItem();
                listItem.Value = category.Id.ToString();
                listItem.Text = category.CategoryName.ToString();

                selectList.Add(listItem);
             }*/


            // returnam lista de categorii
            return selectList;
        }
    }
}
