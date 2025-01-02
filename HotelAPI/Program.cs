using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using HotelAPI.Entities;
using HotelAPI.Seeders;
using Microsoft.Extensions.Hosting;
using HotelAPI.JWTstuff;
using HotelAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Entity Framework stuff

builder.Services.AddDbContext<HotelDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelsConnectionString"));
});
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddScoped<IHotelSeeder, HotelSeeder>();

// API stuff

builder.Services.AddControllers().AddFluentValidation();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();

var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";

}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins(builder.Configuration["AllowedOrigins"]);
    });
});

// Dependcies injection

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUpdateService, UpdateService>();
builder.Services.AddSingleton(authenticationSettings);

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

builder.Services.AddScoped<IValidator<CreateHotelDto>, CreateHotelDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateHotelDto>, UpdateHotelDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateHotelDto>, UpdateHotelDtoValidator>();
builder.Services.AddScoped<IValidator<HotelQuery>, HotelQueryValidator>();
builder.Services.AddScoped<IValidator<CreateReservationDto>, CreateReservationDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateReservationDto>, UpdateReservationDtoValidator>();
builder.Services.AddScoped<IValidator<CreateRoomDto>, CreateRoomDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateRoomDto>, UpdateRoomDtoValidator>();
builder.Services.AddScoped<IValidator<RoomQuery>, RoomQueryValidator>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedHotelRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedReservationRequirementHandler>();


var app = builder.Build();

app.UseStaticFiles();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Scope for seeder

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
var seeder = scope.ServiceProvider.GetService<IHotelSeeder>();
seeder.Seed(dbContext);
var updater = scope.ServiceProvider.GetService<IUpdateService>();
updater.Update(dbContext);

app.Run();
