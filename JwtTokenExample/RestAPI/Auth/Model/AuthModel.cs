using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPI.Auth.Model
{
    public class AuthModel
    {
        public AuthModel()
        {
            IsValid = false;
            Message = null;
            Token = null;
        }

        public AuthModel(bool isValid, string message, string token)
        {
            IsValid = isValid;
            Message = message;
            Token = token;
        }
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public string Token { get; set; }
        public string Expiry { get; set; }
        public int Company { get; set; }

    }
}