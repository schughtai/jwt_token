using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPI.Auth.Model
{
    public class ApiUserDto
    {
        public string UserId { get; set; }
        public string[] Roles { get; set; }
    }
}