using RestAPI.Auth;
using RestAPI.Auth.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestAPI.Controllers
{
    public class TokenController : ApiController
    {
        [BasicAuthentication]
        public AuthModel Post([FromBody]UserAuthModel model)
        {
            return AuthHelper.GetToken(model.Username, model.Password, model.CompanyId);
        }
    }
}
