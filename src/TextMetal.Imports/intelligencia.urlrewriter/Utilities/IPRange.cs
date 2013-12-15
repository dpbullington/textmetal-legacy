// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Intelligencia.UrlRewriter.Utilities
{
	/// <summary>
	/// Represents a range of IP addresses.
	/// </summary>
	public sealed class IPRange
	{
		#region Constructors/Destructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="address"> A range of 1 IP address. </param>
		public IPRange(IPAddress address)
		{
			this._minimumAddress = address;
			this._maximumAddress = address;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="minimumAddress"> Lowest IP address. </param>
		/// <param name="maximumAddress"> Highest IP address. </param>
		public IPRange(IPAddress minimumAddress, IPAddress maximumAddress)
		{
			if (Compare(minimumAddress, maximumAddress) == -1)
			{
				this._minimumAddress = minimumAddress;
				this._maximumAddress = maximumAddress;
			}
			else
			{
				this._minimumAddress = maximumAddress;
				this._maximumAddress = minimumAddress;
			}
		}

		#endregion

		#region Fields/Constants

		private IPAddress _maximumAddress;
		private IPAddress _minimumAddress;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Maximum address (inclusive).
		/// </summary>
		public IPAddress MaximumAddress
		{
			get
			{
				return this._maximumAddress;
			}
		}

		/// <summary>
		/// Minimum address (inclusive).
		/// </summary>
		public IPAddress MinimumAddress
		{
			get
			{
				return this._minimumAddress;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Compares two IPAddresses.
		/// Less than zero {left} is less than {right}.
		/// Zero {left} equals {right}.
		/// Greater than zero {left} is greater than {right}.
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"> </param>
		/// <returns> </returns>
		public static int Compare(IPAddress left, IPAddress right)
		{
			if (left == null)
				throw new ArgumentNullException("left");
			if (right == null)
				throw new ArgumentNullException("right");

			byte[] leftBytes = left.GetAddressBytes();
			byte[] rightBytes = right.GetAddressBytes();
			if (leftBytes.Length != rightBytes.Length)
				throw new ArgumentOutOfRangeException(MessageProvider.FormatString(Message.AddressesNotOfSameType));

			for (int i = 0; i < leftBytes.Length; i++)
			{
				if (leftBytes[i] == rightBytes[i])
					continue;

				return leftBytes[i] - rightBytes[i];
			}

			return 0;
		}

		/// <summary>
		/// Parses an IP address range.
		/// </summary>
		/// <remarks>
		/// ddd.ddd.ddd.ddd - single IP address
		/// ddd.ddd.ddd.* - class C range
		/// ddd.ddd.* - class B range
		/// ddd.* - class A range
		/// ddd.ddd.ddd.ddd - ccc.ccc.ccc.ccc - specific range
		/// </remarks>
		/// <param name="pattern"> The pattern </param>
		/// <returns> The IPRange instance. </returns>
		public static IPRange Parse(string pattern)
		{
			pattern = Regex.Replace(pattern, @"([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})\.\*", @"$1.0-$1.255");
			pattern = Regex.Replace(pattern, @"([0-9]{1,3}\.[0-9]{1,3})\.\*", @"$1.0.0-$1.255.255");
			pattern = Regex.Replace(pattern, @"([0-9]{1,3})\.\*", @"$1.0.0.0-$1.255.255.255");

			string[] parts = pattern.Split('-');
			return (parts.Length > 1)
				? new IPRange(IPAddress.Parse(parts[0].Trim()), IPAddress.Parse(parts[1].Trim()))
				: new IPRange(IPAddress.Parse(pattern.Trim()));
		}

		/// <summary>
		/// Deteremines if the given IP address is in the range.
		/// </summary>
		/// <param name="address"> The IP address. </param>
		/// <returns> True if the address is in the range. </returns>
		public bool InRange(IPAddress address)
		{
			return Compare(this.MinimumAddress, address) <= 0 && Compare(address, this.MaximumAddress) <= 0;
		}

		#endregion
	}
}