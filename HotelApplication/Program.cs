using System.Net.Http.Headers; // paystack
using System.Text;
using HotelApplication.Data;
using HotelApplication.Models;
using HotelApplication.Repos;
using HotelApplication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog; //   logging


//  SERILOG

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information("\n \n ===  Starting application");

// Add services to the container.
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());
// Bacground Service
builder.Services.AddHostedService<ExpiredReservationGroupCleanupWorker>();
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
//  JWT Authentication Scheme
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["AuthSettings:Issuer"],
        ValidAudience = builder.Configuration["AuthSettings:Audience"],
        ValidateAudience = true,
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:JWTKey"] ?? throw new ArgumentException("AuthSettings:JWTKey is not provided in condiguration")))
    };
});

//  Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("front-desk", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(nameof(Roles.Admin),
                            nameof(Roles.Manager),
                            nameof(Roles.Receptionist));
    });
    options.AddPolicy("management", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(nameof(Roles.Admin),
                            nameof(Roles.Manager));
    });
    options.AddPolicy("staff", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(nameof(Roles.Admin),
                            nameof(Roles.Manager),
                            nameof(Roles.Housekeepers),
                            nameof(Roles.Receptionist));
    });
    options.AddPolicy("housekeeping", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(nameof(Roles.Housekeepers),
                            nameof(Roles.Manager),
                            nameof(Roles.Admin));
    });
});
//  Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // <--- Standard header name
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your_token}"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});
//  Serialize Enum
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });
//  Auto Mapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
//  Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IReservationGroupService, ReservationGroupService>();
builder.Services.AddScoped<IReservationGroupRepo, ReservationGroupRepo>();
builder.Services.AddScoped<IReservationRepo, ReservationRepo>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomRepo, RoomRepo>();
builder.Services.AddScoped<IRoomDateAllocationRepo, RoomDateAllocationRepo>();
builder.Services.AddScoped<IRoomTypePriceRepo, RoomTypePriceRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//  ===   PAYSTACK
builder.Services.AddHttpClient<IPaymentService, PaystackPaymentService>(client =>
{
    client.BaseAddress = new Uri("https://api.paystack.co/");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", builder.Configuration["Paystack:SecretKey"]);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
    

var app = builder.Build();
app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Log.Information("=====     DEVELOPMENT       =====");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "HotelsAPI v2");
        options.RoutePrefix = string.Empty;
    });
    //  Seed Database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await DbInitializer.SeedDatabaseAsync(services);
    }

}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


Log.Information("==========         Server starting              ==========");
app.Run();