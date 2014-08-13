<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.TextMetalViewModel>" %>
<div class="txtmtl-channel-links">

	<% if (this.Model.UserWasAuthenticated)
	   { %>
		TASKS: <%= this.Html.ActionLink("Dashboard", "Dashboard", "Organization") %>
		|
		<%= this.Html.ActionLink("Account Settings", "Edit", "User") %> |
		<%= this.Html.ActionLink("Sign Out", "Logout", "User") %>
	<% }
	   else
	   { %>

		New members: <%= this.Html.ActionLink("Sign Up", "Index", "SignUp") %> (free) -- or --
		Existing members: <%= this.Html.ActionLink("Sign In", "Login", "User") %>

	<% } %>

</div>
