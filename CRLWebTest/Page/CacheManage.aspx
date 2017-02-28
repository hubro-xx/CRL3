<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CacheManage.aspx.cs" Inherits="WebTest.Page.CacheManage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style2 {
            width: 70px;
        }

        .auto-style3 {
            width: 56px;
        }

        .auto-style4 {
            width: 93px;
        }

        .auto-style5 {
            width: 133px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>以下演示了如何管理缓存</h4>
    <blockquote>
        创建的缓存会由后台线程自动进行更新,当然也可以通过以下示例手动管理
    </blockquote>
    <table border="1" style="width:100%">
    <tr>
        <td class="auto-style5">KEY</td>
        <td class="auto-style2">数据类型</td>
        <td class="auto-style3">过期(分)</td>
        <td class="auto-style4">上次更新</td>
        <td width="50">行数</td>
        <td width="200">查询</td>
        <td width="100">参数</td>
        <td width="40">操作</td>
    </tr>
    <%
        foreach (var item in caches)
        {
    %>
    <tr>
        <td class="auto-style5"><%=item.Key %></td>
        <td class="auto-style2"><%=item.DataType %> [<%=item.DatabaseName %>]</td>
        <td class="auto-style3"><%=item.TimeOut %></td>
        <td class="auto-style4"><%=item.UpdateTime %></td>
        <td><%=item.RowCount %></td>
        <td><%=item.TableName %> </td>
        <td><%=item.Params %></td>
        <td><a href="?type=update&key=<%=item.Key %>" target="_blank">更新</a></td>
    </tr>
    <%} %>
</table>
    获取缓存列表
    <pre>
        caches = CRL.MemoryDataCache.CacheService.GetCacheList();
    </pre>
    通过KEY更新缓存
    <pre>
        string key = Request["key"];
        var a = CRL.MemoryDataCache.CacheService.UpdateCache(key);
    </pre>
    <h3>缓存变量信息</h3>
    <table border="1">
        <tr>
            <td>名称</td>
            <td>数量</td>
        </tr>
        <% foreach (var item in tempCache)
           {%>
        <tr>
            <td><%=item.Key %></td>
            <td><%=item.Value %></td>
        </tr>
        <% }%>
    </table>
</asp:Content>
