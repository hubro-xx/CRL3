namespace tenpay
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Web;

    public class RequestHandler
    {
        private string debugInfo;
        private string gateUrl;
        protected HttpContext httpContext;
        private string key;
        protected Hashtable parameters = new Hashtable();

        public RequestHandler(HttpContext httpContext)
        {
            this.httpContext = httpContext;
            this.setGateUrl("https://www.tenpay.com/cgi-bin/v1.0/service_gate.cgi");
        }

        protected virtual void createSign()
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
            string parameterValue = MD5Util.GetMD5(builder.ToString(), this.getCharset()).ToLower();
            this.setParameter("sign", parameterValue);
            this.setDebugInfo(builder.ToString() + " => sign:" + parameterValue);
        }

        public void doSend()
        {
            this.httpContext.Response.Redirect(this.getRequestURL());
        }

        public Hashtable getAllParameters()
        {
            return this.parameters;
        }

        protected virtual string getCharset()
        {
            return this.httpContext.Request.ContentEncoding.BodyName;
        }

        public string getDebugInfo()
        {
            return this.debugInfo;
        }

        public string getGateUrl()
        {
            return this.gateUrl;
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

        public virtual string getRequestURL()
        {
            this.createSign();
            StringBuilder builder = new StringBuilder();
            ArrayList list = new ArrayList(this.parameters.Keys);
            list.Sort();
            foreach (string str in list)
            {
                string instr = (string) this.parameters[str];
                if (((instr != null) && ("key".CompareTo(str) != 0)) && ("spbill_create_ip".CompareTo(str) != 0))
                {
                    builder.Append(str + "=" + TenpayUtil.UrlEncode(instr, this.getCharset()) + "&");
                }
                else if ("spbill_create_ip".CompareTo(str) == 0)
                {
                    builder.Append(str + "=" + instr.Replace(".", "%2E") + "&");
                }
            }
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            return (this.getGateUrl() + "?" + builder.ToString());
        }

        public virtual void init()
        {
        }

        public void setDebugInfo(string debugInfo)
        {
            this.debugInfo = debugInfo;
        }

        public void setGateUrl(string gateUrl)
        {
            this.gateUrl = gateUrl;
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

