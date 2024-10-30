using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMS.Core.DomainLayer.Configuration
{
    public class PasswordEncryptionOptions
    {
        public const string Auth = "PasswordEncryption";

        public string StaticSalt { get; set; } = string.Empty;
    }
}
