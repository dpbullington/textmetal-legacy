<%@ Page Title="Member List" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.MemberViewModel>" %>
<%@ Import Namespace="TextMetal.Common.Core" %>
<%@ Import Namespace="TextMetal.HostImpl.AspNetSample.DomainModel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">

	<table class="txtmtl-results">
		<tr class="txtmtl-results-header">
			<th>
				<%= this.Html.ActionLink("Full Name", "List", "Member", new { sort = "0" }, new { @class = "txtmtl-results-sortby" }) %>
			</th>
			<th>
				<%= this.Html.ActionLink("Username", "List", "Member", new { sort = "1" }, new { @class = "txtmtl-results-sortby" }) %>
			</th>
			<th>
				<%= this.Html.ActionLink("Email Address", "List", "Member", new { sort = "2" }, new { @class = "txtmtl-results-sortby" }) %>
			</th>
			<th>
				<%= this.Html.ActionLink("Created", "List", "Member", new { sort = "3" }, new { @class = "txtmtl-results-sortby" }) %>
			</th>
			<th></th>
		</tr>
		<%
			int index = 0;
			if ((object)this.Model.MemberResults != null)
			{
				foreach (var memberResult in this.Model.MemberResults)
				{
		%>
				<tr class="<%= (index % 2) == 0 ? "txtmtl-results-row" : "txtmtl-results-altrow" %>">
					<td>
						<%= this.Html.Encode(memberResult.MemberName.SafeToString()) %>
					</td>
					<td>
						<%= this.Html.Encode(memberResult.UserName.SafeToString()) %>
					</td>
					<td>
						<%= this.Html.Encode(memberResult.EmailAddress.SafeToString()) %>
					</td>
					<td>
						<%= this.Html.Encode(memberResult.CreationTimestamp.SafeToString()) %>
					</td>
					<td>
						<% if (memberResult.MemberId != (int)Current.MemberId)
						   { %>
							<%= this.Html.ActionLink("Remove", "Remove", new
																		{
																			id = memberResult.MemberId
																		})
	%>
						<% } %>

						<%= this.Html.ActionLink("Edit", "Edit", new
																{
																	id = memberResult.MemberId
																})
	%>
					</td>
				</tr>
		<%

			index++;
				}
			} %>
		<tr class="txtmtl-results-footer"></tr>
	</table>

	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Create a new member in this organization", "Create", "Member") %><br />
		<%= this.Html.ActionLink("Return to organization dashboard", "Dashboard", "Organization") %>
	</div>

</asp:Content>
