// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// The Addin class holds information about an addin.
	/// </summary>
	[Serializable]
	public class Addin
	{
		#region Constructors/Destructors

		/// <summary>
		/// Construct an Addin for a type.
		/// </summary>
		/// <param name="type"> The type to be used </param>
		public Addin(Type type)
		{
			this.typeName = type.AssemblyQualifiedName;

			object[] attrs = type.GetCustomAttributes(typeof(NUnitAddinAttribute), false);
			if (attrs.Length == 1)
			{
				NUnitAddinAttribute attr = (NUnitAddinAttribute)attrs[0];
				this.name = attr.Name;
				this.description = attr.Description;
				this.extensionType = attr.Type;
			}

			if (this.name == null)
				this.name = type.Name;

			if (this.extensionType == 0)
				this.extensionType = ExtensionType.Core;

			this.status = AddinStatus.Enabled;
		}

		#endregion

		#region Fields/Constants

		private string description;
		private ExtensionType extensionType;
		private string message;
		private string name;
		private AddinStatus status;
		private string typeName;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// Brief description of what the Addin does
		/// </summary>
		public string Description
		{
			get
			{
				return this.description;
			}
		}

		/// <summary>
		/// The type or types of extension provided, using
		/// one or more members of the ExtensionType enumeration.
		/// </summary>
		public ExtensionType ExtensionType
		{
			get
			{
				return this.extensionType;
			}
		}

		/// <summary>
		/// Any message that clarifies the status of the Addin,
		/// such as an error message or an explanation of why
		/// the addin is disabled.
		/// </summary>
		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		/// <summary>
		/// The name of the Addin
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// The status of the addin
		/// </summary>
		public AddinStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		/// <summary>
		/// The AssemblyQualifiedName of the type that implements
		/// the addin.
		/// </summary>
		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Return true if two Addins have teh same type name
		/// </summary>
		/// <param name="obj"> The other addin to be compared </param>
		public override bool Equals(object obj)
		{
			Addin addin = obj as Addin;
			if (addin == null)
				return false;

			return this.typeName.Equals(addin.typeName);
		}

		/// <summary>
		/// Return a hash code for this addin
		/// </summary>
		public override int GetHashCode()
		{
			return this.typeName.GetHashCode();
		}

		#endregion
	}
}