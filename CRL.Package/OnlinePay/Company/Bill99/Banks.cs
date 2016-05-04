/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.OnlinePay.Company.Bill99
{
    public class Banks
    {
        public static Dictionary<string, string> AllBanks
        {
            get
            {
                var dic = new Dictionary<string, string>();
                dic.Add("CMB", "招商银行");
                dic.Add("ICBC", "中国工商银行");
                dic.Add("ABC", "中国农业银行");
                dic.Add("CCB", "中国建设银行");
                dic.Add("BOC", "中国银行");
                dic.Add("SPDB", "浦发银行");
                dic.Add("BCOM", "中国交通银行");
                dic.Add("CMBC", "中国民生银行");
                dic.Add("SDB", "深圳发展银行");
                dic.Add("GDB", "广东发展银行");
                dic.Add("CITIC", "中信银行");
                dic.Add("HXB", "华夏银行");
                dic.Add("CIB", "兴业银行");
                dic.Add("GZRCC", "广州农村商业银行");
                dic.Add("GZCB", "广州银行");
                dic.Add("CUPS", "中国银联");
                dic.Add("SRCB", "上海农村商业银行");
                dic.Add("POST", "中国邮政");
                dic.Add("BOB", "北京银行");
                dic.Add("CBHB", "渤海银行");
                dic.Add("BJRCB", "北京农商银行");
                dic.Add("NJCB", "南京银行");
                dic.Add("CEB", "中国光大银行");
                dic.Add("BEA", "东亚银行");
                dic.Add("NBCB", "宁波银行");
                dic.Add("HZB", "杭州银行");
                dic.Add("PAB", "平安银行");
                dic.Add("HSB", "徽商银行");
                dic.Add("CZB", "浙商银行");
                dic.Add("SHB", "上海银行");
                dic.Add("PSBC", "中国邮政储蓄银行");
                dic.Add("JSB", "江苏银行");
                dic.Add("DLB", "大连银行");
                return dic;
            }
        }
    }
}
