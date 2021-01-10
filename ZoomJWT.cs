using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomJWT
{
    /// <summary>
    /// Wrapper class for functions used to create Java Web Tokens for use with Zoom APIs.  At the time this library was written, the information on how to
    /// generate these tokens was not available in any one place, and a reliable implementation in C# could not be found.  This is an amalgamation of the
    /// following:
    ///   https://github.com/zoom/zoom-sdk-windows/blob/master/CHANGELOG.md#new-sdk-initialization-method-using-jwt-token
    ///   https://devforum.zoom.us/t/zoom-sdk-initialize-failed-when-running-the-sample-project/26511
    ///   https://devforum.zoom.us/t/how-to-create-jwt-token-using-rest-api-in-c/6620
    /// </summary>
    public static class ZoomJWT
    {
        /// <summary>
        /// Creates a Java Web Token for use with the Zoom Client SDK: https://marketplace.zoom.us/docs/sdk/native-sdks/introduction.
        /// </summary>
        /// <param name="sdkKey">The "SDK Key" obtained from your SDK App's "App Credentials" page.</param>
        /// <param name="sdkSecret">The "SDK Secret" obtained from your SDK App's "App Credentials" page.</param>
        /// <param name="accessTokenValidityInMinutes">How long the access token is valid for, in minutes.  Maximum value: 48 hours.</param>
        /// <param name="tokenValidityInMinutes">How long the token is valid for, in minutes.  Minimum value: 30 minutes.  Defaults to accessTokenValidityInMinutes.</param>
        /// <returns>A string containing the generated Java Web Token.</returns>
        public static string CreateClientSDKToken(string sdkKey, string sdkSecret, int accessTokenValidityInMinutes = 120, int tokenValidityInMinutes = -1)
        {
            DateTime now = DateTime.UtcNow;

            if (tokenValidityInMinutes <= 0)
            {
                tokenValidityInMinutes = accessTokenValidityInMinutes;
            }

            int tsNow = (int)(now - new DateTime(1970, 1, 1)).TotalSeconds;
            int tsAccessExp = (int)(now.AddMinutes(accessTokenValidityInMinutes) - new DateTime(1970, 1, 1)).TotalSeconds;
            int tsTokenExp = (int)(now.AddMinutes(tokenValidityInMinutes) - new DateTime(1970, 1, 1)).TotalSeconds;

            return CreateToken(sdkSecret, new JwtPayload
                {
                    { "appKey", sdkKey },
                    { "iat", tsNow },
                    { "exp", tsAccessExp },
                    { "tokenExp", tsTokenExp },
                });
        }

        /// <summary>
        /// Creates a Java Web Token for use with the Zoom API: https://marketplace.zoom.us/docs/api-reference/zoom-api.
        /// </summary>
        /// <param name="apiKey">The "SDK Key" obtained from your SDK App's "App Credentials" page.</param>
        /// <param name="apiSecret">The "SDK Secret" obtained from your SDK App's "App Credentials" page.</param>
        /// <param name="tokenValidityInMinutes">How long the token is valid for, in minutes.  Minimum value: 30 minutes.</param>
        /// <returns>A string containing the generated Java Web Token.</returns>
        public static string CreateAPIToken(string apiKey, string apiSecret, int tokenValidityInMinutes = 120)
        {
            DateTime now = DateTime.UtcNow;

            int tsNow = (int)(now - new DateTime(1970, 1, 1)).TotalSeconds;
            int tokenExp = (int)(now.AddMinutes(tokenValidityInMinutes) - new DateTime(1970, 1, 1)).TotalSeconds;

            return CreateToken(apiSecret, new JwtPayload
                {
                    { "appKey", apiKey },
                    { "iat", tsNow },
                    { "tokenExp", tokenExp },
                });
        }

        /// <summary>
        /// Creates a Java Web Token with the given secret and payload.  This should not be called directly unless you intend to hand-craft a JWT's payload.
        /// </summary>
        /// <param name="secret">Encryption key.</param>
        /// <param name="payload">JWT Payload.</param>
        /// <returns>A string containing the generated Java Web Token.</returns>
        public static string CreateToken(string secret, JwtPayload payload)
        {
            // Create Security key using private key above:
            // note that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Finally create a Token
            var header = new JwtHeader(credentials);

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            return handler.WriteToken(secToken);
        }
    }
}
