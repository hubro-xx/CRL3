using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.OnlinePay.Company.Lianlian
{
    public class LianlianService
    {
        public static T Request<T>(Message.MessageBase request) where T : Message.MessageBase, new()
        {
            request.oid_partner = PartnerConfig.OID_PARTNER;
            request.sign_type = PartnerConfig.SIGN_TYPE;
            request.SetSign();
            var data = CoreHelper.SerializeHelper.SerializerToJson(request);
            var json = CoreHelper.HttpRequest.HttpPost(request.InterFaceUrl, data, Encoding.UTF8, "application/json");
            return CoreHelper.SerializeHelper.SerializerFromJSON(json, typeof(T), Encoding.UTF8) as T;
        }
        /// <summary>
        /// 查询卡信息
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public static Message.BankCardQueryResponse QueryCard(string cardNo)
        {
            var request = new Message.BankCardQuery();
            request.card_no = cardNo;
            request.pay_type = "D";
            var result = Request<Message.BankCardQueryResponse>(request);
            return result;
        }
        /// <summary>
        /// 查询签约过的卡
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<Message.agreement_list> QueryBindCard(string userId)
        {
            var request = new Message.BindCardQuery();
            request.user_id = userId;
            request.pay_type = "D";
            var result = Request<Message.BindCardQueryResponse>(request);
            return result.agreement_list;
        }
        
    }
}
