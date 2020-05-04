namespace NetCoreApiSandbox.Infrastructure.Errors
{
    #region

    using System;
    using System.Net;

    #endregion

    public class RestException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestException"/> class.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="errors"></param>
        public RestException(HttpStatusCode code, object errors = null)
        {
            this.Code = code;
            this.Errors = errors;
        }

        public RestException() { }

        public RestException(string message): base(message) { }

        public RestException(string message, Exception innerException): base(message, innerException) { }

        public object Errors { get; set; }

        public HttpStatusCode Code { get; }
    }
}
