// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Core;

namespace NUnit.Util
{
	using System;

	/// <summary>
	/// Summary description for XmlResultWriter.
	/// </summary>
	public class XmlResultWriter
	{
		#region Constructors/Destructors

		public XmlResultWriter(string fileName)
		{
			this.xmlWriter = new XmlTextWriter(new StreamWriter(fileName, false, Encoding.UTF8));
		}

		public XmlResultWriter(TextWriter writer)
		{
			this.memoryStream = new MemoryStream();
			this.writer = writer;
			this.xmlWriter = new XmlTextWriter(new StreamWriter(this.memoryStream, Encoding.UTF8));
		}

		#endregion

		#region Fields/Constants

		private MemoryStream memoryStream;
		private TextWriter writer;
		private XmlTextWriter xmlWriter;

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Makes string safe for xml parsing, replacing control chars with '?'
		/// </summary>
		/// <param name="encodedString"> string to make safe </param>
		/// <returns> xml safe string </returns>
		private static string CharacterSafeString(string encodedString)
		{
			/*The default code page for the system will be used.
			Since all code pages use the same lower 128 bytes, this should be sufficient
			for finding uprintable control characters that make the xslt processor error.
			We use characters encoded by the default code page to avoid mistaking bytes as
			individual characters on non-latin code pages.*/
			char[] encodedChars = Encoding.Default.GetChars(Encoding.Default.GetBytes(encodedString));

			ArrayList pos = new ArrayList();
			for (int x = 0; x < encodedChars.Length; x++)
			{
				char currentChar = encodedChars[x];
				//unprintable characters are below 0x20 in Unicode tables
				//some control characters are acceptable. (carriage return 0x0D, line feed 0x0A, horizontal tab 0x09)
				if (currentChar < 32 && (currentChar != 9 && currentChar != 10 && currentChar != 13))
				{
					//save the array index for later replacement.
					pos.Add(x);
				}
			}
			foreach (int index in pos)
				encodedChars[index] = '?'; //replace unprintable control characters with ?(3F)
			return Encoding.Default.GetString(Encoding.Default.GetBytes(encodedChars));
		}

		private void InitializeXmlFile(TestResult result)
		{
			ResultSummarizer summaryResults = new ResultSummarizer(result);

			this.xmlWriter.Formatting = Formatting.Indented;
			this.xmlWriter.WriteStartDocument(false);
			this.xmlWriter.WriteComment("This file represents the results of running a test suite");

			this.xmlWriter.WriteStartElement("test-results");

			this.xmlWriter.WriteAttributeString("name", summaryResults.Name);
			this.xmlWriter.WriteAttributeString("total", summaryResults.TestsRun.ToString());
			this.xmlWriter.WriteAttributeString("errors", summaryResults.Errors.ToString());
			this.xmlWriter.WriteAttributeString("failures", summaryResults.Failures.ToString());
			this.xmlWriter.WriteAttributeString("not-run", summaryResults.TestsNotRun.ToString());
			this.xmlWriter.WriteAttributeString("inconclusive", summaryResults.Inconclusive.ToString());
			this.xmlWriter.WriteAttributeString("ignored", summaryResults.Ignored.ToString());
			this.xmlWriter.WriteAttributeString("skipped", summaryResults.Skipped.ToString());
			this.xmlWriter.WriteAttributeString("invalid", summaryResults.NotRunnable.ToString());

			DateTime now = DateTime.Now;
			this.xmlWriter.WriteAttributeString("date", XmlConvert.ToString(now, "yyyy-MM-dd"));
			this.xmlWriter.WriteAttributeString("time", XmlConvert.ToString(now, "HH:mm:ss"));
			this.WriteEnvironment();
			this.WriteCultureInfo();
		}

		public void SaveTestResult(TestResult result)
		{
			this.InitializeXmlFile(result);
			this.WriteResultElement(result);
			this.TerminateXmlFile();
		}

		private void StartTestElement(TestResult result)
		{
			if (result.Test.IsSuite)
			{
				this.xmlWriter.WriteStartElement("test-suite");
				this.xmlWriter.WriteAttributeString("type", result.Test.TestType);
				this.xmlWriter.WriteAttributeString("name", result.Name);
			}
			else
			{
				this.xmlWriter.WriteStartElement("test-case");
				this.xmlWriter.WriteAttributeString("name", result.FullName);
			}

			if (result.Description != null)
				this.xmlWriter.WriteAttributeString("description", result.Description);

			this.xmlWriter.WriteAttributeString("executed", result.Executed.ToString());
			this.xmlWriter.WriteAttributeString("result", result.ResultState.ToString());

			if (result.Executed)
			{
				this.xmlWriter.WriteAttributeString("success", result.IsSuccess.ToString());
				this.xmlWriter.WriteAttributeString("time", result.Time.ToString("#####0.000", NumberFormatInfo.InvariantInfo));
				this.xmlWriter.WriteAttributeString("asserts", result.AssertCount.ToString());
			}
		}

		private void TerminateXmlFile()
		{
			try
			{
				this.xmlWriter.WriteEndElement(); // test-results
				this.xmlWriter.WriteEndDocument();
				this.xmlWriter.Flush();

				if (this.memoryStream != null && this.writer != null)
				{
					this.memoryStream.Position = 0;
					using (StreamReader rdr = new StreamReader(this.memoryStream))
						this.writer.Write(rdr.ReadToEnd());
				}

				this.xmlWriter.Close();
			}
			finally
			{
				//writer.Close();
			}
		}

		private void WriteCData(string text)
		{
			int start = 0;
			while (true)
			{
				int illegal = text.IndexOf("]]>", start);
				if (illegal < 0)
					break;
				this.xmlWriter.WriteCData(text.Substring(start, illegal - start + 2));
				start = illegal + 2;
				if (start >= text.Length)
					return;
			}

			if (start > 0)
				this.xmlWriter.WriteCData(text.Substring(start));
			else
				this.xmlWriter.WriteCData(text);
		}

		private void WriteCategoriesElement(TestResult result)
		{
			if (result.Test.Categories != null && result.Test.Categories.Count > 0)
			{
				this.xmlWriter.WriteStartElement("categories");
				foreach (string category in result.Test.Categories)
				{
					this.xmlWriter.WriteStartElement("category");
					this.xmlWriter.WriteAttributeString("name", category);
					this.xmlWriter.WriteEndElement();
				}
				this.xmlWriter.WriteEndElement();
			}
		}

		private void WriteChildResults(TestResult result)
		{
			this.xmlWriter.WriteStartElement("results");

			if (result.HasResults)
			{
				foreach (TestResult childResult in result.Results)
					this.WriteResultElement(childResult);
			}

			this.xmlWriter.WriteEndElement();
		}

		private void WriteCultureInfo()
		{
			this.xmlWriter.WriteStartElement("culture-info");
			this.xmlWriter.WriteAttributeString("current-culture",
				CultureInfo.CurrentCulture.ToString());
			this.xmlWriter.WriteAttributeString("current-uiculture",
				CultureInfo.CurrentUICulture.ToString());
			this.xmlWriter.WriteEndElement();
		}

		private void WriteEnvironment()
		{
			this.xmlWriter.WriteStartElement("environment");
			this.xmlWriter.WriteAttributeString("nunit-version",
				Assembly.GetExecutingAssembly().GetName().Version.ToString());
			this.xmlWriter.WriteAttributeString("clr-version",
				Environment.Version.ToString());
			this.xmlWriter.WriteAttributeString("os-version",
				Environment.OSVersion.ToString());
			this.xmlWriter.WriteAttributeString("platform",
				Environment.OSVersion.Platform.ToString());
			this.xmlWriter.WriteAttributeString("cwd",
				Environment.CurrentDirectory);
			this.xmlWriter.WriteAttributeString("machine-name",
				Environment.MachineName);
			this.xmlWriter.WriteAttributeString("user",
				Environment.UserName);
			this.xmlWriter.WriteAttributeString("user-domain",
				Environment.UserDomainName);
			this.xmlWriter.WriteEndElement();
		}

		private void WriteFailureElement(TestResult result)
		{
			this.xmlWriter.WriteStartElement("failure");

			this.xmlWriter.WriteStartElement("message");
			this.WriteCData(result.Message);
			this.xmlWriter.WriteEndElement();

			this.xmlWriter.WriteStartElement("stack-trace");
			if (result.StackTrace != null)
				this.WriteCData(StackTraceFilter.Filter(result.StackTrace));
			this.xmlWriter.WriteEndElement();

			this.xmlWriter.WriteEndElement();
		}

		private void WritePropertiesElement(TestResult result)
		{
			IDictionary props = result.Test.Properties;

			if (result.Test.Properties != null && props.Count > 0)
			{
				int nprops = 0;

				foreach (string key in result.Test.Properties.Keys)
				{
					if (!key.StartsWith("_"))
					{
						object val = result.Test.Properties[key];
						if (val != null)
						{
							if (nprops == 0)
								this.xmlWriter.WriteStartElement("properties");

							this.xmlWriter.WriteStartElement("property");
							this.xmlWriter.WriteAttributeString("name", key);
							this.xmlWriter.WriteAttributeString("value", val.ToString());
							this.xmlWriter.WriteEndElement();

							++nprops;
						}
					}
				}

				if (nprops > 0)
					this.xmlWriter.WriteEndElement();
			}
		}

		private void WriteReasonElement(TestResult result)
		{
			this.xmlWriter.WriteStartElement("reason");
			this.xmlWriter.WriteStartElement("message");
			this.xmlWriter.WriteCData(result.Message);
			this.xmlWriter.WriteEndElement();
			this.xmlWriter.WriteEndElement();
		}

		private void WriteResultElement(TestResult result)
		{
			this.StartTestElement(result);

			this.WriteCategoriesElement(result);
			this.WritePropertiesElement(result);

			switch (result.ResultState)
			{
				case ResultState.Ignored:
				case ResultState.NotRunnable:
				case ResultState.Skipped:
					this.WriteReasonElement(result);
					break;

				case ResultState.Failure:
				case ResultState.Error:
				case ResultState.Cancelled:
					if (!result.Test.IsSuite || result.FailureSite == FailureSite.SetUp)
						this.WriteFailureElement(result);
					break;
				case ResultState.Success:
				case ResultState.Inconclusive:
					if (result.Message != null)
						this.WriteReasonElement(result);
					break;
			}

			if (result.HasResults)
				this.WriteChildResults(result);

			this.xmlWriter.WriteEndElement(); // test element
		}

		#endregion
	}
}