<%@ Page Title="Forgot Password" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.UserViewModel>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<script type="text/javascript">
		$(document).ready(function() {
			$("#Username").focus();
		});
	</script>

	<p>Please enter the following information in order to have a password reset email sent to your registered email address.</p>

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.Username, "Username") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.Username, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.Username) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.UsernameConfirm, "Username (confirm)") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.UsernameConfirm, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.UsernameConfirm) %>
			</div>
			
			<div class="txtmtl-data-button">
				<input type="submit" class="txtmtl-input-button" value="Submit" id="Submit" />
			</div>
		</div>
	<% } %>
	
	<div class="txtmtl-ctxnav-wrapper">		
		<%= this.Html.ActionLink("Forgot username", "ForgotUsername", "User") %>
	</div>
</asp:Content>