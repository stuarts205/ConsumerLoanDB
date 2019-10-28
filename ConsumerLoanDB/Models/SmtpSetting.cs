using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerLoanDB.Models
{
    class SmtpSetting
    {
        public int SmtpSettingId { get; set; }
        public int? Port { get; set; }
        public string ServerName { get; set; }
        public string Domain { get; set; }
        public string Password { get; set; }
        public string Office { get; set; }
        public string EmailAddress { get; set; }
    }
}
