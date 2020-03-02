using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Web.Extensions
{
    /// <summary>
    /// Session extension methods to store complex objects in HttpSession using JSON
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Stores a complex object into HttpSession using a string key.
        /// </summary>
        /// <param name="session">The <see="ISession" /> to store the complex object on</param>
        /// <param name="key">The string key to store the complex object against</param>
        /// <param name="value">The complex object to store</param>
        /// <typeparam name="T">The complex objects type definition</typeparam>
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        /// <summary>
        /// Gets a complex object from HttpSession using its' string key
        /// </summary>
        /// <param name="session">The <see="ISession" /> to get the complex object from</param>
        /// <param name="key">The string key to get the complex object from</param>
        /// <typeparam name="T">The complex objects type definition</typeparam>
        /// <returns>The complex object returned from HttpSession of default if not found</returns>
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
