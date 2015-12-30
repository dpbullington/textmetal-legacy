#region Using

using System;
using System.Collections;
using System.Reflection;

#endregion

namespace NMock.Monitoring
{
	/// <summary>
	/// Manages a list of parameters for a mocked method together with the parameter's values.
	/// </summary>
	public class ParameterList
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterList" /> class.
		/// </summary>
		/// <param name="method"> The method to be mocked. </param>
		/// <param name="values"> The values of the parameters. </param>
		public ParameterList(MethodBase method, object[] values)
		{
			if (method == null)
				throw new ArgumentNullException("method");
			if (values == null)
				throw new ArgumentNullException("values");

			this.method = method;
			this.values = values;
			this.isValueSet = new BitArray(values.Length);

			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
				this.isValueSet[i] = !parameters[i].IsOut;
		}

		#endregion

		#region Fields/Constants

		/// <summary>
		/// Holds a boolean for each value if it was set or not.
		/// </summary>
		private readonly BitArray isValueSet;

		/// <summary>
		/// Holds the method to be mocked.
		/// </summary>
		private readonly MethodBase method;

		/// <summary>
		/// An array holding the values of the parameters.
		/// </summary>
		private readonly object[] values;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Gets or sets the <see cref="System.Object" /> with the specified index.
		/// </summary>
		/// <param name="index"> The index of the value to be get or set. </param>
		/// <value>
		/// The value of a parameter specified by its <paramref name="index" />.
		/// </value>
		public object this[int index]
		{
			get
			{
				if (this.IsValueSet(index))
					return this.values[index];

				throw new InvalidOperationException(string.Format("Parameter '{0}' has not been set.", this.GetParameterName(index)));
			}

			set
			{
				if (this.CanValueBeSet(index))
				{
					this.values[index] = value;
					this.isValueSet[index] = true;
				}
				else
					throw new InvalidOperationException(string.Format("Cannot set the value of in parameter '{0}'", this.GetParameterName(index)));
			}
		}

		/// <summary>
		/// Gets the values as array.
		/// </summary>
		/// <value> Values as array. </value>
		internal object[] AsArray
		{
			get
			{
				return this.values;
			}
		}

		/// <summary>
		/// Gets the number of values.
		/// </summary>
		/// <value> The number of values. </value>
		public int Count
		{
			get
			{
				return this.values.Length;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Determines whether the parameter specified by index can be set.
		/// </summary>
		/// <param name="index"> The index of the parameter. </param>
		/// <returns>
		/// Returns <c> true </c> if the parameter specified by index can be set; otherwise, <c> false </c>.
		/// </returns>
		private bool CanValueBeSet(int index)
		{
			ParameterAttributes attributes = this.method.GetParameters()[index].Attributes;
			return (attributes & ParameterAttributes.In) == ParameterAttributes.None;
		}

		/// <summary>
		/// Gets the parameter name by index.
		/// </summary>
		/// <param name="index"> The index of the parameter name to get. </param>
		/// <returns>
		/// Returns the parameter name with the given index.
		/// </returns>
		private string GetParameterName(int index)
		{
			return this.method.GetParameters()[index].Name;
		}

		/// <summary>
		/// Determines whether the value specified by index was set.
		/// </summary>
		/// <param name="index"> The index. </param>
		/// <returns>
		/// Returns <c> true </c> if value specified by index was set; otherwise, <c> false </c>.
		/// </returns>
		public bool IsValueSet(int index)
		{
			return this.isValueSet[index];
		}

		/// <summary>
		/// Marks all values as set.
		/// </summary>
		internal void MarkAllValuesAsSet()
		{
			this.isValueSet.SetAll(true);
		}

		#endregion
	}
}