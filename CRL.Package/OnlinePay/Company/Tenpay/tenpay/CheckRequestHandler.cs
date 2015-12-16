namespace tenpay
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Web;

    public class CheckRequestHandler : RequestHandler
    {
        public CheckRequestHandler(HttpContext httpContext) : base(httpContext)
        {
            base.setGateUrl("http://mch.tenpay.com/cgi-bin/mchdown_real_new.cgi");
        }

        protected override void createSign()
        {
            StringBuilder builder = new StringBuilder();
            ArrayList list = new ArrayList();
            list.Add("spid");
            list.Add("trans_time");
            list.Add("stamp");
            list.Add("cft_signtype");
            list.Add("mchtype");
            foreach (string str in list)
            {
                string strB = (string) base.parameters[str];
                if ((((strB != null) && ("".CompareTo(strB) != 0)) && ("sign".CompareTo(str) != 0)) && ("key".CompareTo(str) != 0))
                {
                    builder.Append(str + "=" + strB + "&");
                }
            }
            builder.Append("key=" + base.getKey());
            string parameterValue = MD5Util.GetMD5(builder.ToString(), this.getCharset()).ToLower();
            base.setParameter("sign", parameterValue);
            base.setDebugInfo(builder.ToString() + " => sign:" + parameterValue);
        }
    }
}

