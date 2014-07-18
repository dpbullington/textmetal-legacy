/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;

using TextMetal.Common.Core;
using TextMetal.Common.Core.StringTokens;
using TextMetal.Common.Xml;
using TextMetal.Framework.Core;
using TextMetal.Framework.ExpressionModel;
using TextMetal.Framework.HostingModel;
using TextMetal.Framework.InputOutputModel;
using TextMetal.Framework.SourceModel.Primative;
using TextMetal.Framework.TemplateModel;

namespace TextMetal.HostImpl.Web.AspNet
{
	public sealed class WebHost : IWebHost
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the WebHost class.
		/// </summary>
		public WebHost()
		{
		}

		#endregion

		#region Methods/Operators

		public void Host(HttpContext httpContext, string templateFilePath, object source, TextWriter textWriter)
		{
			IXmlPersistEngine xpe;
			TemplateConstruct template;
			Dictionary<string, object> globalVariableTable;
			string toolVersion;
			string templateDirectoryPath;

			if ((object)templateFilePath == null)
				throw new ArgumentNullException("templateFilePath");

			if ((object)textWriter == null)
				throw new ArgumentNullException("textWriter");

			if (DataType.IsWhiteSpace(templateFilePath))
				throw new ArgumentOutOfRangeException("templateFilePath");

			toolVersion = Assembly.GetAssembly(typeof(IXmlPersistEngine)).GetName().Version.ToString();
			templateFilePath = Path.GetFullPath(templateFilePath);
			templateDirectoryPath = Path.GetDirectoryName(templateFilePath);

			xpe = new XmlPersistEngine();
			xpe.RegisterWellKnownConstructs();

			template = (TemplateConstruct)xpe.DeserializeFromXml(templateFilePath);

			using (IInputMechanism inputMechanism = new FileInputMechanism(templateDirectoryPath, xpe, new NullSourceStrategy())) // relative to template
			{
				using (IOutputMechanism outputMechanism = new TextWriterOutputMechanism(textWriter))
				{
					using (ITemplatingContext templatingContext = new TemplatingContext(xpe, new Tokenizer(true), inputMechanism, outputMechanism, new Dictionary<string, IList<string>>()))
					{
						templatingContext.Tokenizer.TokenReplacementStrategies.Add("StaticPropertyResolver", new DynamicValueTokenReplacementStrategy(DynamicValueTokenReplacementStrategy.StaticPropertyResolver));
						templatingContext.Tokenizer.TokenReplacementStrategies.Add("StaticMethodResolver", new DynamicValueTokenReplacementStrategy(DynamicValueTokenReplacementStrategy.StaticMethodResolver));
						templatingContext.Tokenizer.TokenReplacementStrategies.Add("rb", new ContextualDynamicValueTokenReplacementStrategy(RubyConstruct.RubyExpressionResolver, new object[] { templatingContext }));

						// globals
						templatingContext.VariableTables.Push(globalVariableTable = new Dictionary<string, object>());
						globalVariableTable.Add("ToolVersion", toolVersion);

						// add HTTP request properties to GVT
						foreach (string key in httpContext.Request.Params.AllKeys)
						{
							string value = httpContext.Request.Params[key];
							globalVariableTable.Add(key, value);
						}

						templatingContext.IteratorModels.Push(source);
						template.ExpandTemplate(templatingContext);
						templatingContext.IteratorModels.Pop();
						templatingContext.VariableTables.Pop();
					}
				}
			}
		}

		#endregion
	}
}