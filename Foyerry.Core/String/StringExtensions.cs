using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foyerry.Core.String
{
    public static class StringExtensions
    {
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] Items(this string source, string separator = ",")
        {
            return string.IsNullOrEmpty(source)
                ? new string[0]
                : source.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetItem(this string source, int itemIndex, string separator = ",")
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            var items = source.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            return itemIndex > items.Length
                ? string.Empty
                : items[itemIndex];
        }

        public static string Ellipsis(this string source, int length, int ellipsisLength)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            return source.Length <= length
                ? source
                : source.Substring(0, length - ellipsisLength).PadRight(length, '.');
        }
    }
}
