/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Reflection;

using TextMetal.Framework.Core;
using TextMetal.Framework.Tokenization;
using TextMetal.Framework.XmlDialect;
using TextMetal.Middleware.Solder.Utilities;

namespace TextMetal.Framework.Expression
{
	/// <summary>
	/// This class uses the C# compiler style of numeric promotions.
	/// </summary>
	[XmlElementMapping(LocalName = "UnaryExpression", NamespaceUri = "http://www.textmetal.com/api/v6.0.0", ChildElementModel = ChildElementModel.Sterile)]
	public sealed class UnaryExpressionConstruct : ExpressionXmlObject
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the UnaryExpressionConstruct class.
		/// </summary>
		public UnaryExpressionConstruct()
		{
		}

		#endregion

		#region Fields/Constants

		private IExpressionContainerConstruct theExpression;
		private UnaryOperator unaryOperator;

		#endregion

		#region Properties/Indexers/Events

		[XmlChildElementMapping(ChildElementType = ChildElementType.ParentQualified, LocalName = "TheExpression", NamespaceUri = "http://www.textmetal.com/api/v6.0.0")]
		public IExpressionContainerConstruct TheExpression
		{
			get
			{
				return this.theExpression;
			}
			set
			{
				this.theExpression = value;
			}
		}

		[XmlAttributeMapping(LocalName = "operator", NamespaceUri = "")]
		public UnaryOperator UnaryOperator
		{
			get
			{
				return this.unaryOperator;
			}
			set
			{
				this.unaryOperator = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override object CoreEvaluateExpression(ITemplatingContext templatingContext)
		{
			DynamicWildcardTokenReplacementStrategy dynamicWildcardTokenReplacementStrategy;
			object theObj = null;
			Type theType = null;

			if ((object)templatingContext == null)
				throw new ArgumentNullException(nameof(templatingContext));

			// *** THIS MUST USE THIS OVERLOAD OR CODE WILL FAIL ***
			dynamicWildcardTokenReplacementStrategy = templatingContext.GetDynamicWildcardTokenReplacementStrategy(false);

			if ((object)this.TheExpression != null)
				theObj = this.TheExpression.EvaluateExpression(templatingContext);

			if ((object)theObj != null)
			{
				theType = theObj.GetType();

				var _theTypeInfo = theType.GetTypeInfo();

				if (_theTypeInfo.IsEnum)
				{
					theType = Enum.GetUnderlyingType(theType);
					theObj = ExtensionMethods.DataTypeFascadeLegacyInstance.ChangeType(theObj, theType);
				}
			}

			if ((object)theObj == null &&
				this.UnaryOperator != UnaryOperator.IsNull &&
				this.UnaryOperator != UnaryOperator.IsNotNull)
				return null;

			switch (this.UnaryOperator)
			{
				case UnaryOperator.Not:
				{
					if (theType == typeof(Boolean) || theType == typeof(Boolean?))
					{
						Boolean ths;

						ths = theObj.ChangeType<Boolean>();

						return !ths;
					}

					break;
				}
				case UnaryOperator.IsNull:
				{
					return (object)theObj == null;
				}
				case UnaryOperator.IsNotNull:
				{
					return (object)theObj != null;
				}
				case UnaryOperator.IsDef:
				{
					if (theType == typeof(AspectConstruct))
					{
						AspectConstruct ths;
						object obj;

						ths = theObj.ChangeType<AspectConstruct>();

						if (ExtensionMethods.DataTypeFascadeLegacyInstance.IsWhiteSpace(ths.Name))
							throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message. | dataTypeFascade.ReflectionFascadeLegacyInstance.IsNullOrWhiteSpace(ths)");

						return dynamicWildcardTokenReplacementStrategy.GetByToken(ths.Name, out obj);
					}

					break;
				}
				case UnaryOperator.BComp:
				{
					if (theType == typeof(SByte) || theType == typeof(Byte) || theType == typeof(Int16) || theType == typeof(UInt16) || theType == typeof(Char) ||
						theType == typeof(SByte?) || theType == typeof(Byte?) || theType == typeof(Int16?) || theType == typeof(UInt16?) || theType == typeof(Char?))
					{
						// promote to Int32
						theType = typeof(Int32);
						theObj = theObj.ChangeType<Int32>();
					}

					if (theType == typeof(Int32) ||
						theType == typeof(Int32?))
					{
						Int32 ths;

						ths = theObj.ChangeType<Int32>();

						return ~ths;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ~ths;
					}
					else if (theType == typeof(UInt32) || theType == typeof(UInt32?))
					{
						UInt32 ths;

						ths = theObj.ChangeType<UInt32>();

						return ~ths;
					}
					else if (theType == typeof(UInt64) || theType == typeof(UInt64?))
					{
						UInt64 ths;

						ths = theObj.ChangeType<UInt64>();

						return ~ths;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ~ths;
					}

					break;
				}
				case UnaryOperator.Decr:
				{
					if (theType == typeof(SByte) || theType == typeof(Byte) || theType == typeof(Int16) || theType == typeof(UInt16) || theType == typeof(Char) ||
						theType == typeof(SByte?) || theType == typeof(Byte?) || theType == typeof(Int16?) || theType == typeof(UInt16?) || theType == typeof(Char?))
					{
						// promote to Int32
						theType = typeof(Int32);
						theObj = theObj.ChangeType<Int32>();
					}

					if (theType == typeof(Int32) ||
						theType == typeof(Int32?))
					{
						Int32 ths;

						ths = theObj.ChangeType<Int32>();

						return ths - 1;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ths - 1;
					}
					else if (theType == typeof(UInt32) || theType == typeof(UInt32?))
					{
						UInt32 ths;

						ths = theObj.ChangeType<UInt32>();

						return ths - 1;
					}
					else if (theType == typeof(UInt64) || theType == typeof(UInt64?))
					{
						UInt64 ths;

						ths = theObj.ChangeType<UInt64>();

						return ths - 1;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ths - 1;
					}
					else if (theType == typeof(Single) || theType == typeof(Single?))
					{
						Single ths;

						ths = theObj.ChangeType<Single>();

						return ths - 1;
					}
					else if (theType == typeof(Double) || theType == typeof(Double?))
					{
						Double ths;

						ths = theObj.ChangeType<Double>();

						return ths - 1;
					}
					else if (theType == typeof(Decimal) || theType == typeof(Decimal?))
					{
						Decimal ths;

						ths = theObj.ChangeType<Decimal>();

						return ths - 1;
					}

					break;
				}
				case UnaryOperator.Incr:
				{
					if (theType == typeof(SByte) || theType == typeof(Byte) || theType == typeof(Int16) || theType == typeof(UInt16) || theType == typeof(Char) ||
						theType == typeof(SByte?) || theType == typeof(Byte?) || theType == typeof(Int16?) || theType == typeof(UInt16?) || theType == typeof(Char?))
					{
						// promote to Int32
						theType = typeof(Int32);
						theObj = theObj.ChangeType<Int32>();
					}

					if (theType == typeof(Int32) ||
						theType == typeof(Int32?))
					{
						Int32 ths;

						ths = theObj.ChangeType<Int32>();

						return ths + 1;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ths + 1;
					}
					else if (theType == typeof(UInt32) || theType == typeof(UInt32?))
					{
						UInt32 ths;

						ths = theObj.ChangeType<UInt32>();

						return ths + 1;
					}
					else if (theType == typeof(UInt64) || theType == typeof(UInt64?))
					{
						UInt64 ths;

						ths = theObj.ChangeType<UInt64>();

						return ths + 1;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ths + 1;
					}
					else if (theType == typeof(Single) || theType == typeof(Single?))
					{
						Single ths;

						ths = theObj.ChangeType<Single>();

						return ths + 1;
					}
					else if (theType == typeof(Double) || theType == typeof(Double?))
					{
						Double ths;

						ths = theObj.ChangeType<Double>();

						return ths + 1;
					}
					else if (theType == typeof(Decimal) || theType == typeof(Decimal?))
					{
						Decimal ths;

						ths = theObj.ChangeType<Decimal>();

						return ths + 1;
					}

					break;
				}
				case UnaryOperator.Neg:
				{
					if (theType == typeof(SByte) || theType == typeof(Byte) || theType == typeof(Int16) || theType == typeof(UInt16) || theType == typeof(Char) ||
						theType == typeof(SByte?) || theType == typeof(Byte?) || theType == typeof(Int16?) || theType == typeof(UInt16?) || theType == typeof(Char?))
					{
						// promote to Int32
						theType = typeof(Int32);
						theObj = theObj.ChangeType<Int32>();
					}

					if (theType == typeof(UInt32) ||
						theType == typeof(UInt32?))
					{
						// promote to Int64
						theType = typeof(Int64);
						theObj = theObj.ChangeType<Int64>();
					}

					if (theType == typeof(Int32) ||
						theType == typeof(Int32?))
					{
						Int32 ths;

						ths = theObj.ChangeType<Int32>();

						return ths - 1;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ths - 1;
					}
					else if (theType == typeof(UInt32) || theType == typeof(UInt32?))
					{
						UInt32 ths;

						ths = theObj.ChangeType<UInt32>();

						return ths - 1;
					}
					else if (theType == typeof(UInt64) || theType == typeof(UInt64?))
					{
						UInt64 ths;

						ths = theObj.ChangeType<UInt64>();

						return ths - 1;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ths - 1;
					}
					else if (theType == typeof(Single) || theType == typeof(Single?))
					{
						Single ths;

						ths = theObj.ChangeType<Single>();

						return -ths;
					}
					else if (theType == typeof(Double) || theType == typeof(Double?))
					{
						Double ths;

						ths = theObj.ChangeType<Double>();

						return -ths;
					}
					else if (theType == typeof(Decimal) || theType == typeof(Decimal?))
					{
						Decimal ths;

						ths = theObj.ChangeType<Decimal>();

						return -ths;
					}

					break;
				}
				case UnaryOperator.Pos:
				{
					if (theType == typeof(SByte) || theType == typeof(Byte) || theType == typeof(Int16) || theType == typeof(UInt16) || theType == typeof(Char) ||
						theType == typeof(SByte?) || theType == typeof(Byte?) || theType == typeof(Int16?) || theType == typeof(UInt16?) || theType == typeof(Char?))
					{
						// promote to Int32
						theType = typeof(Int32);
						theObj = theObj.ChangeType<Int32>();
					}

					if (theType == typeof(Int32) ||
						theType == typeof(Int32?))
					{
						Int32 ths;

						ths = theObj.ChangeType<Int32>();

						return ths;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ths;
					}
					else if (theType == typeof(UInt32) || theType == typeof(UInt32?))
					{
						UInt32 ths;

						ths = theObj.ChangeType<UInt32>();

						return ths;
					}
					else if (theType == typeof(UInt64) || theType == typeof(UInt64?))
					{
						UInt64 ths;

						ths = theObj.ChangeType<UInt64>();

						return ths;
					}
					else if (theType == typeof(Int64) || theType == typeof(Int64?))
					{
						Int64 ths;

						ths = theObj.ChangeType<Int64>();

						return ths;
					}
					else if (theType == typeof(Single) || theType == typeof(Single?))
					{
						Single ths;

						ths = theObj.ChangeType<Single>();

						return ths;
					}
					else if (theType == typeof(Double) || theType == typeof(Double?))
					{
						Double ths;

						ths = theObj.ChangeType<Double>();

						return ths;
					}
					else if (theType == typeof(Decimal) || theType == typeof(Decimal?))
					{
						Decimal ths;

						ths = theObj.ChangeType<Decimal>();

						return ths;
					}

					break;
				}
				default:
				{
					throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message. | unary operator is not recognized");
				}
			}

			throw new InvalidOperationException("(?) Something went wrong but the software engineers were too lazy to add a meaningful error message. | type is not supported by the unary operator");
		}

		#endregion
	}
}