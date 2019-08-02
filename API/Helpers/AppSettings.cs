using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
    }

    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
    }
}
