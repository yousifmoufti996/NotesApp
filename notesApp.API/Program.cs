using Microsoft.EntityFrameworkCore;
using notesApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using notesApp.API.Models.DomainModels;
using Microsoft.AspNetCore.Identity;
using notesApp.API.Utils;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddIdentity<User, IdentityRole>(options =>
{
   
    options.Password.RequireLowercase = true;
   
    
})
.AddEntityFrameworkStores<NotesDBCon>() // Replace with your actual DbContext
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EncreptionClass.DecryptContent(builder.Configuration["Jwt:Key"]))) // Customize this
                    };
                });



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPostman", builder =>
    {
        builder.WithOrigins("https://localhost:7002"); // Replace with the correct origin(s)
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "NotesApp ",
        Description = "This .NET Core 6 RESTful API provides user resource management," +
        " allowing authenticated users to create, read, update, and delete their own resources (e.g., 'Tasks' or 'Notes'). " +
        "It uses token-based authentication (JWT) to secure the API, ensuring that users can only perform CRUD operations on their own resources. " +
        "For simplicity, an in-memory database is employed." +
        "",

    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


builder.Services.AddDbContext<NotesDBCon>(options =>
{
    options.UseInMemoryDatabase("notesdb");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowPostman");


app.MapControllers();

app.Run();
