using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteThuCungBento.Models
{
    public class QuenMKModels
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}