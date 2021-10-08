using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPI.Auth.Model
{
    public class UserAuthModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string CompanyId { get; set; }
    }
}