### Facebook and Instagram Setup ![](../docs/instagram-logo.png)

Before you begin you will need to create an Instagram  `client_id` and `client_secret` by creating a Facebook app and configuring it so that it knows your `redirect_url`. There are full [instructions here](https://developers.facebook.com/docs/instagram-basic-display-api/getting-started).

#### Step 1 - Create a Facebook App

Go to [developers.facebook.com](https://developers.facebook.com), click **My Apps**, and create a new app. Once you have created the app and are in the App Dashboard, navigate to **Settings > Basic**, scroll the bottom of page, and click **Add Platform**.

![Step 1a - Create a Facebook App](https://i.imgur.com/kgnGHkv.png)

Choose **Website**, add your website's URL, and save your changes. You can change the platform later if you wish, but for this tutorial, use **Website**

![Step 1b - Create a Facebook App](https://i.imgur.com/1BFpWpQ.png)

#### Step 2 - Configure Instagram Basic Display

Click **Products**, locate the **Instagram** product, and click **Set Up** to add it to your app.

![Step 2a - Configure Instagram Basic Display](https://i.imgur.com/6sK3tkC.png)

Click **Basic Display**, scroll to the bottom of the page, then click **Create New App**.

![Step 2b - Configure Instagram Basic Display](https://i.imgur.com/Nq6V25f.png)

In the form that appears, complete each section using the guidelines below.

**Display Name**
Enter the name of the Facebook app you just created.

**Valid OAuth Redirect URIs**
Enter <https://localhost:5001/auth/oauth/> for your `redirect_url` that will be used later. **HTTPS must be used on all redirect urls**

**Deauthorize Callback URL**
Enter <https://localhost:5001/deauthorize>

**Data Deletion Request Callback URL**
Enter <https://localhost:5001/datadeletion>

**App Review**
Skip this section for now since this is just a demo.

#### Step 3 - Add an Instagram Test User

Navigate to **Roles > Roles** and scroll down to the **Instagram Testers section**. Click **Add Instagram Testers** and enter your Instagram account's username and send the invitation.

![Step 3a - Add an Instagram Test User](https://i.imgur.com/UWPx2NK.png)

Open a new web browser and go to [www.instagram.com](https://www.instagram.com/) and sign into your Instagram account that you just invited. Navigate to **(Profile Icon) > Edit Profile > Apps and Websites > Tester Invites** and accept the invitation.

![Step 3b - Add an Instagram Test User](https://i.imgur.com/Z25fBaq.png)

You can view these [invitations and applications](https://www.instagram.com/accounts/manage_access/) by navigating to **(Profile Icon) > Edit Profile > Apps and Websites**

![Step 3c - Add an Instagram Test User](https://i.imgur.com/oFqXPEG.png)

### Facebook and Instagram Credentials

Navigate to **My Apps > Your App Name > Basic Display**

![Navigate to My App](https://i.imgur.com/YoI2Wrm.png)

Make a note of the following Facebook and Instagram credentials:

**Instagram App ID**
This is going to be known as `client_id` later

**Instagram App Secret**
This is going to be known as `client_secret` later

**Client OAuth Settings > Valid OAuth Redirect URIs**
This is going to be known as `redirect_url` later

![Facebook and Instagram Credentials](https://i.imgur.com/k5H2JSN.png) *[go here for a full size screenshot](https://i.imgur.com/bSHOS5p.png)*

### User Token Generator

The Instagram User Token Generator is a tool you can use to quickly generate [long-lived Instagram User Access Tokens](https://developers.facebook.com/docs/instagram-basic-display-api/overview#instagram-user-access-tokens) for any of your public Instagram accounts. This is useful if you're testing your app and don't want to bother with implementing the [Authorization Window](https://developers.facebook.com/docs/instagram-basic-display-api/overview#authorization-window).

The tool works by triggering the Authorization Window, which you can sign into with a public Instagram account that you have designated as a [tester account](https://developers.facebook.com/docs/instagram-basic-display-api/overview#instagram-testers). After signing in, the tool will generate a long-lived access token that you can copy and paste. Note that tokens can only be generated for public Instagram accounts.

You can access the token generator in the **App Dashboard > Products > Instagram > Basic Display** tab.

![User Token Generator](https://i.imgur.com/ToqSQr7.png)
