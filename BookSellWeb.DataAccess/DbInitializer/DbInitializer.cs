using BookEcomWeb.DataAccess.Data;
using BookEcomWeb.Models;
using BookEcomWeb.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEcomWeb.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext dbContext;

        public DbInitializer(UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
        }

        public void Initialize()
        {
            // migrations if not applied
            try
            {
                if (dbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    dbContext.Database.Migrate();
                }
            } catch (Exception ex)
            {
                // log error
                Console.WriteLine(ex.Message);
            }
            // create roles if they do not exist
            if (!roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                // if roles do not exist, create them
                userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Name = "Admin",
                    Email = "Admin@gmail.com",
                    PhoneNumber = "1234567890",
                    StreetAddress = "123 Admin St",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                }, "Abc@123").GetAwaiter().GetResult();

                ApplicationUser user = dbContext.ApplicationUsers.FirstOrDefault(u => u.Email == "Admin@gmail.com");
                userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;
        }
    }
}
