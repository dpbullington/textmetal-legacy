<%@ Page Title="Edit User Account" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.UserViewModel>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<script type="text/javascript">
		$(document).ready(function() {
			$("#EmailAddress").focus();
		});
	</script>

	<p>Editing an existing user account requires the following information.</p>

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.EmailAddress, "Email Address") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.EmailAddress, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.EmailAddress) %>
			</div>
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.EmailAddressConfirm, "Email Address (confirm)") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.EmailAddressConfirm, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.EmailAddressConfirm) %>
			</div>

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

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.PasswordClearText, "Password") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.PasswordFor(m => m.PasswordClearText, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.PasswordClearText) %>
			</div>
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.PasswordClearTextConfirm, "Password (confirm)") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.PasswordFor(m => m.PasswordClearTextConfirm, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.PasswordClearTextConfirm) %>
			</div>
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.SecurityQuestion, "Security Question") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.SecurityQuestion, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.SecurityQuestion) %>
			</div>
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.SecurityQuestionConfirm, "Security Question (confirm)") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.SecurityQuestionConfirm, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.SecurityQuestionConfirm) %>
			</div>
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.SecurityAnswerClearText, "Security Answer") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.SecurityAnswerClearText, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.SecurityAnswerClearText) %>
			</div>
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.SecurityAnswerClearTextConfirm, "Security Answer (confirm)") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.SecurityAnswerClearTextConfirm, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.SecurityAnswerClearTextConfirm) %>
			</div>

			<div class="txtmtl-data-button">			
				<input type="submit" class="txtmtl-input-button" value="Save" id="Save" />
			</div>
		</div>
	<% } %>
		
	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Return to home", "Index", "Welcome") %>
	</div>
</asp:Content>