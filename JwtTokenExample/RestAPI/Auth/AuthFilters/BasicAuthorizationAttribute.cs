using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RestAPI.Auth
{
    public class BasicAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var headers = context.Request.Headers;
            if (headers == null || headers.Authorization == null)
            {
                context.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }
            // only basic authentication
            else if (headers.Authorization.Scheme != "bearer")
            {
                context.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.BadRequest);
                return;
            }

            var auth = headers.Authorization;
            var user = AuthHelper.ValidateToken(auth.Parameter);
            if (!user.IsValid)
            {
                context.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }
        }
    }
}