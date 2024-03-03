using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_ASP_NET.Data;
using Project_ASP_NET.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Project_ASP_NET.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public MessagesController(ApplicationDbContext context)
        {
            _db = context;
        }

        // Add a message
        [HttpPost]
        public IActionResult New(Message message)
        {
            message.DateCreated = DateTime.UtcNow;
            message.DateUpdated = null;
            message.UserId = _db.Users.Find(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)?.Id;
            try
            {
                _db.Messages.Add(message);
                _db.SaveChanges();
                return RedirectToAction("Show", "Groups", new { id = message.GroupId });
            }
            catch (Exception)
            {
                return RedirectToAction("Show", "Groups", new { id = message.GroupId });
            }
        }

        // Delete a message
        public IActionResult Delete(int id)
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            Message? message = _db.Messages.Find(id);
            int groupId = message?.GroupId ?? 0;

            if (message != null)
            {
                _db.Messages.Remove(message);
                _db.SaveChanges();
            }

            return RedirectToAction("Show", "Groups", new { id = groupId });
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

            Message? message = _db.Messages.Find(id);

            ViewBag.Message = message;

            return View();
        }

        // Edit a message
        [HttpPost]
        public IActionResult Edit(int id, Message editedMessage)
        {
            Message? message = _db.Messages.Find(id);

            if (message == null)
            {
                return RedirectToAction("Edit", id);
            }

            try
            {
                message.Content = editedMessage.Content;
                message.DateUpdated = DateTime.UtcNow;
                _db.SaveChanges();

                return RedirectToAction("Show", "Groups", new { id = message.GroupId });
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", id);
            }
        }
    }
}
