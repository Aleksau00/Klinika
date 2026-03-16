using Klinika.Configuration;
using Klinika.DATA;
using Klinika.Repositories;
using Klinika.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Add services
// ----------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Clinic API", Version = "v1" });

    // Define the Bearer token scheme
    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",           // MUST be lowercase
        BearerFormat = "JWT",
        Description = "JWT Authorization header using Bearer scheme"
    });

    // Register the requirement using the new OpenApiSecuritySchemeReference
    c.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = new List<string>()
        }
    );
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));

// Register repositories
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
builder.Services.AddScoped<IAppointmentSlotRepository, AppointmentSlotRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>(); // NEW

// Register services
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAppointmentSlotService, AppointmentSlotService>();
builder.Services.AddScoped<IClinicService, ClinicService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>(); // NEW
builder.Services.AddScoped<IAppointmentNotificationService, AppointmentNotificationService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IAllergenRepository, AllergenRepository>();
builder.Services.AddScoped<IAllergenService, AllergenService>();
builder.Services.AddScoped<IVaccinationRepository, VaccinationRepository>();
builder.Services.AddScoped<IVaccinationService, VaccinationService>();

// JWT Authentication
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ----------------------------
// Middleware pipeline
// ----------------------------
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
