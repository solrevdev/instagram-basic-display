using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;
using Solrevdev.InstagramBasicDisplay.Core.Exceptions;
using Solrevdev.InstagramBasicDisplay.Core.Extensions;

namespace Solrevdev.InstagramBasicDisplay.Core
{
    /// <summary>
    /// A wrapper around <see also="IHttpClientFactory" /> that given a valid set of <see also="InstagramCredentials" />
    /// will make HTTP GET and HTTP POST requests to the Instagram Basic Display API in order to authenticate and do the
    /// OAuth dance.
    /// </summary>
    public class InstagramHttpClient
    {
        private readonly InstagramCredentials _appSettings;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<InstagramApi> _logger;

        public InstagramHttpClient(IOptions<InstagramCredentials> appSettings, IHttpClientFactory clientFactory, ILogger<InstagramApi> logger)
        {
            _appSettings = appSettings.Value;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Makes a HTTP GET request to Instagram, pass in the URL to call and the T type to return and it will use the
        /// <see also="System.Text.Json.JsonSerializer" /> to Deserialize back as T
        /// </summary>
        /// <param name="url">The url to call</param>
        /// <typeparam name="T">The T type to return</typeparam>
        /// <returns>Either T or null if an error is caught</returns>
        public async Task<T> GetJsonAsync<T>(string url)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", _appSettings.Name);
                var client = _clientFactory.CreateClient();

                using var response = await client.SendAsync(request).ConfigureAwait(false);
                await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);

                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };
                return JsonSerializer.Deserialize<T>(responseString, options);
            }
            catch (InstagramOAuthException ex)
            {
                _logger.LogError(ex, "Instagram OAuth error - [{me}] with url [{url}] with message [{message}] and exception [{ex}] and stack [{stack}] with instagram response message : [{message}] error_type : [{type}] error_code : [{code}] fb_trace [{fbTrace}]", nameof(GetJsonAsync), url, ex.Message, ex, ex.StackTrace, ex.Message, ex.ErrorType, ex.ErrorCode, ex.FbTraceId);
                throw;
            }
            catch (InstagramApiException ex)
            {
                _logger.LogError(ex, "Instagram API error - [{me}] with url [{url}] with message [{message}] and exception [{ex}] and stack [{stack}] with instagram response message : [{message}] error_type : [{type}] error_code : [{code}] error_sub_code : [{subCode}] fb_trace [{fbTrace}]", nameof(GetJsonAsync), url, ex.Message, ex, ex.StackTrace, ex.Message, ex.ErrorType, ex.ErrorCode, ex.ErrorSubcode, ex.FbTraceId);
                throw;
            }
            catch (InstagramException ex)
            {
                _logger.LogError(ex, "General Instagram error - [{me}] with url [{url}] with message [{message}] and exception [{ex}] and stack [{stack}] with instagram response message : [{message}] error_type : [{type}] error_code : [{code}] fb_trace [{fbTrace}]", nameof(GetJsonAsync), url, ex.Message, ex, ex.StackTrace, ex.Message, ex.ErrorType, ex.ErrorCode, ex.FbTraceId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown exception calling [{me}] with url [{url}] with message [{message}] and exception [{ex}] and stack [{stack}]", nameof(GetJsonAsync), url, ex.Message, ex, ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        ///Makes a HTTP POST call to Instagram, pass in the URL to call and the T type to return and it will use the
        /// <see also="System.Text.Json.JsonSerializer" /> to Deserialize back as T
        /// </summary>
        /// <param name="url">The url to post to</param>
        /// <param name="parameters">The key/value pairs to post</param>
        /// <typeparam name="T">The T type to return</typeparam>
        /// <returns>Either T or null if an error is caught</returns>
        public async Task<T> PostJsonAsync<T>(string url, Dictionary<string, string> parameters)
        {
            try
            {
                var encodedContent = new FormUrlEncodedContent(parameters);
                var client = _clientFactory.CreateClient();

                using var response = await client.PostAsync(url, encodedContent).ConfigureAwait(false);
                await response.EnsureSuccessStatusCodeAsync().ConfigureAwait(false);

                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };
                return JsonSerializer.Deserialize<T>(responseString, options);
            }
            catch (InstagramOAuthException ex)
            {
                _logger.LogError(ex, "Instagram OAuth error - [{me}] with url [{url}] with message [{message}] and exception [{ex}] and stack [{stack}] with instagram response message : [{message}] error_type : [{type}] error_code : [{code}] fb_trace [{fbTrace}]", nameof(PostJsonAsync), url, ex.Message, ex, ex.StackTrace, ex.Message, ex.ErrorType, ex.ErrorCode, ex.FbTraceId);
                throw;
            }
            catch (InstagramApiException ex)
            {
                _logger.LogError(ex, "Instagram API error - [{me}] with url [{url}] with message [{message}] and exception [{ex}] and stack [{stack}] with instagram response message : [{message}] error_type : [{type}] error_code : [{code}] error_sub_code : [{subCode}] fb_trace [{fbTrace}]", nameof(PostJsonAsync), url, ex.Message, ex, ex.StackTrace, ex.Message, ex.ErrorType, ex.ErrorCode, ex.ErrorSubcode, ex.FbTraceId);
                throw;
            }
            catch (InstagramException ex)
            {
                _logger.LogError(ex, "General Instagram error - [{me}] with url [{url}] with message [{message}] and exception [{ex}] and stack [{stack}] with instagram response message : [{message}] error_type : [{type}] error_code : [{code}] fb_trace [{fbTrace}]", nameof(PostJsonAsync), url, ex.Message, ex, ex.StackTrace, ex.Message, ex.ErrorType, ex.ErrorCode, ex.FbTraceId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown exception calling [{me}] with url [{url}] with message [{message}] and exception [{ex}] and stack [{stack}]", nameof(PostJsonAsync), url, ex.Message, ex, ex.StackTrace);
                throw;
            }
        }
    }
}
