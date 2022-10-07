using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using TestAPI.Services.Contracts;

namespace TestAPI.Filters
{
    public class AutoAuthenticationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            //HttpContext.Current.User = new GenericPrincipal(
            //    new GenericIdentity("username"),
            //    new string[] { });
            
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class AutoAuthenticationFilter : IAsyncActionFilter
    {
        IUserRepository _userRepository;
        public AutoAuthenticationFilter(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //try
            //{
            //    //if (context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            //    //{
            //    //    byte[] basicAuthBytes= System.Text.Encoding.UTF8.GetBytes(context.HttpContext.Request.Headers["authentication"]);
            //    //    string basicAuthStr = System.Text.Encoding.UTF8.GetString(basicAuthBytes);
            //        context.HttpContext.User = new GenericPrincipal(
            //            new GenericIdentity("username"),
            //            new string[] { });
            //        await Task.Delay(16);
            //    //}
            await next();
            //}
            //catch(Exception ex)
            //{

            //}
        }
    }
}