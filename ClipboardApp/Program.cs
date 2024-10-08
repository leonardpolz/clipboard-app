using System.Security.Claims;
using System.Text;
using ClipboardApp.Handlers.AuthGuestHandler;
using ClipboardApp.Handlers.GetBlobDownloadContextHandler;
using ClipboardApp.Handlers.GetBlobUploadContextHandler;
using ClipboardApp.Handlers.GetTextHandler;
using ClipboardApp.Handlers.GetWsFileNameHandler;
using ClipboardApp.Handlers.GetWsTextHandler;
using ClipboardApp.Handlers.SetTextClipboardHandler;
using ClipboardApp.Shared.Authentication;
using ClipboardApp.Shared.BinaryFileClient;
using ClipboardApp.Shared.GeneralSettings;
using ClipboardApp.Shared.TextStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.IdentityModel.Tokens;

Console.WriteLine("Environment: " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddWebSockets(_ => { });
builder.Services.AddOpenApiDocument();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT_SECRET environment variable is not set.");
}

builder.Services.PostConfigure<JwtOptions>(options => { options.JwtSecret = jwtSecret; });

builder.Services.Configure<BinaryFileClientOptions>(builder.Configuration.GetSection("BinaryFileClientOptions"));
builder.Services.PostConfigure<BinaryFileClientOptions>(options =>
{
    var azureClientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
    if (string.IsNullOrEmpty(azureClientSecret) &&
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
    {
        throw new InvalidOperationException("AZURE_CLIENT_SECRET environment variable is not set.");
    }

    options.ClientSecret = azureClientSecret;

    var azureStorageAccountKey = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");
    if (string.IsNullOrEmpty(azureStorageAccountKey) &&
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
    {
        throw new InvalidOperationException("AZURE_STORAGE_ACCOUNT_KEY environment variable is not set.");
    }

    options.StorageAccountKey = azureStorageAccountKey;
});

builder.Services.Configure<ClipboardSessionOptions>(builder.Configuration.GetSection("ClipboardSessionOptions"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
            ValidAudience = builder.Configuration["JwtOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity claimsIdentity)
                {
                    context.Fail("No ClaimsIdentity");
                    return Task.CompletedTask;
                }
        
                var sessionIdClaim = claimsIdentity.FindFirst("sessionId");
        
                if (sessionIdClaim != null && !string.IsNullOrEmpty(sessionIdClaim.Value)) return Task.CompletedTask;
        
                context.Fail("The sessionId claim is missing or empty.");
                return Task.CompletedTask;
            }
        };
    });

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

builder.Services.AddSingleton<TextStorage>();
builder.Services.AddSingleton<FileNameStorage>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

builder.Services.AddTransient<IGetTextHandler, GetTextHandler>();
builder.Services.AddTransient<ISetTextClipboardHandler, SetTextClipboardHandler>();
builder.Services.AddTransient<IWsGetTextHandler, WsGetTextHandler>();
builder.Services.AddTransient<IWsGetFileNameHandler, WsGetFileNameHandler>();
builder.Services.AddTransient<IBinaryFileClient, BinaryFileClient>();
builder.Services.AddTransient<IAuthGuestHandler, AuthGuestHandler>();
builder.Services.AddTransient<IGetBlobDownloadContextHandler, GetBlobDownloadContextHandler>();
builder.Services.AddTransient<IGetBlobUploadContextHandler, GetBlobUploadContextHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
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