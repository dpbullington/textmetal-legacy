// File: CommandLineOptions.cs
//
// This is a re-usable component to be used when you 
// need to parse command-line options/parameters.
//
// Separates command line parameters from command line options.
// Uses reflection to populate member variables the derived class with the values 
// of the options.
//
// An option can start with "-" or "--". On Windows systems, it can start with "/" as well.
//
// I define 3 types of "options":
//   1. Boolean options (yes/no values), e.g: /r to recurse
//   2. Value options, e.g: /loglevel=3
//   2. Parameters: standalone strings like file names
//
// An example to explain:
//   csc /nologo /t:exe myfile.cs
//       |       |      |
//       |       |      + parameter
//       |       |
//       |       + value option
//       |
//       + boolean option
//
// Please see a short description of the CommandLineOptions class
// at http://codeblast.com/~gert/dotnet/sells.html
// 
// Gert Lombard (gert@codeblast.com)
// James Newkirk (jim@nunit.org)

using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

namespace Codeblast
{
	using System;

	//
	// The Attributes
	//

	[AttributeUsage(AttributeTargets.Field)]
	public class OptionAttribute : Attribute
	{
		#region Fields/Constants

		protected string description;
		protected string optName;
		protected object optValue;

		#endregion

		#region Properties/Indexers/Events

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public string Short
		{
			get
			{
				return this.optName;
			}
			set
			{
				this.optName = value;
			}
		}

		public object Value
		{
			get
			{
				return this.optValue;
			}
			set
			{
				this.optValue = value;
			}
		}

		#endregion
	}

	//
	// The CommandLineOptions members
	//

	public abstract class CommandLineOptions
	{
		#region Constructors/Destructors

		public CommandLineOptions(string[] args)
			: this(Path.DirectorySeparatorChar != '/', args)
		{
		}

		public CommandLineOptions(bool allowForwardSlash, string[] args)
		{
			this.allowForwardSlash = allowForwardSlash;
			this.optionCount = this.Init(args);
		}

		#endregion

		#region Fields/Constants

		private bool allowForwardSlash;
		private ArrayList invalidArguments = new ArrayList();
		protected bool isInvalid = false;

		private int optionCount;
		protected ArrayList parameters;

		#endregion

		#region Properties/Indexers/Events

		public string this[int index]
		{
			get
			{
				if (this.parameters != null)
					return (string)this.parameters[index];
				return null;
			}
		}

		public bool AllowForwardSlash
		{
			get
			{
				return this.allowForwardSlash;
			}
		}

		public IList InvalidArguments
		{
			get
			{
				return this.invalidArguments;
			}
		}

		public bool NoArgs
		{
			get
			{
				return this.ParameterCount == 0 && this.optionCount == 0;
			}
		}

		public int ParameterCount
		{
			get
			{
				return this.parameters == null ? 0 : this.parameters.Count;
			}
		}

		public ArrayList Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual string GetHelpText()
		{
			StringBuilder helpText = new StringBuilder();

			Type t = this.GetType();
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
			char optChar = this.allowForwardSlash ? '/' : '-';
			foreach (FieldInfo field in fields)
			{
				object[] atts = field.GetCustomAttributes(typeof(OptionAttribute), true);
				if (atts.Length > 0)
				{
					OptionAttribute att = (OptionAttribute)atts[0];
					if (att.Description != null)
					{
						string valType = "";
						if (att.Value == null)
						{
							if (field.FieldType == typeof(float))
								valType = "=FLOAT";
							else if (field.FieldType == typeof(string))
								valType = "=STR";
							else if (field.FieldType != typeof(bool))
								valType = "=X";
						}

						helpText.AppendFormat("{0}{1,-20}\t{2}", optChar, field.Name + valType, att.Description);
						if (att.Short != null)
							helpText.AppendFormat(" (Short format: {0}{1}{2})", optChar, att.Short, valType);
						helpText.Append(Environment.NewLine);
					}
				}
			}
			return helpText.ToString();
		}

		protected virtual FieldInfo GetMemberField(string name)
		{
			Type t = this.GetType();
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo field in fields)
			{
				if (string.Compare(field.Name, name, true) == 0)
					return field;
				if (this.MatchShortName(field, name))
					return field;
			}
			return null;
		}

		protected virtual bool GetOption(string[] args, ref int index, int pos)
		{
			try
			{
				object cmdLineVal = null;
				string opt = args[index].Substring(pos, args[index].Length - pos);
				this.SplitOptionAndValue(ref opt, ref cmdLineVal);
				FieldInfo field = this.GetMemberField(opt);
				if (field != null)
				{
					object value = this.GetOptionValue(field);
					if (value == null)
					{
						if (field.FieldType == typeof(bool))
							value = true; // default for bool values is true
						else if (field.FieldType == typeof(string))
						{
							value = cmdLineVal != null ? cmdLineVal : args[++index];
							field.SetValue(this, Convert.ChangeType(value, field.FieldType));
							string stringValue = (string)value;
							if (stringValue == null || stringValue.Length == 0)
								return false;
							return true;
						}
						else if (field.FieldType.IsEnum)
							value = Enum.Parse(field.FieldType, (string)cmdLineVal, true);
						else
							value = cmdLineVal != null ? cmdLineVal : args[++index];
					}
					field.SetValue(this, Convert.ChangeType(value, field.FieldType));
					return true;
				}
			}
			catch (Exception)
			{
				// Ignore exceptions like type conversion errors.
			}
			return false;
		}

		protected virtual object GetOptionValue(FieldInfo field)
		{
			object[] atts = field.GetCustomAttributes(typeof(OptionAttribute), true);
			if (atts.Length > 0)
			{
				OptionAttribute att = (OptionAttribute)atts[0];
				return att.Value;
			}
			return null;
		}

		public virtual void Help()
		{
			Console.WriteLine(this.GetHelpText());
		}

		public int Init(params string[] args)
		{
			int count = 0;
			int n = 0;
			while (n < args.Length)
			{
				int pos = this.IsOption(args[n]);
				if (pos > 0)
				{
					// It's an option:
					if (this.GetOption(args, ref n, pos))
						count++;
					else
						this.InvalidOption(args[Math.Min(n, args.Length - 1)]);
				}
				else
				{
					if (this.parameters == null)
						this.parameters = new ArrayList();
					this.parameters.Add(args[n]);
					if (!this.IsValidParameter(args[n]))
						this.InvalidOption(args[n]);
				}
				n++;
			}
			return count;
		}

		protected virtual void InvalidOption(string name)
		{
			this.invalidArguments.Add(name);
			this.isInvalid = true;
		}

		// An option starts with "/", "-" or "--":
		protected virtual int IsOption(string opt)
		{
			char[] c = null;
			if (opt.Length < 2)
				return 0;
			else if (opt.Length > 2)
			{
				c = opt.ToCharArray(0, 3);
				if (c[0] == '-' && c[1] == '-' && this.IsOptionNameChar(c[2]))
					return 2;
			}
			else
				c = opt.ToCharArray(0, 2);
			if ((c[0] == '-' || c[0] == '/' && this.AllowForwardSlash) && this.IsOptionNameChar(c[1]))
				return 1;
			return 0;
		}

		protected virtual bool IsOptionNameChar(char c)
		{
			return Char.IsLetterOrDigit(c) || c == '?';
		}

		protected virtual bool IsValidParameter(string param)
		{
			return true;
		}

		protected virtual bool MatchShortName(FieldInfo field, string name)
		{
			object[] atts = field.GetCustomAttributes(typeof(OptionAttribute), true);
			foreach (OptionAttribute att in atts)
			{
				if (string.Compare(att.Short, name, true) == 0)
					return true;
			}
			return false;
		}

		protected virtual void SplitOptionAndValue(ref string opt, ref object val)
		{
			// Look for ":" or "=" separator in the option:
			int pos = opt.IndexOfAny(new char[] { ':', '=' });
			if (pos < 1)
				return;

			val = opt.Substring(pos + 1);
			opt = opt.Substring(0, pos);
		}

		#endregion

		// Parameter accessor:
	}
}