using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;

namespace Solrevdev.InstagramBasicDisplay.Core.Exceptions
{
    /// <summary>
    /// Represent errors that occur while calling a Instagram API and getting a specific IGApiException type exception returned
    /// </summary>
    [Serializable]
    [SuppressMessage("Design", "RCS1194", Justification = "No caught exception to wrap")]
    public class InstagramApiException : InstagramException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramOAuthException"/> class.
        /// </summary>
        public InstagramApiException()
        {
        }

        public InstagramApiException(InstagramError error) : base(error.Message)
        {
            ErrorType = error.Type;
            ErrorCode = error.Code;
            ErrorSubcode = error.ErrorSubcode;
            FbTraceId = error.FbTraceId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramOAuthException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorType">Type of the error.</param>
        /// <param name="errorCode">Code of the error.</param>
        /// <param name="errorSubcode">Subcode of the error.</param>
        public InstagramApiException(string message, string errorType = default, int errorCode = default, int errorSubcode = default)
            : base(message, errorType, errorCode, errorSubcode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramOAuthException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InstagramApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstagramOAuthException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected InstagramApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
