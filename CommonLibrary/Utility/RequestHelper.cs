using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace CommonLibrary.Utility
{
    public class RequestHelper
    {
        public static string GetRequest(string url, int timeout)
        {
            return GetRequest(url, "", "POST", "text/xml", timeout);
        }
        public static string GetRequest(string url, string post_data, int timeout)
        {
            return GetRequest(url, post_data, "POST", "text/xml", timeout);
        }
        public static string GetRequest(string url, string post_data, string method, string content_type, int timeout)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            if (timeout > 0) req.Timeout = timeout;
            byte[] request_bytes = System.Text.Encoding.UTF8.GetBytes(post_data);
            req.Method = method;//"POST";
            req.ContentType = content_type;//"text/xml,application/x-www-form-urlencoded
            req.ContentLength = request_bytes.Length;
            Stream request_stream = req.GetRequestStream();
            request_stream.Write(request_bytes, 0, request_bytes.Length);
            request_stream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
            string ret = sr.ReadToEnd();
            sr.Close();
            res.Close();
            return ret;
        }

        public static string PostModel(string url, string param, int timeout)
        {
            return PostModel(url, param, Encoding.UTF8, timeout);
        }
        public static string PostModel(string url, string param, Encoding encoding,int timeout)
        {
            Encoding encode = encoding;
            byte[] arrB = encode.GetBytes(param);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            if (timeout > 0) myReq.Timeout = timeout;
            myReq.Method = "POST";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.ContentLength = arrB.Length;
            Stream outStream = myReq.GetRequestStream();
            outStream.Write(arrB, 0, arrB.Length);
            outStream.Close();
            WebResponse myResp = null;
            try
            {
                myResp = myReq.GetResponse();
            }
            catch
            {

            }
            Stream ReceiveStream = myResp.GetResponseStream();
            StreamReader readStream = new StreamReader(ReceiveStream, encode);
            Char[] read = new Char[256];
            int count = readStream.Read(read, 0, 256);
            string str = null;
            while (count > 0)
            {
                str += new String(read, 0, count);
                count = readStream.Read(read, 0, 256);
            }
            readStream.Close();
            myResp.Close();
            return str;
        }
    }
}
