using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Schematics.API.Data;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.Service;
using Schematics.API.Service.Infrastructure;
using Serilog;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILineService, LineService>();
builder.Services.AddScoped<ISchemaService, SchemaService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<ILineRepository, LineRepository>();
builder.Services.AddScoped<ISchamaRepository, SchemaRepository>();
builder.Services.AddScoped<IStationRepository, StationRepository>();
builder.Services.AddScoped<ISharedSchemaRepository, SharedSchemaRepository>();
builder.Services.AddScoped<ISharedSchemaService, SharedSchemaService>();
builder.Services.AddScoped<IStationLineRepository, StationLineRepository>();
builder.Services.AddScoped<ILineCategoryRepository, LineCategoryRepository>();
builder.Services.AddScoped<ISchemaStatisticsRepository, SchemaStatisticsRepository>();
builder.Services.AddScoped<ISchemaStatisticsService, SchemaStatisticsService>();
builder.Services.AddScoped<ILogService, LogService>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSSQL"));
});


builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services
    .AddAuthentication()
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];

        options.Scope.Add("email");
        options.Fields.Add("email");
        options.Fields.Add("name");

        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    });

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/logs-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (db.Database.GetPendingMigrations().Any())
        db.Database.Migrate();

    if (!db.Roles.Any(x => x.Name == "Admin"))
    {
        await db.Roles.AddAsync(new IdentityRole
        {
            Id = "801f7026-3d45-4b6e-8c23-509eedb55be2",
            ConcurrencyStamp = "NULL",
            Name = "Admin",
            NormalizedName = "ADMIN"
        });
        await db.SaveChangesAsync();
    }
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@localhost.pl";
    string adminPassword = "Test1234!"; 

   
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Firstname = "Admin",   
            Lastname = "Systemowy",
            
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);

        if (!createResult.Succeeded)
        {
            Console.WriteLine("B³¹d tworzenia admina:");
            foreach (var error in createResult.Errors)
                Console.WriteLine(error.Description);
        }
        else
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                await userManager.AddToRoleAsync(adminUser, "Admin");

            Console.WriteLine($"Utworzono admina: {adminEmail}");
        }
    }
    else
    {
     
        var needUpdate = false;
        if (string.IsNullOrWhiteSpace(adminUser.Firstname)) { adminUser.Firstname = "Admin"; needUpdate = true; }
        if (string.IsNullOrWhiteSpace(adminUser.Lastname)) { adminUser.Lastname = "Admin"; needUpdate = true; }

        if (needUpdate)
        {
            var updateResult = await userManager.UpdateAsync(adminUser);
            if (!updateResult.Succeeded)
            {
                Console.WriteLine("B³¹d aktualizacji admina:");
                foreach (var error in updateResult.Errors)
                    Console.WriteLine(error.Description);
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();