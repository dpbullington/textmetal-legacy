<%@ Page Title="Signing Up: Start" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.SignUpViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<p>It is easy and free to begin using TextMetal. This process will guide you through the two easy sign-up steps below. Once you are setup with a family and a parent, you can then explore the remaining customization options inside of TextMetal.</p>
	
	<p style="margin-top: 32px" class="txtmtl-callout-text">TextMetal sign-up</p>
	
	<ol>
		<li>Create a family (the container for parents and children).</li>
		<li>Create a parent (user) which manages a family.</li>
	</ol>

	<p style="margin-top: 32px" class="txtmtl-callout-text">After sign-up is complete, you can further customize TextMetal to your needs:</p>
	<ol>
		<li>Create additional parents in the family.</li>
		<li>Create one or more children in the family.</li>
		<li>Create or join a trusted play circle.</li>
		<li>Send and respond to play requests.</li>
	</ol>
	
	<a href="<%= this.Url.Action("Step01", "SignUp") %>"><img style="margin: 2px 2px 2px 2px" src="<%= this.Url.Content("~/images/lets-begin-button.png") %>" alt="Let's begin the sign up process." /></a>
	
	<p>By signing up, you agree to our <%= this.Html.ActionLink("License Agreement", "License", "Welcome") %> and <%= this.Html.ActionLink("Privacy Policy", "Privacy", "Welcome") %>.</p>
</asp:Content>