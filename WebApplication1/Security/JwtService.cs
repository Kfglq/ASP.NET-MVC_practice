using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Jose;
using System.Text;

namespace WebApplication1.Security
{
    public class JwtService
    {
        public string GenerateToken(string Account,string Role)
        {
            JwtObject jwtObject = new JwtObject
            {
                Account = Account,
                Role = Role,
                Expire = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"])).ToString()
            };
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            var payload = jwtObject;
            var token = JWT.Encode(payload, Encoding.UTF8.GetBytes(SecretKey), JwsAlgorithm.HS512);
            return token;
        }
    }
}