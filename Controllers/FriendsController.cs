using IslamicCompanion.Data;
using IslamicCompanion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IslamicCompanion.Controllers
{
    // Require users to be logged in to access this page
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FriendsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- HELPER TO GET THE LOGGED IN USER ---
        private AppUser GetCurrentUser()
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        // ------------------------------------------------
        // 1. VIEW FRIENDS & PENDING REQUESTS
        // ------------------------------------------------
        public IActionResult Community()
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null) return RedirectToAction("Login", "Account"); // Redirect if not logged in

            // Get accepted friends (Status == 1). We check both Requester and Receiver sides.
            var friendships = _context.Friendships
                .Include(f => f.Requester)
                .Include(f => f.Receiver)
                .Where(f => (f.RequesterId == currentUser.Id || f.ReceiverId == currentUser.Id) && f.Status == 1)
                .ToList();

            // Get pending requests where this user is the RECEIVER (Status == 0)
            var pendingRequests = _context.Friendships
                .Include(f => f.Requester)
                .Where(f => f.ReceiverId == currentUser.Id && f.Status == 0)
                .ToList();

            ViewBag.PendingRequests = pendingRequests;
            ViewBag.CurrentUserId = currentUser.Id;

            int currentUserId = currentUser.Id;
            foreach (var friendship in friendships)
            {
                // Identify which user is the friend (not you)
                var friend = (friendship.RequesterId == currentUserId)
                    ? friendship.Receiver
                    : friendship.Requester;

                // Run the exact same query you used in the Profile controller!
                friend.TotalDoneTasks = _context.Activities.Count(t =>
                    t.AppUserId == friend.Id &&
                    t.IsCompleted);
            }

            return View(friendships);
        }

        // ------------------------------------------------
        // 2. SEND FRIEND REQUEST
        // ------------------------------------------------
        [HttpPost]
        public IActionResult SendRequest(string targetUsername)
        {
            var currentUser = GetCurrentUser();
            var targetUser = _context.Users.FirstOrDefault(u => u.Username == targetUsername);

            if (targetUser == null || currentUser == null)
            {
                TempData["Error"] = "User not found!";
                return RedirectToAction("Community");
            }

            if (targetUser.Id == currentUser.Id)
            {
                TempData["Error"] = "You cannot add yourself!";
                return RedirectToAction("Community");
            }

            // Check if a request already exists (either pending or accepted)
            bool exists = _context.Friendships.Any(f =>
                (f.RequesterId == currentUser.Id && f.ReceiverId == targetUser.Id) ||
                (f.RequesterId == targetUser.Id && f.ReceiverId == currentUser.Id));

            if (!exists)
            {
                _context.Friendships.Add(new Friendship
                {
                    RequesterId = currentUser.Id,
                    ReceiverId = targetUser.Id,
                    Status = 0 // 0 = Pending Request
                });
                _context.SaveChanges();
                TempData["Success"] = "Friend request sent to " + targetUser.DisplayName + "!";
            }
            else
            {
                TempData["Error"] = "Request already sent or you are already friends.";
            }

            return RedirectToAction("Community");
        }

        // ------------------------------------------------
        // 3. ACCEPT REQUEST
        // ------------------------------------------------
        [HttpPost]
        public IActionResult AcceptRequest(int friendshipId)
        {
            var currentUser = GetCurrentUser();
            var request = _context.Friendships.FirstOrDefault(f => f.Id == friendshipId && f.ReceiverId == currentUser.Id);

            if (request != null)
            {
                request.Status = 1; // 1 = Accepted
                _context.SaveChanges();
                TempData["Success"] = "Friend added!";
            }

            return RedirectToAction("Community");
        }

        // ------------------------------------------------
        // 4. DECLINE OR REMOVE FRIEND
        // ------------------------------------------------
        [HttpPost]
        public IActionResult RemoveFriend(int friendshipId)
        {
            var currentUser = GetCurrentUser();

            // Allows removing if you are either the sender or receiver
            var request = _context.Friendships.FirstOrDefault(f =>
                f.Id == friendshipId &&
                (f.ReceiverId == currentUser.Id || f.RequesterId == currentUser.Id));

            if (request != null)
            {
                _context.Friendships.Remove(request);
                _context.SaveChanges();
                TempData["Success"] = "Friend removed.";
            }

            return RedirectToAction("Community");
        }
    }
}