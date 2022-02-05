using HotelManager.Core;
using HotelManager.Data;
using HotelManager.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManager.API
{
    public class DatabaseSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                var _roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                using (var context = new ApplicationDbContext(
                    serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
                {

                    // create admin user and role if none exists
                    if (await _roleManager.FindByNameAsync(AppConstant.SuperAdminRole) == null)
                    {
                        string adminEmail = "superadmin@hotelmanager.com";
                        string adminPassword = "Abc123*";

                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.SuperAdminRole));
                        var user = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                        var result = await _userManager.CreateAsync(user, adminPassword);
                        if (!result.Succeeded)
                            throw new Exception();
                        await _userManager.AddToRoleAsync(user, AppConstant.SuperAdminRole);
                    }

                    // create business role for company users
                    if (await _roleManager.FindByNameAsync(AppConstant.GuestUserRole) == null)
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.GuestUserRole));

                    await context.SaveChangesAsync();

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
