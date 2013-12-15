// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Xml;

using Intelligencia.UrlRewriter.Logging;
using Intelligencia.UrlRewriter.Parsers;
using Intelligencia.UrlRewriter.Transforms;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter.Configuration
{
	/// <summary>
	/// Configuration for the URL rewriter.
	/// </summary>
	public class RewriterConfiguration : IRewriterConfiguration
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public RewriterConfiguration()
			: this(new ConfigurationManagerFacade())
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="configurationManager"> The configuration manager instance </param>
		public RewriterConfiguration(IConfigurationManager configurationManager)
		{
			if (configurationManager == null)
				throw new ArgumentNullException("configurationManager");

			this._configurationManager = configurationManager;

			this._xPoweredBy = MessageProvider.FormatString(Message.ProductName, Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

			this._actionParserFactory = new ActionParserFactory();
			this._actionParserFactory.AddParser(new IfConditionActionParser());
			this._actionParserFactory.AddParser(new UnlessConditionActionParser());
			this._actionParserFactory.AddParser(new AddHeaderActionParser());
			this._actionParserFactory.AddParser(new SetCookieActionParser());
			this._actionParserFactory.AddParser(new SetPropertyActionParser());
			this._actionParserFactory.AddParser(new SetAppSettingPropertyActionParser());
			this._actionParserFactory.AddParser(new RewriteActionParser());
			this._actionParserFactory.AddParser(new RedirectActionParser());
			this._actionParserFactory.AddParser(new SetStatusActionParser());
			this._actionParserFactory.AddParser(new ForbiddenActionParser());
			this._actionParserFactory.AddParser(new GoneActionParser());
			this._actionParserFactory.AddParser(new NotAllowedActionParser());
			this._actionParserFactory.AddParser(new NotFoundActionParser());
			this._actionParserFactory.AddParser(new NotImplementedActionParser());

			this._conditionParserPipeline = new ConditionParserPipeline();
			this._conditionParserPipeline.AddParser(new AddressConditionParser());
			this._conditionParserPipeline.AddParser(new HeaderMatchConditionParser());
			this._conditionParserPipeline.AddParser(new MethodConditionParser());
			this._conditionParserPipeline.AddParser(new PropertyMatchConditionParser());
			this._conditionParserPipeline.AddParser(new ExistsConditionParser());
			this._conditionParserPipeline.AddParser(new UrlMatchConditionParser());

			this._transformFactory = new TransformFactory();
			this._transformFactory.AddTransform(new DecodeTransform());
			this._transformFactory.AddTransform(new EncodeTransform());
			this._transformFactory.AddTransform(new LowerTransform());
			this._transformFactory.AddTransform(new UpperTransform());
			this._transformFactory.AddTransform(new Base64Transform());
			this._transformFactory.AddTransform(new Base64DecodeTransform());

			this._defaultDocuments = new StringCollection();

			this.LoadFromConfig();
		}

		#endregion

		#region Fields/Constants

		private ActionParserFactory _actionParserFactory;
		private ConditionParserPipeline _conditionParserPipeline;
		private IConfigurationManager _configurationManager;
		private StringCollection _defaultDocuments;
		private IDictionary<int, IRewriteErrorHandler> _errorHandlers = new Dictionary<int, IRewriteErrorHandler>();
		private IRewriteLogger _logger = new NullLogger();
		private IList<IRewriteAction> _rules = new List<IRewriteAction>();
		private TransformFactory _transformFactory;
		private string _xPoweredBy;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The action parser factory.
		/// </summary>
		public ActionParserFactory ActionParserFactory
		{
			get
			{
				return this._actionParserFactory;
			}
		}

		/// <summary>
		/// The condition parser pipeline.
		/// </summary>
		public ConditionParserPipeline ConditionParserPipeline
		{
			get
			{
				return this._conditionParserPipeline;
			}
		}

		/// <summary>
		/// The configuration manager instance.
		/// </summary>
		public IConfigurationManager ConfigurationManager
		{
			get
			{
				return this._configurationManager;
			}
		}

		/// <summary>
		/// Collection of default document names to use if the result of a rewriting
		/// is a directory name.
		/// </summary>
		public StringCollection DefaultDocuments
		{
			get
			{
				return this._defaultDocuments;
			}
		}

		/// <summary>
		/// Dictionary of error handlers.
		/// </summary>
		public IDictionary<int, IRewriteErrorHandler> ErrorHandlers
		{
			get
			{
				return this._errorHandlers;
			}
		}

		/// <summary>
		/// Logger to use for logging information.
		/// </summary>
		public IRewriteLogger Logger
		{
			get
			{
				return this._logger;
			}
			set
			{
				this._logger = value;
			}
		}

		/// <summary>
		/// The rules.
		/// </summary>
		public IList<IRewriteAction> Rules
		{
			get
			{
				return this._rules;
			}
		}

		/// <summary>
		/// The transform factory.
		/// </summary>
		public TransformFactory TransformFactory
		{
			get
			{
				return this._transformFactory;
			}
		}

		/// <summary>
		/// Additional X-Powered-By header.
		/// </summary>
		public string XPoweredBy
		{
			get
			{
				return this._xPoweredBy;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Loads the rewriter configuration from the web.config file.
		/// </summary>
		private void LoadFromConfig()
		{
			XmlNode section = this._configurationManager.GetSection(Constants.RewriterNode) as XmlNode;
			if (section == null)
				throw new ConfigurationErrorsException(MessageProvider.FormatString(Message.MissingConfigFileSection, Constants.RewriterNode), section);

			RewriterConfigurationReader.Read(this, section);
		}

		#endregion
	}
}