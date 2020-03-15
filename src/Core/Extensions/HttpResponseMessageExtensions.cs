using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Solrevdev.InstagramBasicDisplay.Core.Exceptions;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;

namespace Solrevdev.InstagramBasicDisplay.Core.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            // deserialize into an Instagram error object
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var error = JsonSerializer.Deserialize<InstagramGenericError>(content).Error;

            // throw a known OAuthException
            if (string.Equals(error.Type, "OAuthException", StringComparison.OrdinalIgnoreCase))
            {
                throw new InstagramOAuthException(error);
            }

            // throw a known IGApiException
            if (string.Equals(error.Type, "IGApiException", StringComparison.OrdinalIgnoreCase))
            {
                throw new InstagramApiException(error);
            }

            // throw a general Instagram Exception
            throw new InstagramException(error);
        }
    }
}
