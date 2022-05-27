using Android.Content;
using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

namespace Nexus.OAuth.Android.Assets.Api.Models
{
    internal class Authentication
    {
        private const string AuthTokenKey = "Authentication-Token";
        private const string FirstTokenKey = "First-Step-Token";
        private const string RefreshTokenKey = "Refresh-Token";
        private const string AuthSharedPrefreferencesKey = "Authentication-Preferences";
        private const string AuthExpiresKey = "Auth-Expires";
        private const string AuthDateKey = "Auth-Date";
        private const string AuthTypeKey = "Auth-Type";

        public TokenType TokenType { get; private set; }
        public string AuthenticationToken { get; private set; }
        public string? RefreshToken { get; private set; }
        public string? FirstStepToken { get; private set; }
        public double? ExpiresIn { get; private set; }
        public DateTime Date { get; private set; }

        private Authentication(TokenType tokenType, string authenticationToken, string? refreshToken, string? firstStepToken, double? expiresIn, DateTime date)
        {
            TokenType = tokenType;
            AuthenticationToken = authenticationToken ?? throw new ArgumentNullException(nameof(authenticationToken));
            RefreshToken = refreshToken;
            FirstStepToken = firstStepToken;
            ExpiresIn = expiresIn;
            Date = date;
        }

        public Authentication(AuthenticationResult result, FirstStepResult firstStep)
        {
            TokenType = result.TokenType;
            AuthenticationToken = result.Token;
            RefreshToken = result.RefreshToken;
            ExpiresIn = result.ExpiresIn;
            Date = result.Date;
            FirstStepToken = firstStep.Token;
        }

        public string ToHeader()
        {
            string header = $"{Enum.GetName(typeof(TokenType), TokenType)} {AuthenticationToken}";

            if (!string.IsNullOrEmpty(FirstStepToken))
                header += $".{FirstStepToken}";

            return header;
        }

        public async Task SaveAsync(Context context)
        {
            await SecureStorage.SetAsync(RefreshTokenKey, RefreshToken);
            await SecureStorage.SetAsync(AuthTokenKey, AuthenticationToken);
            await SecureStorage.SetAsync(FirstTokenKey, FirstStepToken);

            ISharedPreferencesEditor sharedEditor = context.GetSharedPreferences(AuthSharedPrefreferencesKey, FileCreationMode.Private).Edit();
            sharedEditor.PutFloat(AuthExpiresKey, Convert.ToInt64(ExpiresIn));
            sharedEditor.PutLong(AuthDateKey, Date.ToBinary());
            sharedEditor.PutInt(AuthTypeKey, (int)TokenType);

            sharedEditor.Commit();
        }
        public static async Task<Authentication> GetAsync(Context context)
        {
            string? rfToken = await SecureStorage.GetAsync(RefreshTokenKey);
            string? fsToken = await SecureStorage.GetAsync(FirstTokenKey);
            string authToken = await SecureStorage.GetAsync(AuthTokenKey);

            ISharedPreferences preferences = context.GetSharedPreferences(AuthSharedPrefreferencesKey, FileCreationMode.Private);
            long date = preferences.GetLong(AuthDateKey, 0);
            double expiresIn = preferences.GetFloat(AuthExpiresKey, 0);
            int type = preferences.GetInt(AuthTypeKey, -1);

            if (date == 0 ||
                string.IsNullOrEmpty(authToken) ||
                type == -1)
                throw new UnauthorizedAccessException();

            return new Authentication((TokenType)type,
                authToken,
                rfToken,
                fsToken,
                expiresIn,
                DateTime.FromBinary(date));
        }
        public static async Task LogoutAsync()
        {
            await SecureStorage.SetAsync(RefreshTokenKey, string.Empty);
            await SecureStorage.SetAsync(AuthTokenKey, string.Empty);
            await SecureStorage.SetAsync(FirstTokenKey, string.Empty);
        }
    }
}