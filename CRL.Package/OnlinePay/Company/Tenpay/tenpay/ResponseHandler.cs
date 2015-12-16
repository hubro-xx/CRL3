namespace tenpay
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Text;
    using System.Web;

    public class ResponseHandler
    {
        private string debugInfo;
        protected HttpContext httpContext;
        private string key;
        protected Hashtable parameters;

        public ResponseHandler(HttpContext httpContext)
        {
            NameValueCollection form;
            this.parameters = new Hashtable();
            this.httpContext = httpContext;
            if (this.httpContext.Request.HttpMethod == "POST")
            {
                form = this.httpContext.Request.Form;
            }
            else
            {
                form = this.httpContext.Request.QueryString;
            }
            foreach (string str in form)
            {
                string parameterValue = form[str];
                this.setParameter(str, parameterValue);
            }
        }

        public virtual bool _isTenpaySign(ArrayList akeys)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in akeys)
            {
                string strB = (string) this.parameters[str];
                if ((((strB != null) && ("".CompareTo(strB) != 0)) && ("sign".CompareTo(str) != 0)) && ("key".CompareTo(str) != 0))
                {
                    builder.Append(str + "=" + strB + "&");
                }
            }
            builder.Append("key=" + this.getKey());
            string str3 = MD5Util.GetMD5(builder.ToString(), this.getCharset()).ToLower();
            this.setDebugInfo(builder.ToString() + " => sign:" + str3);
            return this.getParameter("sign").ToLower().Equals(str3);
        }

        public void doShow(string show_url)
        {
            string s = "<html><head>\r\n<meta name=\"TENCENT_ONLINE_PAYMENT\" content=\"China TENCENT\">\r\n<script language=\"javascript\">\r\nwindow.location.href='" + show_url + "';\r\n</script>\r\n</head><body></body></html>";
            this.httpContext.Response.Write(s);
            this.httpContext.Response.End();
        }

        protected virtual string getCharset()
        {
            return this.httpContext.Request.ContentEncoding.BodyName;
        }

        public string getDebugInfo()
        {
            return this.debugInfo;
        }

        public string getKey()
        {
            return this.key;
        }

        public string getParameter(string parameter)
        {
            string str = (string) this.parameters[parameter];
            return ((str == null) ? "" : str);
        }

        public virtual bool isTenpaySign()
        {
            StringBuilder builder = new StringBuilder();
            ArrayList list = new ArrayList(this.parameters.Keys);
            list.Sort();
            foreach (string str in list)
            {
                string strB = (string) this.parameters[str];
                if ((((strB != null) && ("".CompareTo(strB) != 0)) && ("sign".CompareTo(str) != 0)) && ("key".CompareTo(str) != 0))
                {
                    builder.Append(str + "=" + strB + "&");
                }
            }
            builder.Append("key=" + this.getKey());
            string str3 = MD5Util.GetMD5(builder.ToString(), this.getCharset()).ToLower();
            this.setDebugInfo(builder.ToString() + " => sign:" + str3);
            return this.getParameter("sign").ToLower().Equals(str3);
        }

        protected void setDebugInfo(string debugInfo)
        {
            this.debugInfo = debugInfo;
        }

        public void setKey(string key)
        {
            this.key = key;
        }

        public void setParameter(string parameter, string parameterValue)
        {
            if ((parameter != null) && (parameter != ""))
            {
                if (this.parameters.Contains(parameter))
                {
                    this.parameters.Remove(parameter);
                }
                this.parameters.Add(parameter, parameterValue);
            }
        }
    }
}

