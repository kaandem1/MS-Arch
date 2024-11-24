using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMS.Core.DomainLayer.Configuration
{
    public class ConectionOptions
    {
        public const string Connection = "ConnectionStrings";

        public string DefaultConection { get; set; } = string.Empty;
    }
}
