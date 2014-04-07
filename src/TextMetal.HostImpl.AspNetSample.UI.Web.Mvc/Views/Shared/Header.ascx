<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TextMetal.HostImpl.AspNetSample.UI.Web.Mvc.Models.TextMetalViewModel>" %>
<div class="txtmtl-header">
	<div>
		<div>
			<a href="<%= this.Url.Content("~") %>">
				<img class="txtmtl-header-logo" src="<%= this.Url.Content("~/images/textmetal-logo.png") %>" alt="TextMetal logo" />
			</a>
		</div>
		<div class="txtmtl-header-family-slogan"><span>A zero friction open source text templating engine for .NET.</span></div>
	</div>
</div>