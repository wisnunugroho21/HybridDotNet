using General.Data;
using General.Data.Types;
using General.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Common.Services;

public static class ApplicationService
{
    extension(WebApplicationBuilder builder)
    {
        public void AddDatabase()
        {
            builder.Services
                .AddDbContextFactory<AppDbContext>(opt => 
                    opt.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB"))
                        .UseSeeding((db, b) =>
                        {
                            var user = new User
                            {
                                Name = "Admin",
                                Password = "admin",
                                Username = "admin"
                            };

                            var hashPassword = (new PasswordHasher<User>()).HashPassword(user, user.Password);
                            user.Password = hashPassword;

                            db.Set<User>().Add(user);
                            db.SaveChanges();
                        })
                        .UseAsyncSeeding(async (db, b, cancelToken) =>
                        {
                            var user = new User
                            {
                                Name = "Admin",
                                Password = "admin",
                                Username = "admin"
                            };

                            var hashPassword = (new PasswordHasher<User>()).HashPassword(user, user.Password);
                            user.Password = hashPassword;

                            await db.Set<User>().AddAsync(user, cancelToken);
                            await db.SaveChangesAsync(cancelToken);
                        })
                    );
        }
        
        public void AddApplicationService()
        {   
            builder.Services.AddScoped<TodoService>();
        }
    }
}