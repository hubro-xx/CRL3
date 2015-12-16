using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
namespace CRL
{
    /// <summary>
    /// 对象结构信息导出
    /// </summary>
    public class SummaryAnalysis
    {
        public class FieldItem
        {
            public string Remark;
            public string Name;
            public string ModelRemark;
            public Type Type;
        }
        public static List<Type> GetInfoFromDll(Type[] currentTypes)
        {
            List<Type> findTypes = new List<Type>();
            Dictionary<string, List<FieldItem>> fileds = new Dictionary<string, List<FieldItem>>();
            foreach (var currentType in currentTypes)
            {
                #region 加载类型
                var assembyle = System.Reflection.Assembly.GetAssembly(currentType);
                Type[] types = assembyle.GetTypes();
                var typeCRL = typeof(CRL.IModel);
                foreach (var type in types)
                {
                    if (type.IsClass)
                    {
                        var type1 = type.BaseType;
                        while (type1 != null)
                        {
                            if (type1.BaseType == typeCRL || type1.FullName == typeCRL.FullName)
                            {
                                findTypes.Add(type);
                                break;
                            }
                            type1 = type1.BaseType;
                        }
                    }
                }
                #endregion
            }
            return findTypes.OrderBy(b => b.Name).ToList();
        }
        public static Dictionary<string, List<FieldItem>> GetInfoFromXml(string[] xmlFiles)
        {
            Dictionary<string, List<FieldItem>> list = new Dictionary<string, List<FieldItem>>();
            if (xmlFiles == null)
            {
                return list;
            }
            if (xmlFiles.Length == 0)
            {
                return list;
            }
            foreach (string xmlFile in xmlFiles)
            {
                var rootE = XElement.Load(xmlFile);
                IEnumerable<XElement> query =
                                            from ele in rootE.Element("members").Elements("member")
                                            where ele.Attribute("name").Value.StartsWith("P:")
                                            select ele;
                foreach (XElement e in query)
                {
                    string name = e.Attribute("name").Value.Substring(2);
                    int index = name.LastIndexOf('.');
                    string propertyName = name.Substring(index + 1, name.Length - index - 1);
                    string typeName = name.Substring(0, index);
                    var summary = e.Element("summary");
                    string remark = "";
                    if (summary != null)
                    {
                        remark = summary.Value.Trim();
                    }
                    if (!list.ContainsKey(typeName))
                    {
                        list.Add(typeName, new List<FieldItem>());
                    }
                    list[typeName].Add(new FieldItem() { Name = propertyName, Remark = remark });
                }
                //找对象注释
                IEnumerable<XElement> query2 =
                                            from ele in rootE.Element("members").Elements("member")
                                            where ele.Attribute("name").Value.StartsWith("T:")
                                            select ele;
                foreach (XElement e in query2)
                {
                    string name = e.Attribute("name").Value.Substring(2);
                    var summary = e.Element("summary");
                    string remark = "";
                    if (summary != null)
                    {
                        remark = summary.Value.Trim();
                    }
                    if (list.ContainsKey(name))
                    {
                        list[name].ForEach(b =>
                        {
                            b.ModelRemark = remark;
                        });
                    }
                }
            }
            return list;
        }

        public static Dictionary<Type, List<CRL.Attribute.FieldAttribute>> Merge(List<Type> types,Dictionary<string, List<FieldItem>> fields)
        {
            var result = new Dictionary<Type, List<CRL.Attribute.FieldAttribute>>();
            foreach(var type in types)
            {
                string typeName = type.FullName;
                var list2 = CRL.TypeCache.GetProperties(type, true);
                if (fields.ContainsKey(typeName))
                {
                    var list = fields[typeName];
                    foreach (var item in list)
                    {
                        var item2 = list2[item.Name];
                        if (item2 != null)
                        {
                            item2.Remark = item.Remark;
                            item2.ModelRemark = item.ModelRemark;
                        }
                    }
                }
                else
                {
                    fields.Remove(typeName);
                }
                result.Add(type, list2.Values.ToList());
            }
            return result;
        }

        public static string ExportToFile(Type[] currentTypes, string[] xmlFiles)
        {
            var a = GetInfoFromDll(currentTypes);
            var b = GetInfoFromXml(xmlFiles);
            var c = Merge(a, b);
            StringBuilder sb = new StringBuilder("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>");
            foreach (var item in c)
            {
                var tableName = CRL.TypeCache.GetTableName(item.Key,null);
                sb.Append(@"<table border='1' cellpadding='4' cellspacing='1' style='width:100%'>
  <tr>
    <td colspan='5'><b>" + tableName + "[" + item.Key.FullName + "]</b>(" + item.Value[0].ModelRemark + ")</td></tr><tr><th>名称</th><th>类型</th><th>长度</th><th>索引</th><th>备注</th></tr>");
                var list = item.Value;
                foreach (var p in list)
                {
                    sb.Append(@"<tr><td width='150'>" + p.Name + "</td><td  width='220'>" + p.PropertyType + "</td><td  width='40'>" + p.Length + "</td><td  width='40'>" + p.FieldIndexType + "</td><td>" + p.Remark + "</td></tr>");
                }
                sb.Append("</table>");
            }
            string saveFile = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("/model{0}.html", currentTypes[0].Name));
            System.IO.File.WriteAllText(saveFile, sb.ToString());
            System.Diagnostics.Process.Start(saveFile);
            return sb.ToString(); 
        }
    }
}
