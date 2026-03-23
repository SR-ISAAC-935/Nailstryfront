using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nails_trail.Data;
using nails_trail.Models;
using System.Security.Claims;


namespace nails_trail {

	public class Program
	{
		public static async Task Main(String[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();
			builder.Services.AddDbContext<NailstryContext>(options =>
				options.UseSqlServer(connectionString));

			builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
				options.SignIn.RequireConfirmedEmail = false;
			}).AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>();


			builder.Services
				.AddAuthentication()
				.AddGoogle(options =>
				{
					options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
					options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
					options.ClaimsIssuer = builder.Configuration["Authentication:Google:ClaimsIssuer"];

					options.Scope.Add("profile");
					options.Scope.Add("email");

					options.Events.OnCreatingTicket = context =>
					{
						var firstName = context.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
						var lastName = context.Principal.FindFirst(ClaimTypes.Surname)?.Value;
						var fullName = context.Principal.FindFirst(ClaimTypes.Name)?.Value;

						if (!string.IsNullOrEmpty(fullName))
							context.Identity.AddClaim(new Claim("FullName", fullName));

						if (!string.IsNullOrEmpty(firstName))
							context.Identity.AddClaim(new Claim("FirstName", firstName));

						if (!string.IsNullOrEmpty(lastName))
							context.Identity.AddClaim(new Claim("LastName", lastName));

						return Task.CompletedTask;
					};
				});



			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseRouting();

			app.UseCookiePolicy();   // ✔ Debe ir aquí

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapStaticAssets();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}")
				.WithStaticAssets();

			
			app.MapRazorPages()
			   .WithStaticAssets();

			using (var scope = app.Services.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				var roles = new[] { "Admin", "User" };

				foreach (var role in roles)
				{
					if (!await roleManager.RoleExistsAsync(role))
					{
						await roleManager.CreateAsync(new IdentityRole(role));
					}
				}
			}

			using (var scope = app.Services.CreateScope())
			{
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

				string email = "admin@admin.admin.com";
				string password= "Admin123*";
				if (await userManager.FindByEmailAsync(email) == null)
				{
					var user = new ApplicationUser();
					{
						user.UserName = email;
						user.Email = email;
					};
					var result = await userManager.CreateAsync(user, password);

					await userManager.AddToRoleAsync(user, "Admin");
				}
			}
			app.Run();


		}
	}

}