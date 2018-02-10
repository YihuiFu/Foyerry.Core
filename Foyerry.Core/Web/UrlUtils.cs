using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Foyerry.Core.Web
{
    public static class UrlUtils
    {
        public static T GetParamVal<T>(string paramName, T defaultVal)
        {
            var result = defaultVal;
            try
            {
                var paramVal = HttpContext.Current.Request[paramName];
                if (!string.IsNullOrEmpty(paramVal))
                {
                    result = (T)Convert.ChangeType(paramVal, typeof(T));
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static string AddQueryParam(this string source, string key, string val)
        {
            string delimit;
            if (string.IsNullOrEmpty(source) || source.Contains("?"))
            {
                delimit = "?";
            }
            else if (source.EndsWith("?") || source.EndsWith("&"))
            {
                delimit = string.Empty;
            }
            else
            {
                delimit = "&";
            }
            return source + delimit + HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(val);
        }

        public static string Post(string url, string paramVal, int? timeOut)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            var dataArray = Encoding.UTF8.GetBytes(paramVal);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = dataArray.Length;
            timeOut = timeOut ?? 5;
            webRequest.Timeout = (int)timeOut * 1000;
            try
            {
                webRequest.GetRequestStream().Write(dataArray, 0, dataArray.Length);
                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                var streamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                var result = streamReader.ReadToEnd();
                streamReader.Close();
                webResponse.Close();
                webRequest.Abort();
                return result;
            }
            catch (WebException ex)
            {
                var expStr = ex.Message;
                if (ex.Response != null)
                {
                    if (ex.Response.ContentLength != 0)
                    {
                        using (var stream = ex.Response.GetResponseStream())
                        {
                            if (stream == null) return "";
                            using (var reader = new StreamReader(stream))
                            {
                                expStr = reader.ReadToEnd();
                            }
                        }
                    }
                }
                //write log exception message
                Console.WriteLine(expStr);
                return "";
            }

        }
    }
}
