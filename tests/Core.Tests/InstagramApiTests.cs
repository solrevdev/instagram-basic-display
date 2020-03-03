using System;
using Xunit;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Moq.Protected;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.IO;

namespace Solrevdev.InstagramBasicDisplay.Core.Tests
{
    public abstract class BaseTest
    {
        public static string LoadFromFile(string fileName)
        {
            // Get the absolute path to the JSON file
            var path = Path.IsPathRooted(fileName)
                ? fileName
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), fileName);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            // Load the file
            return File.ReadAllText(fileName);
        }
    }

    public class InstagramApiTests : BaseTest
    {
        [Fact]
        public void Can_Create_InstanceOf_InstagramApi()
        {
            // Arrange
            var credentials = MockInstagramCredentials();
            var options = Options.Create(credentials);
            var logger = Mock.Of<ILogger<InstagramApi>>();
            var mockFactory = MockHttpClientFactory();
            var instagramHttpClient = new InstagramHttpClient(options, mockFactory.Object, logger);

            // Act
            var api = new InstagramApi(options, logger, instagramHttpClient);

            // Assert
            Assert.NotNull(api);
        }

        [Fact]
        public void Authorize_Without_Name_In_InstagramSettings_ThrowsException()
        {
            // Arrange
            var credentials = MockInstagramCredentials();
            credentials.Name = null;

            var options = Options.Create(credentials);
            var logger = Mock.Of<ILogger<InstagramApi>>();
            var mockFactory = MockHttpClientFactory();
            var instagramHttpClient = new InstagramHttpClient(options, mockFactory.Object, logger);

            // Act
            var api = new InstagramApi(options, logger, instagramHttpClient);
            var ex = Assert.Throws<Exception>(() => api.Authorize(""));

            // Assert
            Assert.NotNull(api);
            Assert.Equal("The Name is either null or empty please check the InstagramCredentials section in your appsettings.json", ex.Message);
        }

        [Fact]
        public void Authorize_Without_ClientId_In_InstagramSettings_ThrowsException()
        {
            // Arrange
            var credentials = MockInstagramCredentials();
            credentials.ClientId = null;

            var options = Options.Create(credentials);
            var logger = Mock.Of<ILogger<InstagramApi>>();
            var mockFactory = MockHttpClientFactory();
            var instagramHttpClient = new InstagramHttpClient(options, mockFactory.Object, logger);

            // Act
            var api = new InstagramApi(options, logger, instagramHttpClient);
            var ex = Assert.Throws<Exception>(() => api.Authorize(""));

            // Assert
            Assert.NotNull(api);
            Assert.Equal("The ClientId is either null or empty please check the InstagramCredentials section in your appsettings.json", ex.Message);
        }

        [Fact]
        public void Authorize_Without_ClientSecret_In_InstagramSettings_ThrowsException()
        {
            // Arrange
            var credentials = MockInstagramCredentials();
            credentials.ClientSecret = null;

            var options = Options.Create(credentials);
            var logger = Mock.Of<ILogger<InstagramApi>>();
            var mockFactory = MockHttpClientFactory();
            var instagramHttpClient = new InstagramHttpClient(options, mockFactory.Object, logger);

            // Act
            var api = new InstagramApi(options, logger, instagramHttpClient);
            var ex = Assert.Throws<Exception>(() => api.Authorize(""));

            // Assert
            Assert.NotNull(api);
            Assert.Equal("The ClientSecret is either null or empty please check the InstagramCredentials section in your appsettings.json", ex.Message);
        }

        [Fact]
        public void Authorize_Without_RedirectUrl_In_InstagramSettings_ThrowsException()
        {
            // Arrange
            var credentials = MockInstagramCredentials();
            credentials.RedirectUrl = null;

            var options = Options.Create(credentials);
            var logger = Mock.Of<ILogger<InstagramApi>>();
            var mockFactory = MockHttpClientFactory();
            var instagramHttpClient = new InstagramHttpClient(options, mockFactory.Object, logger);

            // Act
            var api = new InstagramApi(options, logger, instagramHttpClient);
            var ex = Assert.Throws<Exception>(() => api.Authorize(""));

            // Assert
            Assert.NotNull(api);
            Assert.Equal("The RedirectUrl is either null or empty please check the InstagramCredentials section in your appsettings.json", ex.Message);
        }

        [Fact]
        public void Authorize_Returns_Expected_Uri()
        {
            // Arrange
            var credentials = MockInstagramCredentials();
            var options = Options.Create(credentials);
            var logger = Mock.Of<ILogger<InstagramApi>>();
            var mockFactory = MockHttpClientFactory();
            var instagramHttpClient = new InstagramHttpClient(options, mockFactory.Object, logger);

            // Act
            const string state = "";
            var api = new InstagramApi(options, logger, instagramHttpClient);
            var url = api.Authorize(state);

            // Assert
            Assert.NotNull(api);
            Assert.NotNull(url);

            var uri = new Uri(url);
            Assert.NotNull(uri);

            Assert.Equal("api.instagram.com", uri.Authority);
            Assert.Equal("https", uri.Scheme);
            Assert.Equal("api.instagram.com", uri.Host);
            Assert.Equal($"?client_id={credentials.ClientId}&redirect_uri={credentials.RedirectUrl}&scope=user_profile,user_media&response_type=code&state={state}", uri.Query);
            Assert.Equal($"/oauth/authorize?client_id={credentials.ClientId}&redirect_uri={credentials.RedirectUrl}&scope=user_profile,user_media&response_type=code&state={state}", uri.PathAndQuery);
        }

        [Fact]
        public async Task Authenticate_Returns_OAuthResponse()
        {
            // Arrange
            var credentials = MockInstagramCredentials();
            var options = Options.Create(credentials);
            var logger = Mock.Of<ILogger<InstagramApi>>();
            var mockFactory = MockHttpClientFactory_For_Authenticate();
            var instagramHttpClient = new InstagramHttpClient(options, mockFactory.Object, logger);

            // Act
            var api = new InstagramApi(options, logger, instagramHttpClient);
            var response = await api.AuthenticateAsync("", "").ConfigureAwait(false);

            // Assert
            Assert.NotNull(api);
            Assert.NotNull(response);
            Assert.Equal("123", response.AccessToken);
            Assert.Equal("123", response.User.Id);
            Assert.Equal("BUSINESS", response.User.AccountType);
            Assert.Equal(116, response.User.MediaCount);
            Assert.Equal("solrevdev", response.User.Username);
        }

        [Fact]
        public async Task GetMediaListAsync_Returns_MediaList()
        {
            // Arrange
            var credentials = MockInstagramCredentials();
            var options = Options.Create(credentials);
            var logger = Mock.Of<ILogger<InstagramApi>>();
            var mockFactory = MockHttpClientFactory_For_GetMediaList();
            var instagramHttpClient = new InstagramHttpClient(options, mockFactory.Object, logger);
            var oAuthResponse = new OAuthResponse
            {
                AccessToken = "123",
                User = new UserInfo
                {
                    Id = "123",
                    Username = "solrevdev"
                }
            };

            // Act
            var api = new InstagramApi(options, logger, instagramHttpClient);
            var response = await api.GetMediaListAsync(oAuthResponse).ConfigureAwait(false);

            // Assert
            Assert.NotNull(api);
            Assert.NotNull(response);
            Assert.NotNull(response.Paging);
            Assert.NotNull(response.Data);
            Assert.Equal("Carousel", response.Data[0].Caption);
            Assert.Equal("CAROUSEL_ALBUM", response.Data[0].MediaType);
            Assert.Equal("18081199675165936", response.Data[0].Id);
            Assert.Equal("producer_journey", response.Data[0].Username);
            Assert.Equal(25, response.Data.Count); // unless you page through its 25 items per request
        }

        private static InstagramCredentials MockInstagramCredentials()
        {
            return new InstagramCredentials()
            {
                Name = "Unit Testing Instagram Basic Display API",
                ClientId = "123",
                ClientSecret = "a4b4c4d4e4",
                RedirectUrl = "http://www.localhost:5000/auth/oauth"
            };
        }

        private static Mock<IHttpClientFactory> MockHttpClientFactory()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"access_token\": \"123\", \"user_id\": 123}"),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            return mockFactory;
        }

        private static Mock<IHttpClientFactory> MockHttpClientFactory_For_Authenticate()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"access_token\": \"123\", \"user_id\": 123}"),
                })
                 .ReturnsAsync(new HttpResponseMessage
                 {
                     StatusCode = HttpStatusCode.OK,
                     Content = new StringContent("{\"account_type\":\"BUSINESS\",\"id\":\"123\",\"media_count\":116,\"username\":\"solrevdev\"}"),
                 });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            return mockFactory;
        }

        private static Mock<IHttpClientFactory> MockHttpClientFactory_For_GetMediaList()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var content = LoadFromFile("GetMediaListAsync.json");

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            return mockFactory;
        }
    }
}
