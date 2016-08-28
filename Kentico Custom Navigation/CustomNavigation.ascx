<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomNavigation.ascx.cs" Inherits="CMSApp.CMSWebParts.Custom.CustomNavigation" %>
<asp:Repeater ID="repeaterNavigation" runat="server">
    <HeaderTemplate>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>
        <li><a href="<%#Eval("PageUrl") %>"><%#Eval("PageName")%> { <%#Eval("CategoryName") %> }</a></li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
