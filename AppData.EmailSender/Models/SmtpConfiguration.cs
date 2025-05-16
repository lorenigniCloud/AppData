using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.EmailSender.Models
{
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
