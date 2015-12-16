using CRL.LambdaQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.Advert
{
    /// <summary>
    /// 广告管理
    /// </summary>
    public class AdvertBusiness<TType> : BaseProvider<Advert> where TType : class
    {
        public static AdvertBusiness<TType> Instance
        {
            get { return new AdvertBusiness<TType>(); }
        }

        protected override LambdaQuery<Advert> CacheQuery()
        {
            return base.CacheQuery().Where(b => b.Disable == false);
        }
        public Advert QueryItem(int id)
        {
            var items = AllCache.Where(b => b.Id == id);
            if (items.Count()==0)
            {
                var item = new Advert();
                item.Title = "尚未添加";
                return item;
            }
            return items.First();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="categoryCode"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public List<Advert> Query(string categoryCode, int top, bool checkDate = false)
        {
            //ParameCollection c = new ParameCollection();
            //c.SetQuerySortField("Sort");
            //c.SetQuerySortType(true);
            //c["categoryCode"] = categoryCode;
            //c["Disable"] = 0;
            //c.SetCacheTime(10);
            //c.SetQueryTop(top);
            //List<IAdvert> list = QueryList<IAdvert>(c);
            //return list;
            var time = DateTime.Now;
            List<Advert> list;
            if (checkDate)
            {
                list = AllCache.Where(b => b.CategoryCode == categoryCode && b.BeginTime < time && b.EndTime > time && b.Disable == false).Skip(0).Take(top).OrderByDescending(b => b.Sort).ToList();
            }
            else
            {
                list = AllCache.Where(b => b.CategoryCode == categoryCode && b.Disable == false).Skip(0).Take(top).OrderByDescending(b => b.Sort).ToList();
            }
            return list.ToList();
        }
    }
}
