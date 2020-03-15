using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;

namespace Solrevdev.InstagramBasicDisplay.Core.Exceptions
{
    /// <summary>
    /// Represent general errors that occur while calling a Instagram API.
    /// </summary>
    [Serializable]
    public class InstagramException : Exception
    {
        /// <summary>
        /// Gets or sets the type of the error.
        /// </summary>
        /// <value>The type of the error.</value>
        [JsonPropertyName("type")]
        public string ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the code of the error.
        /// </summary>
        /// <value>The code of the error.</value>
        [JsonPropertyName("code")]
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error subcode.
        /// </summary>
        /// <value>The code of the error subcode.</value>
        [JsonPropertyName("error_subcode")]
        public int? ErrorSubcode { get; set; }

        [JsonPropertyName("fbtrace_id")]
        public string FbTraceId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramException"/> class.
        /// </summary>
        public InstagramException()
        {
        }

        public InstagramException(InstagramError error) : base(error.Message)
        {
            ErrorType = error.Type;
            ErrorCode = error.Code;
            ErrorSubcode = error.ErrorSubcode;
            FbTraceId = error.FbTraceId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InstagramException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorType">Type of the error.</param>
        public InstagramException(string message, string errorType)
            : this(string.Format(CultureInfo.InvariantCulture, "({0}) {1}", errorType ?? "Unknown", message))
        {
            ErrorType = errorType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorType">Type of the error.</param>
        /// <param name="errorCode">Code of the error.</param>
        public InstagramException(string message, string errorType, int errorCode)
            : this(string.Format(CultureInfo.InvariantCulture, "({0} - #{1}) {2}", errorType ?? "Unknown", errorCode, message))
        {
            ErrorType = errorType;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorType">Type of the error.</param>
        /// <param name="errorCode">Code of the error.</param>
        /// <param name="errorSubcode">Subcode of the error.</param>
        public InstagramException(string message, string errorType, int errorCode, int errorSubcode)
            : this(message, errorType, errorCode)
        {
            ErrorSubcode = errorSubcode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InstagramException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected InstagramException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
