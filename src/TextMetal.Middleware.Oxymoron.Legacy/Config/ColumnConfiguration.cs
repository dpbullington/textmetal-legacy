/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json;

using TextMetal.Middleware.Oxymoron.Legacy.Strategy;
using TextMetal.Middleware.Solder.Extensions;
using TextMetal.Middleware.Solder.Primitives;

namespace TextMetal.Middleware.Oxymoron.Legacy.Config
{
	public class ColumnConfiguration : ConfigurationObject
	{
		#region Constructors/Destructors

		public ColumnConfiguration()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly Dictionary<string, object> obfuscationStrategySpecificConfiguration = new Dictionary<string, object>();
		private string columnName;
		private string obfuscationStrategyAqtn;

		#endregion

		#region Properties/Indexers/Events

		public Dictionary<string, object> ObfuscationStrategySpecificConfiguration
		{
			get
			{
				return this.obfuscationStrategySpecificConfiguration;
			}
		}

		public string ColumnName
		{
			get
			{
				return this.columnName;
			}
			set
			{
				this.columnName = value;
			}
		}

		public string ObfuscationStrategyAqtn
		{
			get
			{
				return this.obfuscationStrategyAqtn;
			}
			set
			{
				this.obfuscationStrategyAqtn = value;
			}
		}

		[JsonIgnore]
		public new TableConfiguration Parent
		{
			get
			{
				return (TableConfiguration)base.Parent;
			}
			set
			{
				base.Parent = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override bool Equals(object obj)
		{
			ColumnConfiguration other;

			other = obj as ColumnConfiguration;

			if ((object)other != null)
				return other.ColumnName.SafeToString().ToLower() == this.ColumnName.SafeToString().ToLower();

			return false;
		}

		public override int GetHashCode()
		{
			return this.ColumnName.SafeToString().ToLower().GetHashCode();
		}

		public IObfuscationStrategy GetObfuscationStrategyInstance()
		{
			IObfuscationStrategy instance;
			Type type;

			type = this.GetObfuscationStrategyType();

			if ((object)type == null)
				return null;

			instance = (IObfuscationStrategy)Activator.CreateInstance(type);

			return instance;
		}

		public Type GetObfuscationStrategyType()
		{
			Type type;

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.ObfuscationStrategyAqtn))
				return null;

			type = Type.GetType(this.ObfuscationStrategyAqtn, false);

			return type;
		}

		public virtual void ResetObfuscationStrategySpecificConfiguration()
		{
			this.ObfuscationStrategySpecificConfiguration.Clear();
		}

		public sealed override IEnumerable<Message> Validate()
		{
			return this.Validate(null);
		}

		public virtual IEnumerable<Message> Validate(int? columnIndex)
		{
			List<Message> messages;
			Type type;
			IObfuscationStrategy obfuscationStrategy;

			messages = new List<Message>();

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.ColumnName))
				messages.Add(NewError(string.Format("Column[{0}] name is required.", columnIndex)));

			if (SolderLegacyInstanceAccessor.DataTypeFascadeLegacyInstance.IsNullOrWhiteSpace(this.ObfuscationStrategyAqtn))
				messages.Add(NewError(string.Format("Column[{0}/{1}] obfuscation strategy AQTN is required.", columnIndex, this.ColumnName)));
			else
			{
				type = this.GetObfuscationStrategyType();

				if ((object)type == null)
					messages.Add(NewError(string.Format("Column[{0}/{1}] obfuscation strategy failed to load type from AQTN.", columnIndex, this.ColumnName)));
				else if (typeof(IObfuscationStrategy).GetTypeInfo().IsAssignableFrom(type))
				{
					obfuscationStrategy = this.GetObfuscationStrategyInstance();

					if ((object)obfuscationStrategy == null)
						messages.Add(NewError(string.Format("Column[{0}/{1}] obfuscation strategy failed to instatiate type from AQTN.", columnIndex, this.ColumnName)));
					else
						messages.AddRange(obfuscationStrategy.ValidateObfuscationStrategySpecificConfiguration(this, columnIndex));
				}
				else
					messages.Add(NewError(string.Format("Column[{0}/{1}] obfuscation strategy loaded an unrecognized type via AQTN.", columnIndex, this.ColumnName)));
			}

			return messages;
		}

		#endregion
	}
}