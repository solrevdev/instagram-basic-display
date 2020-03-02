namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    /// <summary>
    /// A typed version of appsettings used for configuration, It is common to have development
    /// and production credentials when consuming the basic display api
    /// </summary>
    public class InstagramCredentials
    {
        /// <summary>
        /// A friendly name also used as a user agent http header when talking to instagram and to
        /// help differentiate between development and production credentials
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The instagram client-id credentials
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The instagram client-secret credentials
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The redirect-url to match what has been set in the instagram app.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// TODO: remove when publishing
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// TODO: remove when publishing
        /// </summary>
        public string Domain { get; set; }
    }
}
