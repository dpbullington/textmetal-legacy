<%@ Page Title="Edit a Circle" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.DomainModel.Tables.Circle>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">

			<div class="txtmtl-data-label">
				Circle Name
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.CircleName, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.CircleName) %>
			</div>

			<div class="txtmtl-data-label">
				Circle Description
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextAreaFor(m => m.CircleDesc, new { @class = "txtmtl-input-textarea" }) %>
				<%= this.Html.ValidationMessageFor(m => m.CircleDesc) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.PasswordClearText, "Shared Secret (required for other families to join this circle)") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.PasswordFor(m => m.PasswordClearText, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.PasswordClearText) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.PasswordClearTextConfirm, "Shared Secret (confirm)") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.PasswordFor(m => m.PasswordClearTextConfirm, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.PasswordClearTextConfirm) %>
			</div>

			<div class="txtmtl-data-button">
				<input type="submit" class="txtmtl-input-button" value="Save" />
			</div>
		</div>
	<% } %>

	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Back to circle list", "List", "Circle") %><br />
		<%= this.Html.ActionLink("Return to circle dashboard", "Dashboard", "Circle") %>
	</div>

</asp:Content>
