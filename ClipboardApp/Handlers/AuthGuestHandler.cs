using ClipboardApp.Shared.Authentication;
using ClipboardApp.Shared.TextStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace ClipboardApp.Handlers;

public class AuthGuestHandler : ControllerBase, IAuthGuestHandler
{
    private readonly IJwtProvider _jwtProvider;
    private readonly TextStorage _textStorage;
    private readonly IConfiguration _configuration;
    private const int LifetimeHours = 8;
    private const int IdLength = 4;
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public AuthGuestHandler(IJwtProvider jwtProvider, TextStorage textStorage, IConfiguration configuration)
    {
        _jwtProvider = jwtProvider;
        _textStorage = textStorage;
        _configuration = configuration;
    }

    public async Task<IActionResult> HandleAsync(string? sessionId)
    {
        if (sessionId == null || !_textStorage.Storage.ContainsKey(sessionId))
            return Ok(await InitNewSessionAsync(sessionId));
        
        return Ok(await RefreshCurrentSessionAsync(sessionId));
    }

    private async Task<AuthGuestResponseDto> InitNewSessionAsync(string? sessionId)
    {
        var random = new Random();
        if (sessionId == null)
        {
            do
            {
                sessionId = new string(Enumerable.Repeat(Chars, IdLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (_textStorage.Storage.ContainsKey(sessionId));
        }
        
        var response = new AuthGuestResponseDto
        {
            AuthToken = _jwtProvider.Generate(sessionId, LifetimeHours)
        };
        
        _textStorage.Storage.Add(sessionId, string.Empty);

        var cancellationTokenSource = new CancellationTokenSource();

        _textStorage.SessionCleanupCancellationTokens.Add(sessionId, cancellationTokenSource);

        _ = Task.Run(() => SessionCleanupAsync(sessionId, LifetimeHours), cancellationTokenSource.Token);

        return response;
    }


    private async Task<AuthGuestResponseDto> RefreshCurrentSessionAsync(string sessionId)
    {
        var response = new AuthGuestResponseDto
        {
            AuthToken = _jwtProvider.Generate(sessionId, LifetimeHours)
        };

        _textStorage.SessionCleanupCancellationTokens[sessionId].Cancel();

        var cancellationTokenSource = new CancellationTokenSource();

        _textStorage.SessionCleanupCancellationTokens[sessionId] = cancellationTokenSource;

        _ = Task.Run(() => SessionCleanupAsync(sessionId, LifetimeHours), cancellationTokenSource.Token);

        return response;
    }

    private async Task SessionCleanupAsync(string sessionId, int delayHours)
    {
        await Task.Delay(TimeSpan.FromHours(delayHours));
        _textStorage.Storage.Remove(sessionId);
        _textStorage.SessionCleanupCancellationTokens.Remove(sessionId);
    }
}