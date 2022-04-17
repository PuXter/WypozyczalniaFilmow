using Microsoft.AspNetCore.Identity;

namespace WypozyczalniaFilmow.Data
{
    public class MyIdentityDataInitializer
    {
        public static void SeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Admin"
                };
                _ = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Klient").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Klient"
                };
                _ = roleManager.CreateAsync(role).Result;
            }
        }

        public static void SeedOneUser(UserManager<IdentityUser> userManager, string name, string password,
            string role = null)
        {
            if (userManager.FindByNameAsync(name).Result != null) return;
            IdentityUser user = new IdentityUser
            {
                UserName = name, // musi być taki sam jak email, inaczej nie zadziała
                Email = name
            };
            IdentityResult result = userManager.CreateAsync(user, password).Result;
            if (result.Succeeded && role != null)
            {
                userManager.AddToRoleAsync(user, role).Wait();
            }
        }
        
        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            SeedOneUser(userManager, "adminuser@localhost", "1234Admin!", "Admin");
            SeedOneUser(userManager, "testklient@localhost", "1234Klient!", "Klient");
        }
    }
}