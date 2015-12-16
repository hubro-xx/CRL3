using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using CoreHelper;
namespace CRL.Package.OnlinePay
{
	/// <summary>
	/// 支付公司类型
	/// </summary>
    public enum CompanyType
    {
        /// <summary>
        /// 财付通
        /// </summary>
        财付通 = 1,
        /// <summary>
        /// 支付宝
        /// </summary>
        支付宝 = 2,
        银联托管 = 3,
        快钱=4,
        连连=5,
        汇付天下=6,
        微信=7,
        支付宝WAP = 8
    }
}
