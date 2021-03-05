#region Using namespaces...

using System;
using System.Net;
using System.Runtime.Serialization;

#endregion Using namespaces...

namespace Nop.Api.Infrastructure.Api
{
    /// <summary>
    /// Api Data Exception
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiDataException : Exception, IApiExceptions
    {
        #region Public Serializable properties...

        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public string ErrorDescription { get; set; }

        [DataMember]
        public HttpStatusCode HttpStatus { get; set; }

        private string reasonPhrase = "ApiDataException";

        [DataMember]
        public string ReasonPhrase
        {
            get { return reasonPhrase; }
            set { reasonPhrase = value; }
        }

        #endregion Public Serializable properties...

        #region Public constructor...

        /// <summary>
        /// Public construcor for Api Data Exception
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorDescription"></param>
        /// <param name="httpStatus"></param>
        public ApiDataException(int errorCode, string errorDescription, HttpStatusCode httpStatus)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
            HttpStatus = httpStatus;
        }

        #endregion Public constructor...
    }
}