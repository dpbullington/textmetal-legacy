<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.TextMetalViewModel>" %>
<div class="txtmtl-channel-links">
	
	<% if (this.Model.UserWasAuthenticated)
	   { %>	
		TASKS: <%= this.Html.ActionLink("Dashboard", "Dashboard", "Meta") %>
        
		<%= this.Html.ActionLink("Account Settings", "Edit", "User") %> |
		<%= this.Html.ActionLink("Sign Out", "Logout", "User") %>
	<% }
	   else
	   { %>
	
		New parents: <%= this.Html.ActionLink("Sign Up", "Index", "SignUp") %> (free) -- or -- 
		Existing parents: <%= this.Html.ActionLink("Sign In", "Login", "User") %>

	<% } %>	

</div>