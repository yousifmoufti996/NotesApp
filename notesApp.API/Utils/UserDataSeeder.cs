using notesApp.API.Data;
using notesApp.API.Models.DomainModels;
using Microsoft.AspNetCore.Identity;

namespace notesApp.API.Utils

{
    public class UserDataSeeder
    {
       

        public static void SeedUsers(UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync("user1").Result == null)
            {
                User user1 = new User
                {
                   
                    Email = "user1@example.com",
                
                    // Add any other user properties here
                };

    
                IdentityResult result = userManager.CreateAsync(user1, "Password@123").Result;

            }

            if (userManager.FindByNameAsync("user2").Result == null)
            {
                User user2 = new User
                {
                   
                    Email = "user2@example.com",
             
                    // Add any other user properties here
                };

                IdentityResult result = userManager.CreateAsync(user2, "Password@123").Result;

              
            }
        }
    }

}
