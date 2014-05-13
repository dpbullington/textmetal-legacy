
function FilterJsonData(data)
{
	var msg;

	if (data == "")
	{
		return null;
	}

	if (typeof(JSON) !== 'undefined' &&
		typeof(JSON.parse) === 'function')
	{
		msg = JSON.parse(data);
	}
	else
	{
		msg = eval('(' + data + ')');
	}

	if (msg.hasOwnProperty('d'))
	{
		return msg.d;
	}
	else
	{
		return msg;
	}
}

function ExecuteSynchJsonAjax(url, parameter)
{
	var result;

	result = $.ajax(
		{
			async: false,

			type: "GET",

			url: url,

			dataType: "json",

			data: parameter
		}
	);

	return FilterJsonData(result.responseText);
}

function ExecuteSynchPostXml(url, postData)
{
	var result;

	result = $.ajax(
		{
			async: false,

			type: "POST",

			url: url,

			dataType: "xml",

			processData: false,

			contentType: "text/xml",

			data: postData
		}
	);

	return result.responseText;
}

function SetupProductUnitComboBox()
{
	(function($)
	{
		$.widget("ui.combobox", {
			_create: function()
			{
				var self = this,
					select = this.element.hide(),
					selected = select.children(":selected"),
					value = selected.val() ? selected.text() : "";
				var input = this.input = $("<input>")
					.insertAfter(select)
					.val(value)
					.autocomplete({
						delay: 0,
						minLength: 0,
						source: function(request, response)
						{
							var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
							response(select.children("option").map(function() {
								var text = $(this).text();
								if (this.value && (!request.term || matcher.test(text)))
								{
									return {
										label: text.replace(
											new RegExp(
												"(?![^&;]+;)(?!<[^<>]*)(" +
												$.ui.autocomplete.escapeRegex(request.term) +
												")(?![^<>]*>)(?![^&;]+;)", "gi"
											), "<strong>$1</strong>"),
										value: text,
										option: this
									};
								}
							}));
						},

						select: function(event, ui)
						{
							ui.item.option.selected = true;
							self._trigger("selected", event, {
								item: ui.item.option
							});

							// EDIT:daniel / UPDATE HIDDEN FIELDS
							$("#ProductUnitId").val(select.val());
							$("#ProductUnitName").val(ui.item.value);
						},

						change: function(event, ui)
						{
							if (!ui.item)
							{
								var matcher = new RegExp("^" + $.ui.autocomplete.escapeRegex($(this).val()) + "$", "i"),
									valid = false;
								select.children("option").each(function() {
									if ($(this).text().match(matcher))
									{
										this.selected = valid = true;
										return false;
									}
								});
								if (!valid)
								{
									// EDIT:daniel / DO NOT REMOVE BAD VALUE FROM TEXTBOX
									//remove invalid value, as it didn't match anything
									//$(this).val("");

									select.val("");
									input.data("autocomplete").term = "";
									//return false;
								}

								// EDIT:daniel / UPDATE HIDDEN FIELDS
								$("#ProductUnitId").val(select.val());
								$("#ProductUnitName").val($(this).val());

								if (!valid)
								{
									return false;
								}
							}
						}
					})
					.addClass("ui-widget ui-widget-content ui-corner-left");

				input.data("autocomplete")._renderItem = function(ul, item)
				{
					return $("<li></li>")
						.data("item.autocomplete", item)
						.append("<a>" + item.label + "</a>")
						.appendTo(ul);
				};

				this.button = $("<button type='button'>&nbsp;</button>")
					.attr("tabIndex", -1)
					.attr("title", "Show All Items")
					.insertAfter(input)
					.button({
						icons: {
							primary: "ui-icon-triangle-1-s"
						},
						text: false
					})
					.removeClass("ui-corner-all")
					.addClass("ui-corner-right ui-button-icon")
					.click(function() {
						// close if already visible
						if (input.autocomplete("widget").is(":visible"))
						{
							input.autocomplete("close");
							return;
						}

						// pass empty string as value to search for, displaying all results
						input.autocomplete("search", "");
						input.focus();
					});
			},

			destroy: function()
			{
				this.input.remove();
				this.button.remove();
				this.element.show();
				$.Widget.prototype.destroy.call(this);
			}
		});
	})(jQuery);

	$(document).ready(function() {
		$("#ProductUnits").combobox();
	});
}