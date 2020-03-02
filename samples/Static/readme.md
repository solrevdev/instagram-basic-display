## Static Example

🚨 This example shows how to authenticate and consume the Instagram Basic Display API but please do be aware that because it is static and client side you should not publish direct to production as it does not protect your `client_secret`.

Feel free to take the code and use it but you will need something like [dotenv](https://www.npmjs.com/package/dotenv) or similar to be able to store these variables as environment variables or [github secrets](https://help.github.com/en/actions/configuring-and-managing-workflows/creating-and-storing-encrypted-secrets#using-encrypted-secrets-in-a-workflow) or similar.

### Facebook and Instagram Setup

First make sure you have the `client_id`, `client_secret` and `redirect_url` configured and noted down, for more information go to the [Facebook and Instagram Setup](../../docs/facebook-and-instagram-setup.md#facebook-and-instagram-setup) page.

![](https://i.imgur.com/JN2fppT.png)

![](https://i.imgur.com/bhJybs4.png)

### Configure index.js with your `client_id` and `redirect_url`

Open `/samples/Static/index.js` and at the top of the page add your credentials you created in the [Facebook and Instagram Setup](#facebook-and-instagram-setup) above.

**index.js**

```javascript
const client_id = 'enter your client_id here';
const redirect_url = 'enter your redirect_url here';
```

### Configure oauth.js with your `client_id`, `client_secret` and `redirect_url`

Open `/samples/Static/oauth.js` and at the top of the page add your credentials you created in the [Facebook and Instagram Setup](#facebook-and-instagram-setup) above.

🚨 **PLEASE NOTE** This example shows how to authenticate and consume the Instagram Basic Display API but please do be aware that because it is static and client side you should not publish direct to production as it does not protect your `client_secret`.

**oauth.js**

```javascript
const client_id = 'enter your client_id here';
const client_secret = 'enter your client_secret here';
const redirect_url = 'enter your redirect_url here';
```

### Start a local web server

**First navigate to the directory this `readme.md` file is in.**

```bash
cd path/to/samples/static/
```

**Launch a web server in the `samples/Static` folder on port `5000`**

Here are some examples in various languages and tools

**Python**

```bash
python3 -m http.server 5000
python -m SimpleHTTPServer 5000
```

**.NET Core**

```powershell
dotnet tool install --global dotnet-serve
dotnet serve -p 5000
```

**JavaScript**

```bash
sudo npm install http-server -g
http-server --port 5000
```

**PHP**

```php
php -S 127.0.0.1:5000
```

![](https://i.imgur.com/apkhDFl.png)


### Serve local site over HTTPS using ngrok

Next you will need to serve the static site over ***HTTPS***

**First navigate to the directory this `readme.md` file is in.**

```bash
cd path/to/samples/static/
```

**Use [ngrok](https://dashboard.ngrok.com/get-started) to be able to serve local site over HTTPS**

```bash
ngrok http 5000
```

![](https://i.imgur.com/k29dNZR.png)

![](https://i.imgur.com/RfFxwwe.png)

### Update Facebook app to accept ngrok url as a valid redirect_url

![](https://i.imgur.com/bhJybs4.png)

### Open index page via ngrok

e.g `https://ngrok.url/index.html` which will serve the index.html page from the samples/static folder over https which is required by the Facebook app.

