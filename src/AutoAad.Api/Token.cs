using System;

namespace AutoAad.Api
{
    public class Token
    {
        public string AccessToken { get; set; }

        public DateTime ExpireDate { get; set; }
    }
}