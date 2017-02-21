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
using System.Reflection;
using System.Data;
namespace CRL.Package.Category
{
    public class CategoryBusiness<TType, TModel> : CategoryBusiness<TModel>
        where TModel : Category, new()
    {
        public static CategoryBusiness<TType, TModel> Instance
        {
            get { return new CategoryBusiness<TType, TModel>(); }
        }
    }
    /// <summary>
    /// 分类维护
    /// </summary>
    public class CategoryBusiness<TModel> : BaseProvider<TModel>
        where TModel : Category,new()
    {

        public IEnumerable<TModel> GetAllCache(int dataType)
        {
            return AllCache.Where(b => b.DataType == dataType);
        }
        public string MakeNewCode(string parentSequenceCode, TModel category)
        {
            var helper = DBExtend;
            string newCode = parentSequenceCode + "";
            #region 生成新编码

            var list = QueryList(b => b.ParentCode == parentSequenceCode && b.DataType == category.DataType).OrderByDescending(b => b.SequenceCode).ToList();
            if (list.Count() == 0)
            {
                newCode += "01";
            }
            else
            {
                int len = !string.IsNullOrEmpty(parentSequenceCode) ? parentSequenceCode.Length : 0;
                string max = list[0].SequenceCode;
                max = max.Substring(len, 2);
                int n = int.Parse(max);
                if (n >= 99)
                {
                    throw new Exception("子级分类已到最大级99");
                }
                newCode += (n + 1).ToString().PadLeft(2, '0');
            }
            #endregion
            return newCode;
        }
        /// <summary>
        /// 指定父级,添加分类
        /// 如果父级为空,则为第一级
        /// </summary>
        /// <param name="parentSequenceCode"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public TModel Add(string parentSequenceCode, TModel category)
        {
            var helper = DBExtend;
            string newCode = MakeNewCode(parentSequenceCode, category);
            //helper.Clear();
            category.SequenceCode = newCode;
            category.ParentCode = parentSequenceCode;
            helper.InsertFromObj( category);
            //ClearCache();
            return category;
        }
        /// <summary>
        /// 获取一个分类
        /// </summary>
        /// <param name="sequenceCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public TModel Get(string sequenceCode, int type) 
        {
            return GetAllCache(type).Where(b => b.SequenceCode == sequenceCode).FirstOrDefault();
        }
        public IEnumerable<TModel> GetRoot(int type)
        {
            return GetAllCache(type).Where(b => b.ParentCode == "");
        }

        /// <summary>
        /// 指定代码和类型删除
        /// 会删除下级
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequenceCode"></param>
        /// <param name="type"></param>
        public void Delete(string sequenceCode,int type)
        {
            Delete(b => (b.SequenceCode == sequenceCode || b.ParentCode == sequenceCode) && b.DataType == type);
            ClearCache();
        }
        /// <summary>
        /// 获取子分类
        /// </summary>
        /// <param name="parentSequenceCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<TModel> GetChild(string parentSequenceCode, int type)
        {
            return GetAllCache(type).Where(b => b.ParentCode == parentSequenceCode).OrderByDescending(b => b.Sort).ToList();
        }

        /// <summary>
        /// 获取所有父级串
        /// </summary>
        /// <param name="sequenceCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<TModel> GetParents(string sequenceCode, int type)
        {
            List<string> list = new List<string>();
            List<TModel> list2 = new List<TModel>();
            int i = 0;
            while (i <= sequenceCode.Length)
            {
                list.Add(sequenceCode.Substring(0, i));
                i += 2;
            }
            foreach(string s in list)
            {
                TModel item = Get(s, type);
                if (item != null)
                {
                    list2.Add(item);
                }
            }
            return list2;
        }
        public string GetSelectOption(IEnumerable<TModel> list, string value)
        {
            string html = "<option value=''>请选择..</option>";
            string[] colorArry = new string[] { "#CCCCFF", "#CCCCCC", "#CCCC99", "#CCCC66", "#CCCC33", "#CCCC00", "#33CCFF", "#33CCCC", "#33CC99", "#33CC66", "#33CC33", "#33CC00" };
            foreach (var item in list)
            {
                int n = item.SequenceCode.Length / 2;
                int a = Convert.ToInt32(item.SequenceCode.Substring(0, 2));
                string color = "";
                if (a < colorArry.Length)
                {
                    color = colorArry[a];
                }
                string padding = "";
                for (int i = 1; i < n; i++)
                {
                    padding += "&nbsp;&nbsp;&nbsp;&nbsp;";
                }
                string text = string.Format("{0}{1}", padding, item.Name);
                html += string.Format("<option style='background-color:" + color + "' value='{0}' {1}>{2}</option>", item.SequenceCode, item.SequenceCode == value ? "selected" : "", text);
            }
            return html;
        }
    }
}
