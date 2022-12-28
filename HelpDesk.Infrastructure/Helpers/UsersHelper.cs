using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Enums;
using HelpDesk.Domain.Options;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HelpDesk.Infrastructure.Helpers
{
    public static class UsersHelper
    {
        public static async Task AddGlobalAdminIfNotExists(this AppDbContext db, GlobalAdminOptions options)
        {
            var globalAdmin = await db.Users.FirstOrDefaultAsync(x => x.UserRole == UserRole.GlobalAdmin && !x.IsDeleted);

            if (globalAdmin != null)
            {
                return;
            }

            globalAdmin = new User
            {
                PasswordHash = ComputeSha256Hash(options.Password),
                FirstName = options.FirstName,
                LastName = options.LastName,
                Username = options.Username,
                UserRole = UserRole.GlobalAdmin
            };

            await db.Users.AddAsync(globalAdmin);
            await db.SaveChangesAsync();
        }

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (var sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
