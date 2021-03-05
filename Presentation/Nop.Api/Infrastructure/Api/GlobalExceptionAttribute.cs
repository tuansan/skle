#region Using namespaces...

using Microsoft.AspNetCore.Mvc.Filters;

//using System.Web.Http.Filters;
//using System.Web.Http.Tracing;

#endregion Using namespaces...

namespace Nop.Api.Infrastructure.Api
{
    /// <summary>
    /// Action filter to handle for Global application errors.
    /// </summary>
    public class GlobalExceptionAttribute : ExceptionFilterAttribute
    {
        //public override void OnException(HttpActionExecutedContext actionExecutedContext)
        //{
        //    GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
        //    var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
        //    trace.Error(actionExecutedContext.Request, "Controller: " + actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action: " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName, actionExecutedContext.Exception);

        //    var exceptionType = actionExecutedContext.Exception.GetType();

        //    if (exceptionType == typeof(ValidationException))
        //    {
        //        var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
        //        {
        //            Content = new StringContent(actionExecutedContext.Exception.Message),
        //            ReasonPhrase = "ValidationException",
        //        };
        //        throw new HttpResponseException(resp);
        //    }
        //    else if (exceptionType == typeof(UnauthorizedAccessException))
        //    {
        //        throw new HttpResponseException(actionExecutedContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new ServiceStatus() { StatusCode = (int)HttpStatusCode.Unauthorized, StatusMessage = "UnAuthorized", ReasonPhrase = "UnAuthorized Access" }));
        //    }
        //    else if (exceptionType == typeof(ApiException))
        //    {
        //        var webapiException = actionExecutedContext.Exception as ApiException;
        //        if (webapiException != null)
        //            throw new HttpResponseException(actionExecutedContext.Request.CreateResponse(webapiException.HttpStatus, new ServiceStatus() { StatusCode = webapiException.ErrorCode, StatusMessage = webapiException.ErrorDescription, ReasonPhrase = webapiException.ReasonPhrase }));
        //    }
        //    else if (exceptionType == typeof(ApiBusinessException))
        //    {
        //        var businessException = actionExecutedContext.Exception as ApiBusinessException;
        //        if (businessException != null)
        //            throw new HttpResponseException(actionExecutedContext.Request.CreateResponse(businessException.HttpStatus, new ServiceStatus() { StatusCode = businessException.ErrorCode, StatusMessage = businessException.ErrorDescription, ReasonPhrase = businessException.ReasonPhrase }));
        //    }
        //    else if (exceptionType == typeof(ApiDataException))
        //    {
        //        var dataException = actionExecutedContext.Exception as ApiDataException;
        //        if (dataException != null)
        //            throw new HttpResponseException(actionExecutedContext.Request.CreateResponse(dataException.HttpStatus, new ServiceStatus() { StatusCode = dataException.ErrorCode, StatusMessage = dataException.ErrorDescription, ReasonPhrase = dataException.ReasonPhrase }));
        //    }
        //    else
        //    {
        //        throw new HttpResponseException(actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError));
        //    }
        //}
    }
}