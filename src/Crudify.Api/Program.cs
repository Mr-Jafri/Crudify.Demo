
using Crudify.Application.Services;
using Crudify.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtConfig").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtConfig"));

Byte[]? key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:SecretKey"]);
TokenValidationParameters? tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{

    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParams;
});

builder.Services.AddSingleton(tokenValidationParams);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddApplicationContext(builder.Configuration);

builder.Services.AddTransient<IApplicationContext, ApplicationContext>();
builder.Services.AddTransient<ILoggingService, LoggingService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IStudentsService, StudentsService>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Crudify-API");
    });
}

app.SeedData();

app.UseMiddleware<ExceptionLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseMiddleware<ActivityLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
