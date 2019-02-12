using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi.Models
{
    public class TokenRequest
    {
        [Required]
        public string grant_type { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }

        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
