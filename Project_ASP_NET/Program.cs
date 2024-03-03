using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_ASP_NET.Data;
using Project_ASP_NET.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Groups}/{action=Index}/{id?}");
app.MapControllerRoute(name: "groups_other", pattern: "{controller=Groups}/{action=Other}");
app.MapControllerRoute(name: "groups_other_search", pattern: "{controller=Groups}/{action=SearchOther}");
app.MapControllerRoute(name: "groups_new", pattern: "{controller=Groups}/{action=New}");
app.MapControllerRoute(name: "groups_edit", pattern: "{controller=Groups}/{action=Edit}");

app.MapControllerRoute(name: "groups_mod_add",
    pattern: "Groups/AddMod/{groupId}/{userId}",
    defaults: new { controller = "Groups", action = "AddMod" }
);
app.MapControllerRoute(name: "groups_mod_remove",
    pattern: "Groups/RemoveMod/{groupId}/{userId}",
    defaults: new { controller = "Groups", action = "RemoveMod" }
);

app.MapControllerRoute(name: "groups_user_remove",
    pattern: "Groups/RemoveUsr/{groupId}/{userId}",
    defaults: new { controller = "Groups", action = "RemoveUsr" }
);

app.MapControllerRoute(name: "groups_user_accept",
    pattern: "Groups/Accept/{groupId}/{userId}",
    defaults: new { controller = "Groups", action = "Accept" }
);

app.MapControllerRoute(name: "groups_user_reject",
    pattern: "Groups/Reject/{groupId}/{userId}",
    defaults: new { controller = "Groups", action = "Reject" }
);


app.MapControllerRoute(name: "categories", pattern: "{controller=Categories}/{action=Index}/{id?}");
app.MapControllerRoute(name: "categories_new", pattern: "{controller=Categories}/{action=New}");
app.MapControllerRoute(name: "categories_edit", pattern: "{controller=Categories}/{action=Edit}");

app.MapControllerRoute(name: "messages_edit", pattern: "{controller=Messages}/{action=Edit}");

app.MapRazorPages();

app.Run();