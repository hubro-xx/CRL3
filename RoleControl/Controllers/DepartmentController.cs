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
using System.Web;
using System.Web.Mvc;
using CRL.Package.RoleAuthorize;
namespace RoleControl.Controllers
{
    [Authorize]
    public class DepartmentController : RoleControl.Models.BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Part(string parentCode = "", int dataType = 0)
        {
            var list = CRL.Package.RoleAuthorize.DepartmentBusiness.Instance.QueryList(b => b.ParentCode == parentCode && b.DataType == dataType).OrderByDescending(b => b.Sort).ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult Add(string name, string parentCode = "", int dataType = 0)
        {

            var arry = name.Split(',');
            foreach (var item in arry)
            {
                Department c = new Department();
                c.Name = item.Trim();
                c.DataType = dataType;
                c.ParentCode = parentCode;
                CRL.Package.RoleAuthorize.DepartmentBusiness.Instance.Add(parentCode, c);
            }
            return JsonResult(true, "");
        }
        [HttpPost]
        public ActionResult Save(CRL.Package.RoleAuthorize.Department Department)
        {
            CRL.ParameCollection c = new CRL.ParameCollection();
            c["name"] = Department.Name;
            c["Disable"] = Department.Disable;
            c["Sort"] = Department.Sort;
            CRL.Package.RoleAuthorize.DepartmentBusiness.Instance.Update(b => b.SequenceCode == Department.SequenceCode && b.DataType == 0, c);
            return JsonResult(true, "");
        }
        [HttpPost]
        public ActionResult Delete(string code, int dataType = 0)
        {
            CRL.Package.RoleAuthorize.DepartmentBusiness.Instance.Delete(code, dataType);
            return JsonResult(true, "");
        }
        [HttpPost]
        public ActionResult GetDepartment(int dataType = 0, string value = "", string parent = "")
        {
            IEnumerable<Department> Department;
            if (!string.IsNullOrEmpty(parent))
            {
                Department = CRL.Package.RoleAuthorize.DepartmentBusiness.Instance.AllCache.Where(b => b.DataType == dataType && b.Disable == false && b.SequenceCode.StartsWith(parent) && b.SequenceCode != parent).OrderBy(b => b.SequenceCode);
            }
            else
            {
                Department = CRL.Package.RoleAuthorize.DepartmentBusiness.Instance.AllCache.Where(b => b.DataType == dataType && b.Disable == false).OrderBy(b => b.SequenceCode);
            }
            var html = CRL.Package.RoleAuthorize.DepartmentBusiness.Instance.GetSelectOption(Department, value);
            return Content(html);
        }

        public ActionResult Import(int dataType = 0)
        {
            return View();
        }
        class CItem
        {
            public CItem Pre;
            public int Level;
            public string Name;
            public string Code
            {
                get
                {
                    if (Pre == null)//当是第一个时
                    {
                        var item = CRL.Package.RoleAuthorize.DepartmentBusiness.Instance.QueryItem(b => b.ParentCode == "", true);
                        if (item == null)
                        {
                            return "01";
                        }
                        else
                        {
                            return (int.Parse(item.SequenceCode) + 1).ToString().PadLeft(2, '0');
                        }
                    }
                    string preCode = Pre.Code;
                    if (Pre.Level < Level)//当是第一个子级
                    {
                        return preCode + "01";
                    }
                    else if (Pre.Level == Level)//同级
                    {
                        int n = int.Parse(preCode.Substring(preCode.Length - 2, 2));
                        return preCode.Substring(0, preCode.Length - 2) + (n + 1).ToString().PadLeft(2, '0');
                    }
                    else//子级完毕
                    {
                        var code = preCode.Substring(0, (Level + 1) * 2);//同级CODE
                        int n = int.Parse(code.Substring(code.Length - 2, 2));
                        return code.Substring(0, code.Length - 2) + (n + 1).ToString().PadLeft(2, '0');
                    }
                }
            }
            public override string ToString()
            {
                return string.Format("{0} {1} {2}", Name, Level, Code);
            }
        }
        [HttpPost]
        public ActionResult Import(int dataType = 0, string content = "")
        {
            if (string.IsNullOrEmpty(content))
            {
                return View();
            }
            var arry = content.Split('\r');
            CItem last = null;
            var list = new List<Department>();
            foreach (var s in arry)
            {
                string name = s.TrimEnd();
                int length = name.Length;
                int length2 = name.Replace("\t", "").Length;
                int level = length - length2;
                CItem c = new CItem() { Level = level, Name = name, Pre = last };
                last = c;
                Department Department = new Department();
                Department.DataType = dataType;
                Department.Name = name.Trim();
                Department.SequenceCode = c.Code;
                Department.ParentCode = Department.SequenceCode.Substring(0, Department.SequenceCode.Length - 2);
                list.Add(Department);
            }
            CRL.Package.RoleAuthorize.DepartmentBusiness.Instance.BatchInsert(list);
            return AutoBackResult("导入成功");
        }
    }
}
