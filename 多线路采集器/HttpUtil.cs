﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;

namespace 多线路采集器
{
    class HttpUtil
    {
        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        /// <summary>
        /// 指定Url地址使用Get方式获取全部字符串
        /// </summary>
        /// <param name="url">请求链接地址</param>
        /// <returns></returns>
        public static string Get(string url)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            try
            {
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                stream.Close();
            }
            return result;
        }
        /// <summary>
        /// 带Headers请求头Http请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Send(string url, string method)
        {
            string result = "";
            try
            {
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create(url);
                rq.Method = method;
                SetHeaderValue(rq.Headers, "Host", url);
                SetHeaderValue(rq.Headers, "Connection", "keep-alive");
                SetHeaderValue(rq.Headers, "Accept", "application/json, text/javascript, */*; q=0.01");
                SetHeaderValue(rq.Headers, "X-Requested-With", "XMLHttpRequest");
                SetHeaderValue(rq.Headers, "User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36 MicroMessenger/6.5.2.501 NetType/WIFI WindowsWechat QBCore/3.43.901.400 QQBrowser/9.0.2524.400");
                SetHeaderValue(rq.Headers, "Referer", url);
                SetHeaderValue(rq.Headers, "Accept-Encoding", "gzip, deflate");
                SetHeaderValue(rq.Headers, "Accept-Language", " zh-CN,zh;q=0.8,en-us;q=0.6,en;q=0.5;q=0.4");
                //SetHeaderValue(rq.Headers, "Cookie", "This is Cookie");

                HttpWebResponse resp = (HttpWebResponse)rq.GetResponse();
                using (Stream stream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                //失败就用普通方式尝试获取
                result = Get(url);
            }

            return result;
        }

        //设置http请求头的值
        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
    }
}
