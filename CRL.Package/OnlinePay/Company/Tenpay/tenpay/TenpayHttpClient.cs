namespace tenpay
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.RegularExpressions;

    public class TenpayHttpClient
    {
        private string caFile = "";
        private string certFile = "";
        private string certPasswd = "";
        private string charset = "gb2312";
        private string errInfo = "";
        private string method = "POST";
        private string reqContent = "";
        private string resContent = "";
        private int responseCode = 0;
        private int timeOut = 60;

        public bool call()
        {
            StreamReader reader = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            try
            {
                string s = null;
                if (this.method.ToUpper() == "POST")
                {
                    string[] strArray = Regex.Split(this.reqContent, @"\?");
                    request = (HttpWebRequest) WebRequest.Create(strArray[0]);
                    if (strArray.Length >= 2)
                    {
                        s = strArray[1];
                    }
                }
                else
                {
                    request = (HttpWebRequest) WebRequest.Create(this.reqContent);
                }
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.CheckValidationResult);
                if (this.certFile != "")
                {
                    request.ClientCertificates.Add(new X509Certificate2(this.certFile, this.certPasswd));
                }
                request.Timeout = this.timeOut * 0x3e8;
                Encoding encoding = Encoding.GetEncoding(this.charset);
                if (s != null)
                {
                    byte[] bytes = encoding.GetBytes(s);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = bytes.Length;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                response = (HttpWebResponse) request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), encoding);
                this.resContent = reader.ReadToEnd();
                reader.Close();
                response.Close();
            }
            catch (Exception exception)
            {
                this.errInfo = this.errInfo + exception.Message;
                if (response != null)
                {
                    this.responseCode = Convert.ToInt32(response.StatusCode);
                }
                return false;
            }
            this.responseCode = Convert.ToInt32(response.StatusCode);
            return true;
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public string getErrInfo()
        {
            return this.errInfo;
        }

        public string getResContent()
        {
            return this.resContent;
        }

        public int getResponseCode()
        {
            return this.responseCode;
        }

        public void setCaInfo(string caFile)
        {
            this.caFile = caFile;
        }

        public void setCertInfo(string certFile, string certPasswd)
        {
            this.certFile = certFile;
            this.certPasswd = certPasswd;
        }

        public void setCharset(string charset)
        {
            this.charset = charset;
        }

        public void setMethod(string method)
        {
            this.method = method;
        }

        public void setReqContent(string reqContent)
        {
            this.reqContent = reqContent;
        }

        public void setTimeOut(int timeOut)
        {
            this.timeOut = timeOut;
        }
    }
}

