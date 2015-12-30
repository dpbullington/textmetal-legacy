using System;

using NMock.Internal;

namespace NMock
{
	/// <summary>
	/// A syntax class to setup expectations on methods when they throw exceptions.
	/// </summary>
	public class That
	{
		#region Constructors/Destructors

		/// <summary>
		/// Creates an instance of this class specifying the action that will throw an exception
		/// </summary>
		/// <param name="action"> </param>
		public That(Action action)
		{
			this._action = action;
		}

		#endregion

		#region Fields/Constants

		private readonly Action _action;
		private Exception _exception;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Indicates that this method will throw an <see cref="Exception" />.
		/// </summary>
		public void Throws()
		{
			this.Throws<Exception>();
		}

		/// <summary>
		/// Indicates that this method will throw an <see cref="Exception" />.
		/// </summary>
		/// <param name="matchers"> An array of matchers to match the exception string. </param>
		public void Throws(params Matcher[] matchers)
		{
			this.Throws<Exception>(string.Empty, matchers);
		}

		/// <summary>
		/// Indicates that this method will throw an <see cref="Exception" />.
		/// </summary>
		/// <param name="comment"> A description of the reason for this expectation. </param>
		public void Throws(string comment)
		{
			this.Throws<Exception>(comment, null);
		}

		/// <summary>
		/// Indicates that this method will throw an <see cref="Exception" />.
		/// </summary>
		/// <param name="comment"> A description of the reason for this expectation. </param>
		/// <param name="matchers"> An array of matchers to match the exception string. </param>
		public void Throws(string comment, params Matcher[] matchers)
		{
			this.Throws<Exception>(comment, matchers);
		}

		/// <summary>
		/// Indicates that this method will throw an <see cref="Exception" />.
		/// </summary>
		/// <typeparam name="T"> The type of <see cref="Exception" /> to throw. </typeparam>
		public void Throws<T>()
			where T : Exception
		{
			this.Throws<T>(string.Empty, null);
		}

		/// <summary>
		/// Indicates that this method will throw an <see cref="Exception" />.
		/// </summary>
		/// <typeparam name="T"> The type of <see cref="Exception" /> to throw. </typeparam>
		/// <param name="comment"> A description of the reason for this expectation. </param>
		public void Throws<T>(string comment)
			where T : Exception
		{
			this.Throws<T>(comment, null);
		}

		/// <summary>
		/// Indicates that this method will throw an <see cref="Exception" />.
		/// </summary>
		/// <typeparam name="T"> The type of <see cref="Exception" /> to throw. </typeparam>
		/// <param name="matchers"> An array of matchers to match the exception string. </param>
		public void Throws<T>(params Matcher[] matchers)
			where T : Exception
		{
			this.Throws<T>(string.Empty, matchers);
		}

		/// <summary>
		/// Indicates that this method will throw an <see cref="Exception" />.
		/// </summary>
		/// <typeparam name="T"> The type of <see cref="Exception" /> to throw. </typeparam>
		/// <param name="comment"> A description of the reason for this expectation. </param>
		/// <param name="matchers"> An array of matchers to match the exception string. </param>
		public void Throws<T>(string comment, params Matcher[] matchers)
			where T : Exception
		{
			try
			{
				this._action();
			}
			catch (T ex)
			{
				this._exception = ex;

				if (matchers != null)
				{
					foreach (var matcher in matchers)
					{
						if (!matcher.Matches(ex.ToString()))
						{
							var writer = new DescriptionWriter();
							writer.Write("Matching failed due to ");
							matcher.DescribeTo(writer);
							throw new Exception(Environment.NewLine + writer + Environment.NewLine, ex);
						}
					}
				}

				//all matchers matched so this was expected

				var uie = ex as UnexpectedInvocationException;
				if (uie != null)
				{
					//clear this exception because it was expected
					uie.Factory.ClearException();
				}
			}

			if (this._exception == null)
			{
				if (string.IsNullOrEmpty(comment))
					throw new UnmetExpectationException("The expected action did not throw an exception when it was invoked.");
				else
					throw new UnmetExpectationException(comment);
			}
		}

		#endregion
	}
}