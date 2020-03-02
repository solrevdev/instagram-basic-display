## Solrevdev.InstagramBasicDisplay ![](docs/instagram-logo.png)

![CI](https://github.com/solrevdev/instagram-basic-display/workflows/CI/badge.svg)

A [netstandard2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) library that consumes the new [Instagram Basic Display API](https://developers.facebook.com/docs/instagram-basic-display-api/).

Also includes a sample [Razor Pages Web](samples/Web/readme.md#razor-pages-example) example that shows how to use the library and a [Static site](samples/Static/readme.md#static-example) example that shows how to consume the new [Instagram Basic Display API](https://developers.facebook.com/docs/instagram-basic-display-api/) with just vanilla JavaScript.

### Getting started

[Install the nuget package](#installation)

[Setup Facebook and Instagram](#facebook-and-instagram-setup)

[App configuration](#app-configuration)

[Look at common uses](#common-uses)

Browse sample [Razor Pages Web](samples/Web/readme.md#razor-pages-example) and [Static site](samples/Static/readme.md#static-example) code

### Installation

To install using the dotnet cli

```bash
dotnet add package Solrevdev.InstagramBasicDisplay
```

To install using Visual Studio / Powershell

```powershell
Install-Package Solrevdev.InstagramBasicDisplay
```

### Facebook and Instagram Setup

Before you begin you will need to create an Instagram  `client_id` and `client_secret` by creating a Facebook app and configuring it so that it knows your ***https only*** `redirect_url`.

See the [Facebook and Instagram Setup](docs/facebook-and-instagram-setup.md) page for detailed steps.

### App Configuration

In your [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/about) library or application create an `appsettings.json` file if one does not already exist and fill out the `InstagramSettings` section with your Instagram credentials such as client_id, client_secret and redirect_url as mentioned above.

**appsettings.json**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "InstagramCredentials": {
    "Name": "friendly name or your app name can go here - this is passed to Instagram as the user-agent",
    "ClientId": "client-id",
    "ClientSecret": "client-secret",
    "RedirectUrl": "https://localhost:5001/auth/oauth"
  }
}
```

### Common Uses

**Get an Instagram User Access Token and permissions from an Instagram user**

First you send the user to Instagram to authenticate using the `Authorize` method, they will be redirected to the `RedirectUrl` set in `InstagramCredentials` so ensure that is set-up correctly in the Instagram app settings page.

Instagram will redirect the user on a successful login to the `RedirectUrl` page you configured in `InstagramCredentials` and this is where you can call `AuthenticateAsync` which exchanges the [Authorization Code](https://developers.facebook.com/docs/instagram-basic-display-api/overview#authorization-codes) for a [short-lived instagram user access token](https://developers.facebook.com/docs/instagram-basic-display-api/overview#instagram-user-access-tokens) or optionally a [long-lived instagram user access token](https://developers.facebook.com/docs/instagram-basic-display-api/guides/long-lived-access-tokens#get-a-long-lived-token).

You then have access to an `OAuthResponse` which contains your [access token](https://developers.facebook.com/docs/instagram-basic-display-api/reference/access_token) and a [user](https://developers.facebook.com/docs/instagram-basic-display-api/reference/user) which can be used to make further API calls.

```csharp
private readonly InstagramApi _api;

public IndexModel(InstagramApi api)
{
    _api = api;
}

public ActionResult OnGet()
{
    var url = _api.Authorize("anything-passed-here-will-be-returned-as-state-variable");
    return Redirect(url);
}
```

Then in your `RedirectUrl` page

```csharp
private readonly InstagramApi _api;
private readonly ILogger<IndexModel> _logger;

public IndexModel(InstagramApi api, ILogger<IndexModel> logger)
{
    _api = api;
    _logger = logger;
}

// code is passed by instagram, state is whatever you passed in _api.Authorize sent back to you
public async Task<IActionResult> OnGetAsync(string code, string state)
{
    // this returns an access token that will last for 1 hour - short-lived access token
    var response = await _api.AuthenticateAsync(code, state).ConfigureAwait(false);

    // this returns an access token that will last for 60 days - long-lived access token
    // var response = await _api.AuthenticateAsync(code, state, true).ConfigureAwait(false);

    // store in session - see System.Text.Json code below for sample
    HttpContext.Session.Set("Instagram.Response", response);
}
```

If you want to store the `OAuthResponse` in `HttpContext.Session` you can use the new `System.Text.Json` namespace like this

```csharp
using System.Text.Json;
using Microsoft.AspNetCore.Http;
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}
```

**Get an Instagram user’s profile**

```csharp
private readonly InstagramApi _api;
private readonly ILogger<IndexModel> _logger;

public IndexModel(InstagramApi api, ILogger<IndexModel> logger)
{
    _api = api;
    _logger = logger;
}

// code is passed by instagram, state is whatever you passed in _api.Authorize sent back to you
public async Task<IActionResult> OnGetAsync(string code, string state)
{
    // this returns an access token that will last for 1 hour - short-lived access token
    var response = await _api.AuthenticateAsync(code, state).ConfigureAwait(false);

    // this returns an access token that will last for 60 days - long-lived access token
    // var response = await _api.AuthenticateAsync(code, state, true).ConfigureAwait(false);

    // store and log
    var user = response.User;
    var token = response.AccessToken;

    _logger.LogInformation("UserId: {userid} Username: {username} Media Count: {count} Account Type: {type}", user.Id, user.Username, user.MediaCount, user.AccountType);
    _logger.LogInformation("Access Token: {token}", token);
}
```

**Get an Instagram user’s images, videos, and albums**

```csharp
private readonly InstagramApi _api;
private readonly ILogger<IndexModel> _logger;

public List<Media> Media { get; } = new List<Media>();

public IndexModel(InstagramApi api, ILogger<IndexModel> logger)
{
    _api = api;
    _logger = logger;
}

// code is passed by instagram, state is whatever you passed in _api.Authorize sent back to you
public async Task<IActionResult> OnGetAsync(string code, string state)
{
    // this returns an access token that will last for 1 hour - short-lived access token
    var response = await _api.AuthenticateAsync(code, state).ConfigureAwait(false);

    // this returns an access token that will last for 60 days - long-lived access token
    // var response = await _api.AuthenticateAsync(code, state, true).ConfigureAwait(false);

    // store and log
    var media = await _api.GetMediaListAsync(response).ConfigureAwait(false);

    _logger.LogInformation("Initial media response returned with [{count}] records ", media.Data.Count);

    _logger.LogInformation("First caption: {caption}, First media url: {url}",media.Data[0].Caption, media.Data[0].MediaUrl);

    //
    //  toggle the following boolean for a quick and dirty way of getting all a user's media.
    //
    if(false)
    {
        while (!string.IsNullOrWhiteSpace(media?.Paging?.Next))
        {
            var next = media?.Paging?.Next;
            var count = media?.Data?.Count;
            _logger.LogInformation("Getting next page [{next}]", next);

            media = await _api.GetMediaListAsync(next).ConfigureAwait(false);

            _logger.LogInformation("next media response returned with [{count}] records ", count);

            // add to list
            Media.Add(media);
        }

        _logger.LogInformation("The user has a total of {count} items in their Instagram feed", Media.Count);
    }
}
```

**Exchange a short-lived access token for a long-lived access token**

```csharp
private readonly InstagramApi _api;
private readonly ILogger<IndexModel> _logger;

public IndexModel(InstagramApi api, ILogger<IndexModel> logger)
{
    _api = api;
    _logger = logger;
}

// code is passed by instagram, state is whatever you passed in _api.Authorize sent back to you
public async Task<IActionResult> OnGetAsync(string code, string state)
{
    // this returns an access token that will last for 1 hour - short-lived access token
    var response = await _api.AuthenticateAsync(code, state).ConfigureAwait(false);
    _logger.LogInformation("response access token {token}", response.AccessToken);

    var longLived = await _api.GetLongLivedAccessTokenAsync(response).ConfigureAwait(false);
    _logger.LogInformation("longLived access token {token}", longLived.AccessToken);
}
```

**Refresh a long-lived access token for another long-lived access token**

```csharp
private readonly InstagramApi _api;
private readonly ILogger<IndexModel> _logger;

public IndexModel(InstagramApi api, ILogger<IndexModel> logger)
{
    _api = api;
    _logger = logger;
}

// code is passed by instagram, state is whatever you passed in _api.Authorize sent back to you
public async Task<IActionResult> OnGetAsync(string code, string state)
{
    // this returns an access token that will last for 1 hour - short-lived access token
    var response = await _api.AuthenticateAsync(code, state).ConfigureAwait(false);
    _logger.LogInformation("response access token {token}", response.AccessToken);

    var longLived = await _api.GetLongLivedAccessTokenAsync(response).ConfigureAwait(false);
    _logger.LogInformation("longLived access token {token}", longLived.AccessToken);

    var another = await _api.RefreshLongLivedAccessToken(response).ConfigureAwait(false);
    _logger.LogInformation("response access token {token}", another.AccessToken);
}
```

### InstagramApi Class Information

For more information see [InstagramApi.md](docs/InstagramApi.md#instagramapi)

### User Token Generator

Facebook also provide tooling to quickly generate [long-lived Instagram User Access Tokens](https://developers.facebook.com/docs/instagram-basic-display-api/overview#instagram-user-access-tokens) for any of your public Instagram accounts.

For more information see [User Token Generator](facebook-and-instagram-setup.md#user-token-generator).

![User Token Generator](https://i.imgur.com/Ql7mrk0.png)
