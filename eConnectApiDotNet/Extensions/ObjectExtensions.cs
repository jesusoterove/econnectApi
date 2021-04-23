using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectApi.Extensions
{
    public static class ObjectExtensions
    {
        private static readonly DateTime _1900 = new DateTime(1900, 1, 1);
        public static string ToGPDate(this DateTime value)
        {
            if (value < _1900)
            {
                return _1900.ToGPDate();
            }

            return value.ToString("M/d/yyyy");
        }
    }
}
