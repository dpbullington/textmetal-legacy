<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<TextMetal.Common.Core.Message>>" %>
<%@ Import Namespace="TextMetal.Common.Core" %>

<% if ((object)this.Model != null)
   { %>

	<script type="text/javascript">
		$(document).ready(function() {
			$("#MessageList").dialog({ modal: true, title: 'Message List' });
		});
	</script>

	<div id="MessageList" class="txtmtl-message-list">
		<ul>
			<% foreach (Message message in this.Model)
			   { %>
				<li>
					<%= message.Description %></li>
			<% } %>
		</ul>
	</div>
<% } %>
