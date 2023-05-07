using SA4102CASoftware.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Add our database context (using AddScoped() under the hood).
builder.Services.AddDbContext<MyDbContext>(options => {
	var conn_str = builder.Configuration.GetConnectionString("conn_str");
	options.UseLazyLoadingProxies().UseSqlServer(conn_str);
});

var app = builder.Build();

app.UseSession();

app.Use(async (HttpContext, next) =>
{

    // Check if the SessionId key has been set already, if not set set to false as user has not logged in
    if (HttpContext.Session.GetString("LoggedIn") == null)
    {
        // Set the SessionId key to "false".
        HttpContext.Session.SetString("LoggedIn", "false");
    }

    // Call the next middleware in the pipeline.
    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");


// Use this method to delete db and create a new db to populate with data
InitDB(app.Services);

// Use this method to work with data from existing db
//UseExistingDB(app.Services);

app.Run();


// Method 1: delete the previous database and populate with new data
void InitDB(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    MyDbContext db = scope.ServiceProvider.GetRequiredService<MyDbContext>();

    // Delete the previous database
    db.Database.EnsureDeleted();
    // create a new database.
    db.Database.EnsureCreated();
}

// Method 2: Connect to existing database
//void UseExistingDB(IServiceProvider serviceProvider)
//{
//    using var scope = serviceProvider.CreateScope();
//    MyDbContext db = scope.ServiceProvider.GetRequiredService<MyDbContext>();

//    // Use the existing database and migrate
//    db.Database.Migrate();
//}