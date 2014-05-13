<%@ Page Title="Circle List" MasterPageFile="~/Views/Shared/SiteBase.Master" Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<TextMetal.HostImpl.AspNetSample.DomainModel.Tables.Circle>>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MiddleContent" runat="server">
	<table class="txtmtl-results">
		<tr class="txtmtl-results-header">
			<th>
				<%= this.Html.ActionLink("Circle Name", "List", "Circle", new { sort = "0" }, new { @class = "txtmtl-results-sortby" }) %>				
			</th>
			<th>
				<%= this.Html.ActionLink("Circle Desc", "List", "Circle", new { sort = "1" }, new { @class = "txtmtl-results-sortby" }) %>
			</th>			
			<th>
				<%= this.Html.ActionLink("Created", "List", "Circle", new { sort = "2" }, new { @class = "txtmtl-results-sortby" }) %>
			</th>
			<th>
			</th>
		</tr>
		<%
			int index = 0;
			foreach (Circle item in this.Model)
			{
		%>
			<tr class="<%= (index % 2) == 0 ? "txtmtl-results-row" : "txtmtl-results-altrow" %>">
				<td>
					<%= this.Html.Encode(string.Format("{0}", item.CircleName)) %>
				</td>
				<td>
					<%= this.Html.Encode(string.Format("{0}", item.CircleDesc)) %>
				</td>
				<td>
					<%= this.Html.Encode(String.Format("{0:MM/dd/yyyy hh:mm:ss}", item.CreationTimestamp)) %>
				</td>
				<td>
					<%= this.Html.ActionLink("Delete", "Delete", new
																{
																	id = item.CircleId
																}, new { onclick = "javascript: return confirm('Are you sure you want to delete this circle?')" })
	%>
					<%= this.Html.ActionLink("Edit", "Edit", new
															{
																id = item.CircleId
															})
	%>
				</td>
			</tr>
		<%
			index++;
			} %>
		<tr class="txtmtl-results-footer"></tr>
	</table>
	<div class="txtmtl-ctxnav-wrapper">
		<%= this.Html.ActionLink("Create new circle", "Create", "Circle") %><br />
		<%= this.Html.ActionLink("Return to circle dashboard", "Dashboard", "Circle") %>
	</div>
</asp:Content>