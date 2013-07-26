// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
#if !NETCF
#endif

namespace NUnit.Framework.Constraints
{
#if !NETCF

	#region BinarySerializableConstraint

	/// <summary>
	/// 	BinarySerializableConstraint tests whether 
	/// 	an object is serializable in binary format.
	/// </summary>
	public class BinarySerializableConstraint : Constraint
	{
		#region Fields/Constants

		private readonly BinaryFormatter serializer = new BinaryFormatter();

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Returns the string representation
		/// </summary>
		protected override string GetStringRepresentation()
		{
			return "<binaryserializable>";
		}

		/// <summary>
		/// 	Test whether the constraint is satisfied by a given value
		/// </summary>
		/// <param name="actual"> The value to be tested </param>
		/// <returns> True for success, false for failure </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (actual == null)
				throw new ArgumentException();

			MemoryStream stream = new MemoryStream();

			try
			{
				this.serializer.Serialize(stream, actual);

				stream.Seek(0, SeekOrigin.Begin);

				object value = this.serializer.Deserialize(stream);

				return value != null;
			}
			catch (SerializationException)
			{
				return false;
			}
		}

		/// <summary>
		/// 	Write the actual value for a failing constraint test to a
		/// 	MessageWriter. The default implementation simply writes
		/// 	the raw value of actual, leaving it to the writer to
		/// 	perform any formatting.
		/// </summary>
		/// <param name="writer"> The writer on which the actual value is displayed </param>
		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.Write("<{0}>", this.actual.GetType().Name);
		}

		/// <summary>
		/// 	Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("binary serializable");
		}

		#endregion
	}

	#endregion

#endif

#if !NETCF_1_0

	#region XmlSerializableConstraint

	/// <summary>
	/// 	BinarySerializableConstraint tests whether 
	/// 	an object is serializable in binary format.
	/// </summary>
	public class XmlSerializableConstraint : Constraint
	{
		#region Fields/Constants

		private XmlSerializer serializer;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// 	Returns the string representation of this constraint
		/// </summary>
		protected override string GetStringRepresentation()
		{
			return "<xmlserializable>";
		}

		/// <summary>
		/// 	Test whether the constraint is satisfied by a given value
		/// </summary>
		/// <param name="actual"> The value to be tested </param>
		/// <returns> True for success, false for failure </returns>
		public override bool Matches(object actual)
		{
			this.actual = actual;

			if (actual == null)
				throw new ArgumentException();

			MemoryStream stream = new MemoryStream();

			try
			{
				this.serializer = new XmlSerializer(actual.GetType());

				this.serializer.Serialize(stream, actual);

				stream.Seek(0, SeekOrigin.Begin);

				object value = this.serializer.Deserialize(stream);

				return value != null;
			}
			catch (NotSupportedException)
			{
				return false;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}

		/// <summary>
		/// 	Write the actual value for a failing constraint test to a
		/// 	MessageWriter. The default implementation simply writes
		/// 	the raw value of actual, leaving it to the writer to
		/// 	perform any formatting.
		/// </summary>
		/// <param name="writer"> The writer on which the actual value is displayed </param>
		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.Write("<{0}>", this.actual.GetType().Name);
		}

		/// <summary>
		/// 	Write the constraint description to a MessageWriter
		/// </summary>
		/// <param name="writer"> The writer on which the description is displayed </param>
		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("xml serializable");
		}

		#endregion
	}

	#endregion

#endif
}