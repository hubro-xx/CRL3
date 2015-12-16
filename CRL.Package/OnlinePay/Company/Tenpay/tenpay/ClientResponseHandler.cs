namespace tenpay
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Xml;

    public class ClientResponseHandler
    {
        private string charset = "gb2312";
        protected string content;
        private string debugInfo;
        private string key;
        protected Hashtable parameters = new Hashtable();

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

        protected virtual string getCharset()
        {
            return this.charset;
        }

        public string getContent()
        {
            return this.content;
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

        public void setCharset(string charset)
        {
            this.charset = charset;
        }

        public virtual void setContent(string content)
        {
            this.content = content;
            XmlDocument document = new XmlDocument();
            document.LoadXml(content);
            XmlNodeList childNodes = document.SelectSingleNode("root").ChildNodes;
            foreach (XmlNode node2 in childNodes)
            {
                this.setParameter(node2.Name, node2.InnerXml);
            }
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

