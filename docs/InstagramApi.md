## InstagramApi ![](../docs/instagram-logo.png)

The main entry point for talking to the Instagram Basic Display API which allows users of your app to get basic profile information, photos, and videos in their Instagram accounts.

This class provides means to:

    *   Get an Instagram User Access Token and permissions from an Instagram user
    *   Get an Instagram user's profile
    *   Get an Instagram user's images, videos, and albums

You can also view either some quick [getting started notes](../readme.md#common-uses) or view a [full razor pages sample](../samples/Web/readme.md#razor-pages-example)

### User Token Generator

Facebook also provide tooling to quickly generate [long-lived Instagram User Access Tokens](https://developers.facebook.com/docs/instagram-basic-display-api/overview#instagram-user-access-tokens) for any of your public Instagram accounts.

For more information see [User Token Generator](facebook-and-instagram-setup.md#user-token-generator).

![User Token Generator](https://i.imgur.com/Ql7mrk0.png)

### Constructor

Default constructor that takes in the dependencies that it needs to get configuration, logging and external http client calls

```csharp
public InstagramApi(IOptions<InstagramCredentials> appSettings, ILogger<InstagramApi> logger, InstagramHttpClient client)
{
    _appSettings = appSettings.Value;
    _logger = logger;
    _client = client;
}
```

### Methods

The `Authorize` method will be called first to begin authentication with Instagram. A URL is returned which will redirect the user to Instagram's authorization window

```csharp
/// <summary>
/// <para>
/// Returns the URL to send the user of your app to in order to begin the OAuth dance in order to get an access token.
/// </para>
/// <para>
/// This will send the user to Instagram's authorization window where they will be told your app is requesting permissions you set when configuring your Instagram application at https://developers.facebook.com
/// </para>
/// <para>
/// Note: you can pass a custom string in a state variable and Instagram will return that variable in the callback. This is
/// useful for passing data between your app and Instagram. For example user-id's and such.
/// </para>
/// </summary>
/// <param name="state">An optional state parameter that when passed to Instagram will get returned in the callback</param>
/// <returns></returns>
public string Authorize (string state = "")
{

}
```

Once the user has been redirected back from Instagram with an authorization code you will call this method in order to exchange that code for a either a 1 hour short-lived access token or optionally a 60 day long-lived access token.

This method also makes an additional call in order to get information from the Instagram user's profile into a UserInfo object.

```csharp
/// <summary>
///  The authorization code from Instagram is exchanged for short-lived Instagram User Access Token or if the <param name="useLongTermAccessToken"></param> boolean is set to true it is exchanged for a long-lived Instagram User Access Token.
/// </summary>
/// <param name="code">The authorization code from Instagram returned in the querystring</param>
/// <param name="state">The optional state parameter sent with the original request to Instagram</param>
/// <param name="useLongTermAccessToken">true for a long-lived token and false the default short-lived token.</param>
/// <returns>An OAuthResponse object</returns>
public async Task<OAuthResponse> AuthenticateAsync(string code, string state, bool useLongTermAccessToken = false)
{
}
```

While `AuthenticateAsync` can optionally return a long-lived Instagram user access token you may require to call that later and in a separate method call. This is where you can do that.

```csharp
/// <summary>
/// Exchanges short-lived access token for a long-lived access token
/// </summary>
/// <param name="response">A valid <see cref="OAuthResponse"/></param>
/// <returns>A OAuthResponse or null if an error is caught</returns>
/// <exception cref="ArgumentNullException">An Instagram User Access Token is needed</exception>
public async Task<OAuthResponse> GetLongLivedAccessTokenAsync(OAuthResponse response)
{
}
```

While a the initial 1 hour or an exchanged 60 day Instagram user access token will be enough for most cases you can refresh the 60 day long-lived access token for another 60 days.

```csharp
/// <summary>
/// Refreshes an existing long-lived Instagram User Access Token for another 60 day one.
/// </summary>
/// <param name="response">A valid <see cref="OAuthResponse"/></param>
/// <returns>A refreshed long-lived 60 day access token or null if an error is caught</returns>
/// <exception cref="ArgumentNullException">An Instagram User Access Token is needed</exception>
public async Task<OAuthResponse> RefreshLongLivedAccessToken(OAuthResponse response)
{
}
```

While `AuthenticateAsync` will return an `OAuthResponse` object containing a `UserInfo` object you may wish to make this a separate call, this is how you do this by passing a valid Instagram `user_id`. Passing `me` will return the current logged in user.

```csharp
/// <summary>
/// Gets a <see also="UserInfo" /> for either the current user (me) or the user specified with a userId
/// </summary>
/// <param name="accessToken">An Instagram User Access Token</param>
/// <param name="userId">Defaults to me or a valid userId</param>
/// <returns>A UserInfo object or null if an error is caught</returns>
public async Task<UserInfo> GetUserAsync(string accessToken, string userId = "me")
{

}
```

A common use case will be to get an Instagram user's image's, videos and albums as a list

```csharp
/// <summary>
/// Gets a <see also="Media" /> object for a given user
/// </summary>
/// <param name="accessToken">An Instagram User Access Token</param>
/// <param name="userId">Default to me or pass a valid userId</param>
/// <returns>A Media object or null if an error is caught</returns>
public async Task<Media> GetMediaListAsync(string accessToken, string userId)
{
}
```

You can also get an individual media object from it's identifier, a common use case will be where you a list of an Instagram's user's media and where an item is an album you want to view each individual item in that album. Use this method by passing in the individual item's media id.

```csharp
/// <summary>
/// Gets a <see also="Media" /> object via it's unique identifier
/// </summary>
/// <param name="accessToken">An Instagram User Access Token</param>
/// <param name="mediaId">A valid Media Id</param>
/// <returns>A Media object or null if an error is caught</returns>
public async Task<Media> GetMediaAsync(string accessToken, string mediaId)
{
}
```
