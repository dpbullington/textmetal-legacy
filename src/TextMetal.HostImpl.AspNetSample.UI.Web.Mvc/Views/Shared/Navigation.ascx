<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.TextMetalViewModel>" %>

<%= this.Html.ActionLink("Home", "Index", "Welcome", null, new { @class = "txtmtl-com-nav-link" }) %> |
<%= this.Html.ActionLink("About Us", "About", "Welcome", null, new { @class = "txtmtl-com-nav-link" }) %> |
<%= this.Html.ActionLink("Contact Us", "Contact", "Welcome", null, new { @class = "txtmtl-com-nav-link" }) %> |
<%= this.Html.ActionLink("License Agreement", "License", "Welcome", null, new { @class = "txtmtl-com-nav-link" }) %> |
<%= this.Html.ActionLink("Privacy Policy", "Privacy", "Welcome", null, new { @class = "txtmtl-com-nav-link" }) %>
