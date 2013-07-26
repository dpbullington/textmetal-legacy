// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System.ComponentModel;

using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	/// <summary>
	/// 	Basic Asserts on strings.
	/// </summary>
	public class StringAssert
	{
		#region Methods/Operators

		/// <summary>
		/// 	Asserts that two strings are equal, without regard to case.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The actual string </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void AreEqualIgnoringCase(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new EqualConstraint(expected).IgnoreCase, message, args);
		}

		/// <summary>
		/// 	Asserts that two strings are equal, without regard to case.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The actual string </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void AreEqualIgnoringCase(string expected, string actual, string message)
		{
			AreEqualIgnoringCase(expected, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that two strings are equal, without regard to case.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The actual string </param>
		public static void AreEqualIgnoringCase(string expected, string actual)
		{
			AreEqualIgnoringCase(expected, actual, string.Empty, null);
		}

		/// <summary>
		/// 	Asserts that two strings are not equal, without regard to case.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The actual string </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void AreNotEqualIgnoringCase(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new EqualConstraint(expected).IgnoreCase), message, args);
		}

		/// <summary>
		/// 	Asserts that two strings are Notequal, without regard to case.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The actual string </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void AreNotEqualIgnoringCase(string expected, string actual, string message)
		{
			AreNotEqualIgnoringCase(expected, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that two strings are not equal, without regard to case.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The actual string </param>
		public static void AreNotEqualIgnoringCase(string expected, string actual)
		{
			AreNotEqualIgnoringCase(expected, actual, string.Empty, null);
		}

		/// <summary>
		/// 	Asserts that a string is found within another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void Contains(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new SubstringConstraint(expected), message, args);
		}

		/// <summary>
		/// 	Asserts that a string is found within another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void Contains(string expected, string actual, string message)
		{
			Contains(expected, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that a string is found within another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		public static void Contains(string expected, string actual)
		{
			Contains(expected, actual, string.Empty, null);
		}

		/// <summary>
		/// 	Asserts that a string is not found within another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void DoesNotContain(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new SubstringConstraint(expected)), message, args);
		}

		/// <summary>
		/// 	Asserts that a string is found within another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void DoesNotContain(string expected, string actual, string message)
		{
			DoesNotContain(expected, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that a string is found within another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		public static void DoesNotContain(string expected, string actual)
		{
			DoesNotContain(expected, actual, string.Empty, null);
		}

		/// <summary>
		/// 	Asserts that a string does not end with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void DoesNotEndWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new EndsWithConstraint(expected)), message, args);
		}

		/// <summary>
		/// 	Asserts that a string does not end with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void DoesNotEndWith(string expected, string actual, string message)
		{
			DoesNotEndWith(expected, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that a string does not end with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		public static void DoesNotEndWith(string expected, string actual)
		{
			DoesNotEndWith(expected, actual, string.Empty, null);
		}

		/// <summary>
		/// 	Asserts that a string does not match an expected regular expression pattern.
		/// </summary>
		/// <param name="pattern"> The regex pattern to be used </param>
		/// <param name="actual"> The actual string </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void DoesNotMatch(string pattern, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new RegexConstraint(pattern)), message, args);
		}

		/// <summary>
		/// 	Asserts that a string does not match an expected regular expression pattern.
		/// </summary>
		/// <param name="pattern"> The regex pattern to be used </param>
		/// <param name="actual"> The actual string </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void DoesNotMatch(string pattern, string actual, string message)
		{
			DoesNotMatch(pattern, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that a string does not match an expected regular expression pattern.
		/// </summary>
		/// <param name="pattern"> The regex pattern to be used </param>
		/// <param name="actual"> The actual string </param>
		public static void DoesNotMatch(string pattern, string actual)
		{
			DoesNotMatch(pattern, actual, string.Empty, null);
		}

		/// <summary>
		/// 	Asserts that a string does not start with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void DoesNotStartWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new StartsWithConstraint(expected)), message, args);
		}

		/// <summary>
		/// 	Asserts that a string does not start with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void DoesNotStartWith(string expected, string actual, string message)
		{
			DoesNotStartWith(expected, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that a string does not start with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		public static void DoesNotStartWith(string expected, string actual)
		{
			DoesNotStartWith(expected, actual, string.Empty, null);
		}

		/// <summary>
		/// 	Asserts that a string ends with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void EndsWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new EndsWithConstraint(expected), message, args);
		}

		/// <summary>
		/// 	Asserts that a string ends with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void EndsWith(string expected, string actual, string message)
		{
			EndsWith(expected, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that a string ends with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		public static void EndsWith(string expected, string actual)
		{
			EndsWith(expected, actual, string.Empty, null);
		}

		/// <summary>
		/// 	The Equals method throws an AssertionException. This is done 
		/// 	to make sure there is no mistake by calling this function.
		/// </summary>
		/// <param name="a"> </param>
		/// <param name="b"> </param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new AssertionException("Assert.Equals should not be used for Assertions");
		}

		/// <summary>
		/// 	Asserts that a string matches an expected regular expression pattern.
		/// </summary>
		/// <param name="pattern"> The regex pattern to be matched </param>
		/// <param name="actual"> The actual string </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void IsMatch(string pattern, string actual, string message, params object[] args)
		{
			Assert.That(actual, new RegexConstraint(pattern), message, args);
		}

		/// <summary>
		/// 	Asserts that a string matches an expected regular expression pattern.
		/// </summary>
		/// <param name="pattern"> The regex pattern to be matched </param>
		/// <param name="actual"> The actual string </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void IsMatch(string pattern, string actual, string message)
		{
			IsMatch(pattern, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that a string matches an expected regular expression pattern.
		/// </summary>
		/// <param name="pattern"> The regex pattern to be matched </param>
		/// <param name="actual"> The actual string </param>
		public static void IsMatch(string pattern, string actual)
		{
			IsMatch(pattern, actual, string.Empty, null);
		}

		/// <summary>
		/// 	override the default ReferenceEquals to throw an AssertionException. This 
		/// 	implementation makes sure there is no mistake in calling this function 
		/// 	as part of Assert.
		/// </summary>
		/// <param name="a"> </param>
		/// <param name="b"> </param>
		public new static void ReferenceEquals(object a, object b)
		{
			throw new AssertionException("Assert.ReferenceEquals should not be used for Assertions");
		}

		/// <summary>
		/// 	Asserts that a string starts with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		/// <param name="args"> Arguments used in formatting the message </param>
		public static void StartsWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new StartsWithConstraint(expected), message, args);
		}

		/// <summary>
		/// 	Asserts that a string starts with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		/// <param name="message"> The message to display in case of failure </param>
		public static void StartsWith(string expected, string actual, string message)
		{
			StartsWith(expected, actual, message, null);
		}

		/// <summary>
		/// 	Asserts that a string starts with another string.
		/// </summary>
		/// <param name="expected"> The expected string </param>
		/// <param name="actual"> The string to be examined </param>
		public static void StartsWith(string expected, string actual)
		{
			StartsWith(expected, actual, string.Empty, null);
		}

		#endregion
	}
}