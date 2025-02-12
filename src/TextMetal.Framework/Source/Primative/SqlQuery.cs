﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Framework.Source.Primative
{
	/// <summary>
	/// Represents an ordered, keyed set of N-level nested and corrolated SQL queries.
	/// </summary>
	[XmlRoot(ElementName = "SqlQuery", Namespace = "http://www.textmetal.com/api/v6.0.0")]
	public sealed class SqlQuery
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SqlQuery class.
		/// </summary>
		public SqlQuery()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly List<SqlQuery> subQueries = new List<SqlQuery>();
		private string key;
		private int? order;
		private string text;
		private CommandType type = CommandType.Text;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a list of corrolated subqueries.
		/// </summary>
		[XmlArray(ElementName = "SubQueries", Order = 1)]
		[XmlArrayItem(ElementName = "SqlQuery")]
		public List<SqlQuery> SubQueries
		{
			get
			{
				return this.subQueries;
			}
		}

		/// <summary>
		/// Gets or sets the ordinal (order) of this SQL query. Do not use directly; rather use the Order property.
		/// </summary>
		[XmlAttribute("order")]
		public string _Order
		{
			get
			{
				return this.Order.SafeToString();
			}
			set
			{
				int ivalue;

				if (SolderFascadeAccessor.DataTypeFascade.IsNullOrWhiteSpace(value))
					this.Order = null;
				else
				{
					if (!SolderFascadeAccessor.DataTypeFascade.TryParse<int>(value, out ivalue))
						this.Order = null;
					else
						this.Order = ivalue;
				}
			}
		}

		/// <summary>
		/// Gets or sets the key of this SQL query.
		/// </summary>
		[XmlAttribute("key")]
		public string Key
		{
			get
			{
				return (this.key ?? string.Empty).Trim();
			}
			set
			{
				this.key = (value ?? string.Empty).Trim();
			}
		}

		/// <summary>
		/// Gets or sets the ordinal (order) of this SQL query.
		/// </summary>
		[XmlIgnore]
		public int? Order
		{
			get
			{
				return this.order;
			}
			set
			{
				this.order = value;
			}
		}

		/// <summary>
		/// Gets or sets the SQL command text for this SQL query.
		/// </summary>
		[XmlElement("Text", Order = 0)]
		public string Text
		{
			get
			{
				return (this.text ?? string.Empty).Trim();
			}
			set
			{
				this.text = (value ?? string.Empty).Trim();
			}
		}

		/// <summary>
		/// Gets or set the command type of this SQL query.
		/// </summary>
		[XmlAttribute("type")]
		public CommandType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		#endregion
	}
}