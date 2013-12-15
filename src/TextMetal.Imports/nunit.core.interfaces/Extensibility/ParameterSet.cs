// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

using NUnit.Framework;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// ParameterSet encapsulates method arguments and
	/// other selected parameters needed for constructing
	/// a parameterized test case.
	/// </summary>
	public class ParameterSet : ITestCaseData
	{
		//private static readonly string IGNOREREASON = "_IGNOREREASON";

		#region Constructors/Destructors

		/// <summary>
		/// Construct a non-runnable ParameterSet, specifying
		/// the provider excetpion that made it invalid.
		/// </summary>
		public ParameterSet(Exception exception)
		{
			this.runState = RunState.NotRunnable;
			this.providerException = exception;
			this.ignoreReason = exception.Message;
		}

		/// <summary>
		/// Construct an empty parameter set, which
		/// defaults to being Runnable.
		/// </summary>
		public ParameterSet()
		{
			this.runState = RunState.Runnable;
		}

		#endregion

		#region Fields/Constants

		private static readonly string CATEGORIES = "_CATEGORIES";
		private static readonly string DESCRIPTION = "_DESCRIPTION";

		private object[] arguments;
		private string expectedExceptionName;
		private Type expectedExceptionType;
		private string expectedMessage;
		private object expectedResult;
		private bool hasExpectedResult;
		private string ignoreReason;
		private bool isExplicit;
		private bool isIgnored;
		private string matchType;
		private object[] originalArguments;

		/// <summary>
		/// A dictionary of properties, used to add information
		/// to tests without requiring the class to change.
		/// </summary>
		private IDictionary properties;

		private Exception providerException;
		private RunState runState;
		private string testName;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The arguments to be used in running the test,
		/// which must match the method signature.
		/// </summary>
		public object[] Arguments
		{
			get
			{
				return this.arguments;
			}
			set
			{
				this.arguments = value;

				if (this.originalArguments == null)
					this.originalArguments = value;
			}
		}

		/// <summary>
		/// Gets a list of categories associated with this test.
		/// </summary>
		public IList Categories
		{
			get
			{
				if (this.Properties[CATEGORIES] == null)
					this.Properties[CATEGORIES] = new ArrayList();

				return (IList)this.Properties[CATEGORIES];
			}
		}

		/// <summary>
		/// A description to be applied to this test case
		/// </summary>
		public string Description
		{
			get
			{
				return (string)this.Properties[DESCRIPTION];
			}
			set
			{
				if (value != null)
					this.Properties[DESCRIPTION] = value;
				else
					this.Properties.Remove(DESCRIPTION);
			}
		}

		/// <summary>
		/// The Type of any exception that is expected.
		/// </summary>
		public Type ExpectedException
		{
			get
			{
				return this.expectedExceptionType;
			}
			set
			{
				this.expectedExceptionType = value;
			}
		}

		/// <summary>
		/// The FullName of any exception that is expected
		/// </summary>
		public string ExpectedExceptionName
		{
			get
			{
				return this.expectedExceptionName;
			}
			set
			{
				this.expectedExceptionName = value;
			}
		}

		/// <summary>
		/// The Message of any exception that is expected
		/// </summary>
		public string ExpectedMessage
		{
			get
			{
				return this.expectedMessage;
			}
			set
			{
				this.expectedMessage = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ParameterSet" /> is explicit.
		/// </summary>
		/// <value> <c> true </c> if explicit; otherwise, <c> false </c> . </value>
		public bool Explicit
		{
			get
			{
				return this.isExplicit;
			}
			set
			{
				this.isExplicit = value;
			}
		}

		/// <summary>
		/// Returns true if an expected result has been
		/// specified for this parameter set.
		/// </summary>
		public bool HasExpectedResult
		{
			get
			{
				return this.hasExpectedResult;
			}
		}

		/// <summary>
		/// Gets or sets the ignore reason.
		/// </summary>
		/// <value> The ignore reason. </value>
		public string IgnoreReason
		{
			get
			{
				return this.ignoreReason;
			}
			set
			{
				this.ignoreReason = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ParameterSet" /> is ignored.
		/// </summary>
		/// <value> <c> true </c> if ignored; otherwise, <c> false </c> . </value>
		public bool Ignored
		{
			get
			{
				return this.isIgnored;
			}
			set
			{
				this.isIgnored = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of match to be performed on the expected message
		/// </summary>
		public string MatchType
		{
			get
			{
				return this.matchType;
			}
			set
			{
				this.matchType = value;
			}
		}

		/// <summary>
		/// The original arguments supplied by the user,
		/// used for display purposes.
		/// </summary>
		public object[] OriginalArguments
		{
			get
			{
				return this.originalArguments;
			}
		}

		/// <summary>
		/// Gets the property dictionary for this test
		/// </summary>
		public IDictionary Properties
		{
			get
			{
				if (this.properties == null)
					this.properties = new ListDictionary();

				return this.properties;
			}
		}

		/// <summary>
		/// Holds any exception thrown by the parameter provider
		/// </summary>
		public Exception ProviderException
		{
			get
			{
				return this.providerException;
			}
		}

		/// <summary>
		/// The expected result of the test, which
		/// must match the method return type.
		/// </summary>
		public object Result
		{
			get
			{
				return this.expectedResult;
			}
			set
			{
				this.expectedResult = value;
				this.hasExpectedResult = true;
			}
		}

		/// <summary>
		/// The RunState for this set of parameters.
		/// </summary>
		public RunState RunState
		{
			get
			{
				return this.runState;
			}
			set
			{
				this.runState = value;
			}
		}

		/// <summary>
		/// A name to be used for this test case in lieu
		/// of the standard generated name containing
		/// the argument list.
		/// </summary>
		public string TestName
		{
			get
			{
				return this.testName;
			}
			set
			{
				this.testName = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Constructs a ParameterSet from another object, accessing properties
		/// by reflection. The object must expose at least an Arguments property
		/// in order for the test to be runnable.
		/// </summary>
		/// <param name="source"> </param>
		public static ParameterSet FromDataSource(object source)
		{
			ParameterSet parms = new ParameterSet();

			parms.Arguments = GetParm(source, PropertyNames.Arguments) as object[];

			parms.ExpectedException = GetParm(source, PropertyNames.ExpectedException) as Type;
			if (parms.ExpectedException != null)
				parms.ExpectedExceptionName = parms.ExpectedException.FullName;
			else
				parms.ExpectedExceptionName = GetParm(source, PropertyNames.ExpectedExceptionName) as string;

			parms.ExpectedMessage = GetParm(source, PropertyNames.ExpectedMessage) as string;
			object matchEnum = GetParm(source, PropertyNames.MatchType);
			if (matchEnum != null)
				parms.MatchType = matchEnum.ToString();

			// Note: pre-2.6 versions of some attributes don't have the HasExpectedResult property
			object hasResult = GetParm(source, PropertyNames.HasExpectedResult);
			object expectedResult = GetParm(source, PropertyNames.ExpectedResult);
			if (hasResult != null && (bool)hasResult || expectedResult != null)
				parms.Result = expectedResult;

			parms.Description = GetParm(source, PropertyNames.Description) as string;

			parms.TestName = GetParm(source, PropertyNames.TestName) as string;

			object objIgnore = GetParm(source, PropertyNames.Ignored);
			if (objIgnore != null)
				parms.Ignored = (bool)objIgnore;

			parms.IgnoreReason = GetParm(source, PropertyNames.IgnoreReason) as string;

			object objExplicit = GetParm(source, PropertyNames.Explicit);
			if (objExplicit != null)
				parms.Explicit = (bool)objExplicit;

			// Some sources may also implement Properties and/or Categories
			bool gotCategories = false;
			IDictionary props = GetParm(source, PropertyNames.Properties) as IDictionary;
			if (props != null)
			{
				foreach (string key in props.Keys)
				{
					parms.Properties.Add(key, props[key]);
					if (key == CATEGORIES)
						gotCategories = true;
				}
			}

			// Some sources implement Categories. They may have been
			// provided as properties or they may be separate.
			if (!gotCategories)
			{
				IList categories = GetParm(source, PropertyNames.Categories) as IList;
				if (categories != null)
				{
					foreach (string cat in categories)
						parms.Categories.Add(cat);
				}
			}

			return parms;
		}

		private static object GetParm(object source, string name)
		{
			Type type = source.GetType();
			PropertyInfo prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
			if (prop != null)
				return prop.GetValue(source, null);

			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
			if (field != null)
				return field.GetValue(source);

			return null;
		}

		#endregion
	}
}