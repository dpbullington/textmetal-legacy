<%@ Page Title="Sign Up Step 1 of 2: Create an Organization" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.SignUpViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">
	
	<script type="text/javascript">
		$(document).ready(function() {
			$("#OrganizationName").focus();
		});
	</script>

	<p>Creating an organization requires the following information.</p>

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.OrganizationName, "Organization Name") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.OrganizationName, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.OrganizationName) %>
			</div>
			
			<div class="txtmtl-data-button">
				<input type="submit" class="txtmtl-input-button" value="Next >>" id="Save" />
			</div>
		</div>
	<% } %>
	
</asp:Content>