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
    public class ValuesController : ApiController
    {
        [BasicAuthorization]
        public string[] Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
