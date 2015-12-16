<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="TableExistsCache.aspx.cs" Inherits="WebTest.Page.TableCache" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>数据表创建缓存依赖</h4>
    <blockquote style="font-size:15px; line-height:25px; ">CRL会自动创建对象对应的表结构,并初始<br />
        CRL判断该不该创建对象对应的数据表,是由表缓存文件来判断,缓存文件路径/config/TableCache.config<br />
        系统运行时检查目录内有没有缓存,有则加载,没有则创建,在CRL内部检测表创建方法内,根据此缓存来判断,不存在表缓存,则创建表并保存缓存值<br />
        新增的对象,此时缓存内是没有的,则会创建数据表<br />
        在缓存结构和数据表结构一致的情况下,新增对象属性时,会自动创建数据表列

        <h5>缓存文件和数据库实际表结构在一些情况下可能不一致,则需要手动干预</h5>
         
  </blockquote>
    <ul>
                <li>当数据库有表,但字段不全,也没有缓存文件,CRL创建缓存文件时会按对象完整的结构缓存,此时缓存的表结构和数据库结构就不一致</li>
                <li>当缓存内有表,数据库没有表,CRL没法判断,不会自动创建表</li>
                <li>当缓存内表有所有表字段结构,数据库表字段被手动删掉,此时结构不一致</li>
                <li>当缓存文件被其它数据源生成的缓存文件覆盖了,可能产生结构不一致</li>
            </ul>
</asp:Content>
