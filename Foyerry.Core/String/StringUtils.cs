using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foyerry.Core.String
{
   public class StringUtils
    {
        public static T GetDefaultVal<T>(object val, T defaultVal)
        {
            var result = defaultVal;
            if (val == null || val == DBNull.Value) return result;
            try
            {
                result = (T)Convert.ChangeType(val, typeof(T));
            }
            catch (InvalidCastException)
            {
            }
            return result;
        }
    }
}
