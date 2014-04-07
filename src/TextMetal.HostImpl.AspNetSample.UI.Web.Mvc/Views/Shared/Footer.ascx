<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.TextMetalViewModel>" %>
<div class="txtmtl-footer">
	<%= this.Html.Encode(this.Model.AssemblyInformation.Copyright) %><br />
	Release version: <%= this.Html.Encode(this.Model.AssemblyInformation.InformationalVersion) %>; Build version: <%= this.Html.Encode(this.Model.AssemblyInformation.AssemblyVersion) %> / <%= this.Html.Encode(this.Model.AssemblyInformation.Configuration) %>
</div>