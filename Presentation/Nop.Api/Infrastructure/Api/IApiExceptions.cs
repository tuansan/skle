#region Using namespaces...

using System.Net;

#endregion Using namespaces...

namespace Nop.Api.Infrastructure.Api
{
    /// <summary>
    /// IApiExceptions Interface
    /// </summary>
    public interface IApiExceptions
    {
        /// <summary>
        /// ErrorCode
        /// </summary>
        int ErrorCode { get; set; }

        /// <summary>
        /// ErrorDescription
        /// </summary>
        string ErrorDescription { get; set; }

        /// <summary>
        /// HttpStatus
        /// </summary>
        HttpStatusCode HttpStatus { get; set; }

        /// <summary>
        /// ReasonPhrase
        /// </summary>
        string ReasonPhrase { get; set; }
    }
}