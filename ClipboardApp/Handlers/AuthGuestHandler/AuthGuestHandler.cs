using ClipboardApp.Shared.Authentication;
using ClipboardApp.Shared.GeneralSettings;
using ClipboardApp.Shared.TextStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ClipboardApp.Handlers.AuthGuestHandler;

public class AuthGuestHandler(
    IJwtProvider jwtProvider,
    TextStorage textStorage,
    IOptions<JwtOptions> jwtOptions,
    IOptions<ClipboardSessionOptions> clipboardSessionOptions
) : ControllerBase, IAuthGuestHandler
{
    private const string IdChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly ClipboardSessionOptions _clipboardSessionOptions = clipboardSessionOptions.Value;

    public async Task<AuthGuestResponseDto> HandleAsync(string? sessionId)
    {
        if (sessionId == null || !textStorage.Storage.ContainsKey(sessionId))
            return await InitNewSessionAsync(sessionId);

        return await RefreshCurrentSessionAsync(sessionId);
    }

    private async Task<AuthGuestResponseDto> InitNewSessionAsync(string? sessionId)
    {
        var random = new Random();
        if (sessionId == null)
        {
            do
            {
                sessionId = new string(Enumerable.Repeat(IdChars, _clipboardSessionOptions.SessionIdLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (textStorage.Storage.ContainsKey(sessionId));
        }

        var response = new AuthGuestResponseDto
        {
            AuthToken = jwtProvider.Generate(sessionId, _jwtOptions.AuthTokenLifetimeInHours)
        };

        textStorage.Storage.Add(sessionId, string.Empty);

        var cancellationTokenSource = new CancellationTokenSource();
        textStorage.SessionCleanupCancellationTokens.Add(sessionId, cancellationTokenSource);
        _ = Task.Run(() => SessionCleanupAsync(sessionId), cancellationTokenSource.Token);

        return response;
    }

    private async Task<AuthGuestResponseDto> RefreshCurrentSessionAsync(string sessionId)
    {
        var response = new AuthGuestResponseDto
        {
            AuthToken = jwtProvider.Generate(sessionId, _jwtOptions.AuthTokenLifetimeInHours)
        };

        await textStorage.SessionCleanupCancellationTokens[sessionId].CancelAsync();

        var cancellationTokenSource = new CancellationTokenSource();

        textStorage.SessionCleanupCancellationTokens[sessionId] = cancellationTokenSource;

        _ = Task.Run(() => SessionCleanupAsync(sessionId), cancellationTokenSource.Token);

        return response;
    }

    private async Task SessionCleanupAsync(string sessionId)
    {
        await Task.Delay(TimeSpan.FromHours(_jwtOptions.AuthTokenLifetimeInHours));
        textStorage.Storage.Remove(sessionId);
        textStorage.SessionCleanupCancellationTokens.Remove(sessionId);
    }
}