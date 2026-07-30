using IslamicCompanion.Models;
using IslamicCompanion.Data; // Ensure this matches the namespace of your ApplicationDbContext
using System;
using System.Linq;
using BCrypt.Net;

namespace IslamicCompanion.Services
{
    public class AppAuthService
    {
        private readonly ApplicationDbContext _context;

        // 1. Inject the database context instead of setting a file path
        public AppAuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public AppUser GetUserByUsername(string username)
        {
            // Instantly query the SQL database for this exact username
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public void UpdateUser(AppUser updatedUser)
        {
            // Entity Framework tracks the ID and knows exactly how to overwrite the old record
            _context.Users.Update(updatedUser);
            _context.SaveChanges();
        }

        public AppUser Authenticate(string username, string password)
        {
            // Find the user in the database
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            // Check if they exist and the password matches
            if (user != null && user.PasswordHash == password)
            {
                return user;
            }

            return null; // Login failed
        }

        public bool RegisterUser(AppUser newUser)
        {
            // Check if username is already taken (using ToLower() to ensure it's case-insensitive)
            if (_context.Users.Any(u => u.Username.ToLower() == newUser.Username.ToLower()))
            {
                return false; // Registration fails
            }

            // Add the new user to the database and save the changes
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return true; // Registration succeeds
        }
    }
}