using UNP.Data;
using UNP.Models;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using Microsoft.AspNetCore.Identity;
using UNP.Services;
using UNP.Tasks;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

// Add services to the container.
builder.Services.AddControllersWithViews();
 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUserModel, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var checkUnpJobKey = new JobKey("CheckUnpJob");
    q.AddJob<CheckUnpJob>(opts => opts.WithIdentity(checkUnpJobKey));
    q.AddTrigger(opts => opts
        .ForJob(checkUnpJobKey)
        .WithIdentity("CheckUnpJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInHours(8)
            .RepeatForever()));

    var sendEmailJobKey = new JobKey("SendEmailJob");
    q.AddJob<SendEmailJob>(opts => opts.WithIdentity(sendEmailJobKey));
    q.AddTrigger(opts => opts
        .ForJob(sendEmailJobKey)
        .WithIdentity("emailSendingJob-trigger")
        .StartAt(DateBuilder.DateOf(7, 00, 00))
        .WithSimpleSchedule(x => x
            .WithIntervalInHours(24)
            .RepeatForever())); ;
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
