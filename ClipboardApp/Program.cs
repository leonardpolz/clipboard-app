using System.Text;
using ClipboardApp.AutoMapper;
using ClipboardApp.Handlers;
using ClipboardApp.Shared.Authentication;
using ClipboardApp.Shared.BinaryFileClient;
using ClipboardApp.Shared.TextStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.IdentityModel.Tokens;

Console.WriteLine("Environment: " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddWebSockets(_ => { });

builder.Services.AddAutoMapper(typeof(ClipboardProfile));

builder.Services.AddSingleton<TextStorage>();
builder.Services.AddSingleton<FileNameStorage>();

builder.Services.AddTransient<IGetTextHandler, GetTextHandler>();
builder.Services.AddTransient<ISetTextClipboardHandler, SetTextClipboardHandler>();
builder.Services.AddTransient<IWsGetTextHandler, WsGetTextHandler>();
builder.Services.AddTransient<IWsGetFileNameHandler, WsGetFileNameHandler>();

builder.Services.Configure<BinaryFileClientOptions>(builder.Configuration.GetSection("BinaryFileClient"));
builder.Services.PostConfigure<BinaryFileClientOptions>(options =>
{
    var azureClientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
    if (string.IsNullOrEmpty(azureClientSecret) && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
    {
        throw new InvalidOperationException("AZURE_CLIENT_SECRET environment variable is not set.");
    }
    options.ClientSecret = azureClientSecret;
});

builder.Services.AddTransient<IBinaryFileClient, BinaryFileClient>();

builder.Services.AddOpenApiDocument();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsPolicyBuilder =>
        {
            corsPolicyBuilder
                .AllowAnyOrigin() 
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition");
        });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
if (string.IsNullOrEmpty(jwtSecret))
{ 
    throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
}
builder.Services.PostConfigure<JwtOptions>(options =>
{
    options.JwtSecret = jwtSecret;
});

builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
builder.Services.AddTransient<IAuthGuestHandler, AuthGuestHandler>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddTransient<IGetBinaryFileHandler, GetBinaryFileHandler>();
builder.Services.AddTransient<ISetBinaryFileHandler, SetBinaryFileHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseWebSockets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.UseOpenApi();
app.UseSwaggerUi();

app.Run();