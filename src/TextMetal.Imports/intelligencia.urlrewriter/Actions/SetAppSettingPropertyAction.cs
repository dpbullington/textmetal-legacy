// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter.Actions
{
	/// <summary>
	/// Action that sets a property in the context from AppSettings, i.e the appSettings collection
	/// in web.config.
	/// </summary>
	public class SetAppSettingPropertyAction : IRewriteAction
	{
		#region Constructors/Destructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="name"> The name of the variable. </param>
		/// <param name="appSettingsKey"> The name of the key in AppSettings. </param>
		public SetAppSettingPropertyAction(string name, string appSettingsKey)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (appSettingsKey == null)
				throw new ArgumentNullException("appSettingsKey");

			this._name = name;
			this._appSettingsKey = appSettingsKey;
		}

		#endregion

		#region Fields/Constants

		private string _appSettingsKey;
		private string _name;

		#endregion

		#region Properties/Indexers/Events

		/// <summary>
		/// The name of the key in AppSettings.
		/// </summary>
		public string AppSettingKey
		{
			get
			{
				return this._appSettingsKey;
			}
		}

		/// <summary>
		/// The name of the variable.
		/// </summary>
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Executes the action.
		/// </summary>
		/// <param name="context"> The rewrite context. </param>
		public RewriteProcessing Execute(RewriteContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			// If the value cannot be found in AppSettings, default to an empty string.
			string appSettingValue = context.ConfigurationManager.AppSettings[this._appSettingsKey] ?? String.Empty;
			context.Properties.Set(this.Name, appSettingValue);
			return RewriteProcessing.ContinueProcessing;
		}

		#endregion
	}
}