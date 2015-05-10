/*
	Copyright ©2002-2015 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.Framework.InputOutput;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;

namespace TextMetal.Framework.Core
{
	public sealed class TemplatingContext : ITemplatingContext
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the TemplatingContext class.
		/// </summary>
		/// <param name="xpe"> The XML persist engine in-effect. </param>
		/// <param name="tokenizer"> The tokenizer in-efect. </param>
		/// <param name="input"> The input mechanism in-effect. </param>
		/// <param name="output"> The output mechanism in-effect. </param>
		/// <param name="properties"> The properties in-effect. </param>
		public TemplatingContext(IXmlPersistEngine xpe, Tokenizer tokenizer, IInputMechanism input, IOutputMechanism output, IDictionary<string, IList<string>> properties)
		{
			if ((object)xpe == null)
				throw new ArgumentNullException("xpe");

			if ((object)tokenizer == null)
				throw new ArgumentNullException("tokenizer");

			if ((object)input == null)
				throw new ArgumentNullException("input");

			if ((object)output == null)
				throw new ArgumentNullException("output");

			if ((object)properties == null)
				throw new ArgumentNullException("properties");

			this.xpe = xpe;
			this.tokenizer = tokenizer;
			this.input = input;
			this.output = output;
			this.properties = properties;
		}

		#endregion

		#region Fields/Constants

		private readonly IInputMechanism input;
		private readonly Stack<object> iteratorModels = new Stack<object>();
		private readonly IOutputMechanism output;
		private readonly IDictionary<string, IList<string>> properties;
		private readonly Tokenizer tokenizer;
		private readonly Stack<Dictionary<string, object>> variableTables = new Stack<Dictionary<string, object>>();
		private readonly IXmlPersistEngine xpe;
		private bool disposed;

		#endregion

		#region Properties/Indexers/Events

		public IDictionary<string, object> CurrentVariableTable
		{
			get
			{
				return this.VariableTables.Count > 0 ? this.VariableTables.Peek() : null;
			}
		}

		public IInputMechanism Input
		{
			get
			{
				return this.input;
			}
		}

		public Stack<object> IteratorModels
		{
			get
			{
				return this.iteratorModels;
			}
		}

		public IOutputMechanism Output
		{
			get
			{
				return this.output;
			}
		}

		public IDictionary<string, IList<string>> Properties
		{
			get
			{
				return this.properties;
			}
		}

		public Tokenizer Tokenizer
		{
			get
			{
				return this.tokenizer;
			}
		}

		public Stack<Dictionary<string, object>> VariableTables
		{
			get
			{
				return this.variableTables;
			}
		}

		private IXmlPersistEngine Xpe
		{
			get
			{
				return this.xpe;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current instance has been disposed.
		/// </summary>
		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
			private set
			{
				this.disposed = value;
			}
		}

		#endregion

		#region Methods/Operators

		public void AddReference(Type xmlObjectType)
		{
			if ((object)xmlObjectType == null)
				throw new ArgumentNullException("xmlObjectType");

			this.Xpe.RegisterKnownXmlObject(xmlObjectType);
		}

		public void AddReference(XmlName xmlName, Type xmlObjectType)
		{
			if ((object)xmlName == null)
				throw new ArgumentNullException("xmlName");

			if ((object)xmlObjectType == null)
				throw new ArgumentNullException("xmlObjectType");

			this.Xpe.RegisterKnownXmlObject(xmlName, xmlObjectType);
		}

		public void ClearReferences()
		{
			this.Xpe.ClearAllKnowns();
		}

		/// <summary>
		/// Dispose of the unit of work.
		/// </summary>
		public void Dispose()
		{
			if (this.Disposed)
				return;

			try
			{
			}
			finally
			{
				this.Disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		public DynamicWildcardTokenReplacementStrategy GetDynamicWildcardTokenReplacementStrategy()
		{
			return this.GetDynamicWildcardTokenReplacementStrategy(this.Tokenizer.StrictMatching);
		}

		public DynamicWildcardTokenReplacementStrategy GetDynamicWildcardTokenReplacementStrategy(bool strict)
		{
			List<object> temp;
			List<Dictionary<string, object>> temp2;

			// need to review how top level objects are accessed?
			temp = new List<object>(this.IteratorModels);
			temp2 = new List<Dictionary<string, object>>(this.VariableTables);

			temp.InsertRange(0, temp2.ToArray());

			return new DynamicWildcardTokenReplacementStrategy(temp.ToArray(), strict);
		}

		public bool LaunchDebugger()
		{
			bool result;

			result = System.Diagnostics.Debugger.Launch() && System.Diagnostics.Debugger.IsAttached;
			Console.WriteLine("Debugger launch result: '{0}'", result);
			return result;
		}

		public void SetReference(Type xmlObjectType)
		{
			this.Xpe.RegisterKnownXmlTextObject(xmlObjectType);
		}

		#endregion
	}
}