using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationHandler
{
    public static AuthenticationState AuthState { get; private set; } =  AuthenticationState.NotAuthenticated;

    public static async Task<AuthenticationState> DoAuth(int maximumTries = 5)
    {
        if (AuthState == AuthenticationState.Authenticated)
        {
            return AuthState;
        }

        if (AuthState == AuthenticationState.Authenticating)
        {
            Debug.LogWarning("Warning: Already authenticating!");
            await Authenticating();
            return AuthState;
        }
        
        await SignInAnonymouslyAsync(maximumTries);
        
        return AuthState;
    }

    private static async Task<AuthenticationState> Authenticating()
    {
        while (AuthState == AuthenticationState.Authenticating || AuthState == AuthenticationState.NotAuthenticated)
        {
            await Task.Delay(200);
        }
        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maximumTries = 5)
    {
        AuthState = AuthenticationState.Authenticating;

        int tries = 0;
        while (AuthState == AuthenticationState.Authenticating && tries < maximumTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthenticationState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException except)
            {
                Debug.LogError(except.Message);
                AuthState = AuthenticationState.AuthenticationError;
            }
            catch (RequestFailedException except)
            {
                Debug.LogError(except.Message);
                AuthState = AuthenticationState.AuthenticationError;
            }
            
            tries++;
            await Task.Delay(1000);

            if (AuthState != AuthenticationState.Authenticated)
            {
                Debug.LogWarning($"Timeout: not authenticated after {tries} attempts!");
                AuthState = AuthenticationState.AuthenticationTimeout;
            }
        }

    }
}
