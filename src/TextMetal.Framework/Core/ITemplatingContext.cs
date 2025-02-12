﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Framework.InputOutput;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Core
{
	public interface ITemplatingContext : IDisposable
	{
		#region Properties/Indexers/Events

		IDictionary<string, object> CurrentVariableTable
		{
			get;
		}

		IInputMechanism Input
		{
			get;
		}

		Stack<object> IteratorModels
		{
			get;
		}

		IOutputMechanism Output
		{
			get;
		}

		IDictionary<string, IList<string>> Properties
		{
			get;
		}

		Tokenizer Tokenizer
		{
			get;
		}

		Stack<Dictionary<string, object>> VariableTables
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void AddReference(Type xmlObjectType);

		void AddReference(XmlName xmlName, Type xmlObjectType);

		void ClearReferences();

		DynamicWildcardTokenReplacementStrategy GetDynamicWildcardTokenReplacementStrategy();

		DynamicWildcardTokenReplacementStrategy GetDynamicWildcardTokenReplacementStrategy(bool strict);

		bool LaunchDebugger();

		void SetReference(Type xmlObjectType);

		#endregion
	}
}