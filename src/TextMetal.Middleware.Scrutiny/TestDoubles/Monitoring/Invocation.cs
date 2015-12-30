#region Using

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#endregion

namespace NMock.Monitoring
{
	/// <summary>
	/// Represents the invocation of a method on an object (receiver).
	/// </summary>
	public class Invocation : ISelfDescribing
	{
		#region Constructors/Destructors

		internal Invocation(MethodBase method, object[] arguments)
		{
			this.MethodBase = method;
			this.Arguments = arguments;
			this.Parameters = new ParameterList(method, arguments);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Invocation" /> class.
		/// </summary>
		/// <param name="receiver"> The receiver providing the method. </param>
		/// <param name="method"> The method. </param>
		/// <param name="arguments"> The parameters passed to the method.. </param>
		public Invocation(object receiver, MethodBase method, object[] arguments)
			: this(method, arguments)
		{
			if (receiver == null)
				throw new ArgumentNullException("receiver");

			this.Receiver = receiver;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Holds the exception to be thrown. When this field has been set, <see cref="IsThrowing" /> will become true.
		/// </summary>
		private Exception exception;

		/// <summary>
		/// Holds the result of the invocation.
		/// </summary>
		private object result = Missing.Value;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a value indicating if this <see cref="Invocation" /> is an event
		/// </summary>
		public bool IsEventAccessor
		{
			get
			{
				return this.IsEventAdder || this.IsEventRemover;
			}
		}

		/// <summary>
		/// Determines whether the initialized method is an event adder.
		/// </summary>
		/// <returns>
		/// Returns true if initialized method is an event adder; false otherwise.
		/// </returns>
		public bool IsEventAdder
		{
			get
			{
				return this.Method.IsSpecialName
						&& this.Method.Name.StartsWith(Constants.ADD)
					//&& Parameters.Count == 1
						&& typeof(Delegate).IsAssignableFrom(this.Method.GetParameters()[0].ParameterType);
			}
		}

		/// <summary>
		/// Determines whether the initialized method is an event remover.
		/// </summary>
		/// <returns>
		/// Returns true if initialized method is an event remover; false otherwise.
		/// </returns>
		public bool IsEventRemover
		{
			get
			{
				return this.Method.IsSpecialName
						&& this.Method.Name.StartsWith(Constants.REMOVE)
					//&& Parameters.Count == 1
						&& typeof(Delegate).IsAssignableFrom(this.Method.GetParameters()[0].ParameterType);
			}
		}

		/// <summary>
		/// Gets a value indicating if this <see cref="Invocation" /> is a method.
		/// </summary>
		public bool IsMethod
		{
			get
			{
				return (!this.IsEventAccessor && !this.IsPropertyAccessor);
			}
		}

		/// <summary>
		/// Gets a value indicating if this <see cref="Invocation" /> is a property
		/// </summary>
		public bool IsPropertyAccessor
		{
			get
			{
				return this.IsPropertySetAccessor || this.IsPropertyGetAccessor;
			}
		}

		/// <summary>
		/// Gets a value indicating if this <see cref="Invocation" /> is a property getter
		/// </summary>
		public bool IsPropertyGetAccessor
		{
			get
			{
				return this.Method.IsSpecialName && this.Method.Name.StartsWith(Constants.GET);
			}
		}

		/// <summary>
		/// Gets a value indicating if this <see cref="Invocation" /> is a property setter
		/// </summary>
		public bool IsPropertySetAccessor
		{
			get
			{
				return this.Method.IsSpecialName && this.Method.Name.StartsWith(Constants.SET);
			}
		}

		/// <summary>
		/// Holds the method that is being invoked.
		/// </summary>
		public MethodInfo Method
		{
			get
			{
				return this.MethodBase as MethodInfo;
			}
		}

		/// <summary>
		/// Gets the name of method or property specified by the <see cref="Invocation" />
		/// </summary>
		public string MethodName
		{
			get
			{
				if (this.IsPropertyGetAccessor)
					return this.Method.Name.Substring(Constants.GET.Length);

				if (this.IsPropertySetAccessor)
					return this.Method.Name.Substring(Constants.SET.Length);

				return this.Method.Name;
			}
		}

		/// <summary>
		/// Gets the parameters of the method specified by the <see cref="Invocation" />
		/// </summary>
		public ParameterInfo[] MethodParameters
		{
			get
			{
				return this.Method.GetParameters();
			}
		}

		/// <summary>
		/// Gets the return type of the method specified by the <see cref="Invocation" />
		/// </summary>
		public Type MethodReturnType
		{
			get
			{
				return this.Method.ReturnType;
			}
		}

		/// <summary>
		/// Gets a string that represents the signature of the <see cref="Method" />
		/// </summary>
		public string MethodSignature
		{
			get
			{
				if (this.IsPropertyGetAccessor)
					return this.Method.Name + "(" + this.GetParameterTypes(this.Method) + ")";

				if (this.IsPropertySetAccessor)
					return this.Method.Name + "(" + this.GetParameterTypes(this.Method) + ")";

				return this.Method.Name + this.GetGenericSignature(this.Method) + "(" + this.GetParameterTypes(this.Method) + ")";
			}
		}

		/// <summary>
		/// Gets a string that represents the signature of the property setter
		/// </summary>
		public string MethodSignatureForSetter
		{
			get
			{
				if (!this.IsPropertyGetAccessor)
					throw new InvalidOperationException("This method may only be called from property getter invocations.");

				string parameters = this.GetParameterTypes(this.Method);
				if (parameters.Length != 0)
					parameters += ",";
				parameters += this.Method.ReturnType.FullName;

				return Constants.SET + this.MethodName + "(" + parameters + ")";
			}
		}

		/// <summary>
		/// Returns the Receiver as an <see cref="IMockObject" />
		/// </summary>
		public IMockObject MockObject
		{
			get
			{
				return this.Receiver as IMockObject;
			}
		}

		/// <summary>
		/// Gets the arguments passed into the constructor of this <see cref="Invocation" />
		/// </summary>
		public object[] Arguments
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the exception that is thrown on the invocation.
		/// </summary>
		/// <value> The exception. </value>
		public Exception Exception
		{
			get
			{
				return this.exception;
			}

			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				this.exception = value;
				this.result = null;
				this.IsThrowing = true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether an exception is thrown an this invocation.
		/// </summary>
		/// <value>
		/// <c> true </c> if this invocation is throwing an exception; otherwise, <c> false </c>.
		/// </value>
		public bool IsThrowing
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="MethodBase" /> that was passed into the constructor of this <see cref="Invocation" />
		/// </summary>
		public MethodBase MethodBase
		{
			get;
			private set;
		}

		/// <summary>
		/// Holds the parameterlist of the invocation.
		/// </summary>
		public ParameterList Parameters
		{
			get;
			private set;
		}

		/// <summary>
		/// Holds the receiver providing the method.
		/// </summary>
		public object Receiver
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the result of the invocation.
		/// </summary>
		/// <value> The result. </value>
		public object Result
		{
			get
			{
				return this.result;
			}

			set
			{
				this.ValidateReturnType(value);

				this.result = value;
				this.exception = null;
				this.IsThrowing = false;
			}
		}

		/// <summary>
		/// Stores the value being assigned in a setter.
		/// </summary>
		/// <remarks>
		/// Used internally to store a setter value to return in an automatic getter.
		/// </remarks>
		internal object SetterResult
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Describes the event adder to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"> The writer where the description is written to. </param>
		private void DescribeAsEventAdder(TextWriter writer)
		{
			writer.Write(".");
			writer.Write(this.Method.Name.Substring(4));
			writer.Write(" += ");

			var @delegate = (MulticastDelegate)this.Parameters[0];
			writer.Write("<" + @delegate.GetMethodInfo().Name + "[" + @delegate + "]>");
		}

		/// <summary>
		/// Describes the event remover to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"> The writer where the description is written to. </param>
		private void DescribeAsEventRemover(TextWriter writer)
		{
			writer.Write(".");
			writer.Write(this.Method.Name.Substring(7));
			writer.Write(" -= ");

			var @delegate = (MulticastDelegate)this.Parameters[0];
			writer.Write("<" + @delegate.GetMethodInfo().Name + "[" + @delegate + "]>");
		}

		/// <summary>
		/// Describes the index setter with parameters to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"> The writer where the description is written to. </param>
		private void DescribeAsIndexerGetter(TextWriter writer)
		{
			writer.Write("[");
			this.WriteParameterList(writer, this.Parameters.Count);
			writer.Write("]");
		}

		/// <summary>
		/// Describes the index setter with parameters to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"> The writer where the description is written to. </param>
		private void DescribeAsIndexerSetter(TextWriter writer)
		{
			writer.Write("[");
			this.WriteParameterList(writer, this.Parameters.Count - 1);
			writer.Write("] = ");
			writer.Write(this.Parameters[this.Parameters.Count - 1]);
		}

		/// <summary>
		/// Describes the property with parameters to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"> The writer where the description is written to. </param>
		private void DescribeAsProperty(TextWriter writer)
		{
			writer.Write(".");
			writer.Write(this.Method.Name.Substring(4));
			if (this.Parameters.Count > 0)
			{
				writer.Write(" = ");
				writer.Write(this.Parameters[0]);
			}
		}

		/// <summary>
		/// Describes the method with parameters to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"> The writer where the description is written to. </param>
		private void DescribeNormalMethod(TextWriter writer)
		{
			writer.Write(".");
			writer.Write(this.Method.Name);

			this.WriteTypeParams(writer);

			writer.Write("(");
			this.WriteParameterList(writer, this.Parameters.Count);
			writer.Write(")");
		}

		/// <summary>
		/// Describes this object to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"> The text writer the description is added to. </param>
		void ISelfDescribing.DescribeTo(TextWriter writer)
		{
			// This should really be a mock object in most cases, but a few testcases
			// seem to supply strings etc as a Receiver.
			var mock = this.Receiver as IMockObject;
			var mockName = (mock != null) ? mock.MockName : this.Receiver.ToString();

			if (this.IsIndexerGetter())
			{
				writer.Write(mockName);
				this.DescribeAsIndexerGetter(writer);
			}
			else if (this.IsIndexerSetter())
			{
				writer.Write(mockName);
				this.DescribeAsIndexerSetter(writer);
			}
			else if (this.IsEventAdder)
			{
				writer.Write(mockName);
				this.DescribeAsEventAdder(writer);
			}
			else if (this.IsEventRemover)
			{
				writer.Write(mockName);
				this.DescribeAsEventRemover(writer);
			}
			else if (this.IsProperty())
			{
				writer.Write(mockName);
				this.DescribeAsProperty(writer);
			}
			else
			{
				writer.Write(this.Method.ReturnType.FullName);
				writer.Write(" ");
				writer.Write(mockName);
				this.DescribeNormalMethod(writer);
			}
			writer.WriteLine();
		}

		private string DescribeType(object obj)
		{
			var type = obj.GetType();
			var sb = new StringBuilder();
			sb.Append(type);

			Type[] interfaceTypes = type.GetInterfaces();
			if (interfaceTypes.Length > 0)
			{
				sb.Append(": ");

				foreach (Type interfaceType in interfaceTypes)
				{
					sb.Append(interfaceType);
					sb.Append(", ");
				}

				sb.Length -= 2; // cut away last ", "
			}

			return sb.ToString();
		}

		private string GetGenericSignature(MethodInfo methodInfo)
		{
			if (methodInfo.GetGenericArguments().Any())
				return "<" + this.GetGenericTypes(methodInfo) + ">";
			return string.Empty;
		}

		private string GetGenericTypes(MethodInfo method)
		{
			return string.Join(",", method.GetGenericArguments().Select(_ => _.FullName).ToArray());
		}

		private string GetParameterTypes(MethodInfo method)
		{
			return string.Join(",", method.GetParameters().Select(_ => _.ParameterType.FullName).ToArray());
		}

		/// <summary>
		/// Invokes this invocation on the specified receiver and stores the result and exception
		/// returns/thrown by the invocation.
		/// </summary>
		/// <param name="receiver"> The receiver. </param>
		public void InvokeOn(object receiver)
		{
			try
			{
				this.Result = this.Method.Invoke(receiver, this.Parameters.AsArray);
				this.Parameters.MarkAllValuesAsSet();
			}
			catch (TargetInvocationException e)
			{
				this.Exception = e.InnerException;
			}
		}

		/// <summary>
		/// Determines whether the initialized method is an index getter.
		/// </summary>
		/// <returns>
		/// Returns true if initialized method is an index getter; false otherwise.
		/// </returns>
		internal bool IsIndexerGetter()
		{
			return this.Method.IsSpecialName
					&& this.Method.Name == Constants.GET_ITEM
					&& this.Parameters.Count >= 1;
		}

		/// <summary>
		/// Determines whether the initialized method is an index setter.
		/// </summary>
		/// <returns>
		/// Returns true if initialized method is an index setter; false otherwise.
		/// </returns>
		internal bool IsIndexerSetter()
		{
			return this.Method.IsSpecialName
					&& this.Method.Name == Constants.SET_ITEM
					&& this.Parameters.Count >= 2;
		}

		/// <summary>
		/// Determines whether the initialized method is a property.
		/// </summary>
		/// <returns>
		/// Returns true if initialized method is a property; false otherwise.
		/// </returns>
		private bool IsProperty()
		{
			return this.Method.IsSpecialName &&
					((this.Method.Name.StartsWith(Constants.GET) && this.Parameters.Count == 0) ||
					(this.Method.Name.StartsWith(Constants.SET) && this.Parameters.Count == 1));
		}

		public override string ToString()
		{
			return "[Invocation: " + this.Receiver + this.Method + "]";
		}

		/// <summary>
		/// Checks the returnType of the initialized method if it is valid to be mocked.
		/// </summary>
		/// <param name="value"> The return value to be checked. </param>
		private void ValidateReturnType(object value)
		{
			if (this.Method.ReturnType == typeof(void) && value != null)
				throw new ArgumentException("cannot return a value from a void method", "value");

			var _type = this.Method.ReturnType.GetTypeInfo();

			if (this.Method.ReturnType != typeof(void) && _type.IsValueType && value == null
				&& (!(_type.IsGenericType && this.Method.ReturnType.GetGenericTypeDefinition() == typeof(Nullable<>)))
				)
				throw new ArgumentException("cannot return a null value type", "value");

			if (value != null && !this.Method.ReturnType.IsInstanceOfType(value))
			{
				throw new ArgumentException(
					"cannot return a value of type " + this.DescribeType(value) + " from a method returning " + this.Method.ReturnType,
					"value");
			}
		}

		/// <summary>
		/// Writes the parameter list to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"> The writer where the description is written to. </param>
		/// <param name="count"> The count of parameters to describe. </param>
		private void WriteParameterList(TextWriter writer, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
					writer.Write(", ");

				if (this.Method.GetParameters()[i].IsOut)
					writer.Write("out");
				else
					writer.Write(this.Parameters[i]);
			}
		}

		/// <summary>
		/// Writes the generic parameters of the method to the specified <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"> The writer where the description is written to. </param>
		private void WriteTypeParams(TextWriter writer)
		{
			Type[] types = this.Method.GetGenericArguments();
			if (types.Length > 0)
			{
				writer.Write("<");

				for (int i = 0; i < types.Length; i++)
				{
					if (i > 0)
						writer.Write(", ");

					writer.Write(types[i].FullName);
				}

				writer.Write(">");
			}
		}

		#endregion
	}
}