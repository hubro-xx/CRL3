/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;

/**
* 商户基础 配置
* @author guoyx e-mail:guoyx@lianlian.com
* @date:2013-6-25 下午01:45:40
* @version :1.0
*
*/

namespace CRL.Package.OnlinePay.Company.Lianlian
{
	public class PartnerConfig
	{

		// RSA银通公钥
        public static string YT_PUB_KEY
        {
            get
            {
                var file = CoreHelper.CustomSetting.GetConfigKey("连连公钥文件");
                return System.IO.File.ReadAllText(file);
            }
        }
		// RSA商户私钥
        public static string TRADER_PRI_KEY
        {
            get
            {
                var file = CoreHelper.CustomSetting.GetConfigKey("连连私钥文件");
                return System.IO.File.ReadAllText(file);
            }
        }
		// MD5 KEY
        public static string MD5_KEY
        {
            get
            {
                return ChargeConfig.GetConfigKey(CompanyType.连连, ChargeConfig.DataType.Key);
            }
        }

		// 商户编号
        public static string OID_PARTNER
        {
            get
            {
                return ChargeConfig.GetConfigKey(CompanyType.连连, ChargeConfig.DataType.User);
            }
        }
		// 签名方式 RSA或MD5
		public static string            SIGN_TYPE="MD5";    					//请选择签名方式
		// 接口版本号，固定1.0
		public static string VERSION        = "1.0";
		// 业务类型，连连支付根据商户业务为商户开设的业务类型； （101001：虚拟商品销售、109001：实物商品销售、108001：外部账户充值）
        public static string BUSI_PARTNER = "101001";   //请选择业务类型
	}
}

