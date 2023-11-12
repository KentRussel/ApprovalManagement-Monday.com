using ApprovalManagement.Controllers;
using ApprovalManagement.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//DI for DbContext
//builder.Services.AddDbContext<ProjectDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));


var app = builder.Build();

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
    pattern: "{controller=MondayProject}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "ApprovalForm",
    pattern: "project/ApprovalForm/{projectId}",
    defaults: new { controller = "Project", action = "ApprovalForm" }
);

app.MapControllerRoute(
    name: "ApprovalForm",
    pattern: "MondayProject/ApprovalForm/{projectId}",
    defaults: new { controller = "Project", action = "ApprovalForm" }
);

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "webhook",
        pattern: "webhook",
        defaults: new { controller = "MondayProject", action = "HandleWebhook" }
    );
});



app.Run();
