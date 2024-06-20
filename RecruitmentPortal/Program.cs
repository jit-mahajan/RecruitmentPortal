using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using RecruitmentPortal.Infrastructure.Data;
using RecruitmentPortal.Infrastructure.Mappings;
using RecruitmentPortal.Services.Email;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingProfile));            

builder.Services.AddControllers()

.AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
 });


ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

            
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var token = context.SecurityToken as JwtSecurityToken;
                if (token != null)
                {
                    var tokenString = token.RawData;
                    var tokenService = context.HttpContext.RequestServices.GetRequiredService<IToken>();
                    if (!tokenService.IsTokenValid(tokenString))
                    {
                        context.Fail("Token has been revoked.");
                    }
                }
                await Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RecruiterPolicy", policy => policy.RequireRole("Recruiter"));
    options.AddPolicy("CandidatePolicy", policy => policy.RequireRole("Candidate"));
});

builder.Services.AddHttpContextAccessor();




builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IJobs, JobService>();
builder.Services.AddScoped<IApplicationForm, ApplicationFormService>();
builder.Services.AddScoped<IEmail, EmailService>();
builder.Services.AddScoped<IToken, TokenService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Recruitment_Portal"),
         sqlOptions =>
         {
             sqlOptions.EnableRetryOnFailure();
         }
    );
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
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
app.MapControllers();

app.Run();
