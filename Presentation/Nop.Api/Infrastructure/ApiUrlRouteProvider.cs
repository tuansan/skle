using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Api.Infrastructure
{
    public class ApiUrlRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllers();
        }

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            //it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
            get { return -1000000; }
        }

        #endregion Properties
    }
}