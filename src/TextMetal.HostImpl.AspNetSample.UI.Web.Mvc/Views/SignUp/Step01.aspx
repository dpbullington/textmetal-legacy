<%@ Page Title="Sign Up Step 1 of 2: Create a Family" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.SignUpViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">
	
	<script type="text/javascript">
		$(document).ready(function() {
			$("#FamilyName").focus();
		});
	</script>

	<p>Creating a family requires the following information.</p>

	<% using (this.Html.BeginForm())
	   { %>
		<%= this.Html.ValidationSummary(true) %>
		<div class="txtmtl-data-form">
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.FamilyName, "Family Name") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.FamilyName, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.FamilyName) %>
			</div>
			
			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.FamilyAddressLine1, "Family Address Line 1") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.FamilyAddressLine1, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.FamilyAddressLine1) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.FamilyAddressLine2, "Family Address Line 2") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.FamilyAddressLine2, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.FamilyAddressLine2) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.FamilyAddressLine3, "Family Address Line 3") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.FamilyAddressLine3, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.FamilyAddressLine3) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.FamilyCityCountyLocality, "Family City/County/Locality") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.FamilyCityCountyLocality, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.FamilyCityCountyLocality) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.FamilyStateProvince, "Family State/Province") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.FamilyStateProvince, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.FamilyStateProvince) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.FamilyZipPostalCode, "Family Zip/Postal Code") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.FamilyZipPostalCode, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.FamilyZipPostalCode) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.FamilyCountryTerritory, "Family Country/Territory") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.FamilyCountryTerritory, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.FamilyCountryTerritory) %>
			</div>

			<div class="txtmtl-data-label">
				<%= this.Html.LabelFor(m => m.FamilyVoiceTelephoneNumber, "Family Voice Telephone Number") %>
			</div>
			<div class="txtmtl-data-field">
				<%= this.Html.TextBoxFor(m => m.FamilyVoiceTelephoneNumber, new { @class = "txtmtl-input-text" }) %>
				<%= this.Html.ValidationMessageFor(m => m.FamilyVoiceTelephoneNumber) %>
			</div>

			<div class="txtmtl-data-button">
				<input type="submit" class="txtmtl-input-button" value="Next >>" id="Save" />
			</div>
		</div>
	<% } %>
	
</asp:Content>