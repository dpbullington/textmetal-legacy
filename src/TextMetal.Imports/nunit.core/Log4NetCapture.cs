// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.IO;
using System.Reflection;

using BF = System.Reflection.BindingFlags;

namespace NUnit.Core
{
	/// <summary>
	/// 	Proxy class for operations on a real log4net appender,
	/// 	allowing NUnit to work with multiple versions of log4net
	/// 	and to fail gracefully if no log4net assembly is present.
	/// </summary>
	public class Log4NetCapture : TextCapture
	{
		#region Fields/Constants

		private static readonly string logFormat =
			"%d{ABSOLUTE} %-5p [%4t] %c{1} [%x]- %m%n";

		private object appender;

		private Type appenderType;
		private Type basicConfiguratorType;

		private bool isInitialized;
		private Assembly log4netAssembly;

		/// <summary>
		/// 	The threshold for capturing text. A value of "Off"
		/// 	means that no text is captured. A value of "All"
		/// 	should be taken to mean the highest possible level
		/// 	of verbosity supported by the derived class. The 
		/// 	meaning of any other values is determined by the 
		/// 	derived class.
		/// </summary>
		private LoggingThreshold threshold = LoggingThreshold.Off;

		/// <summary>
		/// 	The TextWriter to which text is redirected
		/// </summary>
		private TextWriter writer;

		#endregion

		// Layout codes that work for versions from 
		// log4net 1.2.0.30714 to 1.2.10:
		//
		//	%a = domain friendly name
		//	%c = logger name (%c{1} = last component )
		//	%d = date and time
		//	%d{ABSOLUTE} = time only
		//	%l = source location of the error
		//	%m = message
		//	%n = newline
		//	%p = level
		//	%r = elapsed milliseconds since program start
		//	%t = thread
		//	%x = nested diagnostic content (NDC)

		#region Properties/Indexers/Events

		private bool IsLog4netAvailable
		{
			get
			{
				if (!this.isInitialized)
					this.InitializeTypes();

				return this.log4netAssembly != null && this.basicConfiguratorType != null && this.appenderType != null;
			}
		}

		/// <summary>
		/// 	Gets or sets the capture threshold value, which represents
		/// 	the degree of verbosity of the output text stream.
		/// 	Derived classes may supply multiple levels of capture but
		/// 	must retain the use of the "Off" setting to represent 
		/// 	no logging.
		/// </summary>
		public override LoggingThreshold Threshold
		{
			get
			{
				return this.threshold;
			}
			set
			{
				if (value != this.threshold)
				{
					bool turnOff = value == LoggingThreshold.Off;
					//bool turnOn = threshold == LoggingThreshold.Off;

					//if (turnOff)
					this.StopCapture();

					this.threshold = value;

					if (!turnOff)
						this.StartCapture();
				}
			}
		}

		/// <summary>
		/// 	Gets or sets the TextWriter to which text is redirected
		/// 	when captured. The value may only be changed when the
		/// 	logging threshold is set to "Off"
		/// </summary>
		public override TextWriter Writer
		{
			get
			{
				return this.writer;
			}
			set
			{
				if (this.threshold != LoggingThreshold.Off)
				{
					throw new InvalidOperationException(
						"Writer may not be changed while capture is enabled");
				}

				this.writer = value;
			}
		}

		#endregion

		#region Methods/Operators

		private void ConfigureAppender()
		{
			MethodInfo configureMethod = this.basicConfiguratorType.GetMethod("Configure", new Type[] { this.appenderType });
			if (configureMethod != null)
				configureMethod.Invoke(null, new object[] { this.appender });
		}

		private void InitializeTypes()
		{
			try
			{
				this.log4netAssembly = Assembly.Load("log4net");

				if (this.log4netAssembly != null)
				{
					this.appenderType = this.log4netAssembly.GetType(
						"log4net.Appender.TextWriterAppender", false, false);

					this.basicConfiguratorType = this.log4netAssembly.GetType(
						"log4net.Config.BasicConfigurator", false, false);

					this.appender = this.TryCreateAppender();
					if (this.appender != null)
						this.SetAppenderLogFormat(logFormat);
				}
			}
			catch
			{
			}
			finally
			{
				this.isInitialized = true;
			}
		}

		private void ResumeCapture()
		{
			if (this.IsLog4netAvailable)
			{
				this.SetLoggingThreshold(this.Threshold.ToString());
				this.ConfigureAppender();
			}
		}

		private void SetAppenderLogFormat(string logFormat)
		{
			Type patternLayoutType = this.log4netAssembly.GetType(
				"log4net.Layout.PatternLayout", false, false);
			if (patternLayoutType == null)
				return;

			ConstructorInfo ctor = patternLayoutType.GetConstructor(new Type[] { typeof(string) });
			if (ctor != null)
			{
				object patternLayout = ctor.Invoke(new object[] { logFormat });

				if (patternLayout != null)
				{
					PropertyInfo prop = this.appenderType.GetProperty("Layout", BF.Public | BF.Instance | BF.SetProperty);
					if (prop != null)
						prop.SetValue(this.appender, patternLayout, null);
				}
			}
		}

		private void SetAppenderTextWriter(TextWriter writer)
		{
			PropertyInfo prop = this.appenderType.GetProperty("Writer", BF.Instance | BF.Public | BF.SetProperty);
			if (prop != null)
				prop.SetValue(this.appender, writer, null);
		}

		private bool SetLoggingThreshold(string threshold)
		{
			PropertyInfo prop = this.appenderType.GetProperty("Threshold", BF.Public | BF.Instance | BF.SetProperty);
			if (prop == null)
				return false;

			Type levelType = prop.PropertyType;
			FieldInfo levelField = levelType.GetField(threshold, BF.Public | BF.Static | BF.IgnoreCase);
			if (levelField == null)
				return false;

			object level = levelField.GetValue(null);
			prop.SetValue(this.appender, level, null);
			return true;
		}

		private void StartCapture()
		{
			if (this.IsLog4netAvailable)
			{
				string threshold = this.Threshold.ToString();
				if (!this.SetLoggingThreshold(threshold))
					this.SetLoggingThreshold("Error");

				this.SetAppenderTextWriter(this.Writer);
				this.ConfigureAppender();
			}
		}

		private void StopCapture()
		{
			if (this.writer != null)
				this.writer.Flush();

			if (this.appender != null)
			{
				this.SetLoggingThreshold("Off");
				//SetAppenderTextWriter( null );
			}
		}

		/// <summary>
		/// 	Attempt to create a TextWriterAppender using reflection,
		/// 	failing silently if it is not possible.
		/// </summary>
		private object TryCreateAppender()
		{
			ConstructorInfo ctor = this.appenderType.GetConstructor(Type.EmptyTypes);
			object appender = ctor.Invoke(new object[0]);

			return appender;
		}

		#endregion
	}
}