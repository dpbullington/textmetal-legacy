/*
	Copyright ©2002-2016 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Dnx.Compilation.CSharp;
using Microsoft.Extensions.CompilationAbstractions;
using Microsoft.Extensions.PlatformAbstractions;

using NUnitLite;

using TextMetal.Middleware.Solder.Runtime;

namespace TextMetal.Framework.IntegrationTests
{
	/// <summary>
	/// Entry point class for the application.
	/// </summary>
	internal class Program
	{
		#region Constructors/Destructors

		public Program()
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// The entry point method for this application.
		/// </summary>
		/// <param name="args"> The command line arguments passed from the executing environment. </param>
		/// <returns> The resulting exit code. </returns>
		[STAThread]
		public static int Main(string[] args)
		{
			return __Main(args);
			//return new AutoRun().Execute(typeof(Program).GetTypeInfo().Assembly, Console.Out, Console.In, args);
		}

		static int __Main(string[] args)
		{
			SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(@"using System;
                namespace RoslynCompileSample
                {
                    public class Writer
                    {
                        public void Write(string message)
                        {
                            Console.WriteLine(message);
                        }
                    }
                }");

			IList<MetadataReference> references = new List<MetadataReference>();

			var zz = PlatformServices.Default.LibraryManager.GetLibraries().SelectMany(x => x.Assemblies).Distinct().ToList();
			zz.ForEach(a => Console.WriteLine(a.FullName));

			if ((object)CompilationServices.Default == null)
				throw new Exception("CompilationServices.Default = null");

			if ((object)CompilationServices.Default.LibraryExporter == null)
				throw new Exception("CompilationServices.Default.LibraryExporter = null");

			var exports = CompilationServices.Default.LibraryExporter.GetAllExports(PlatformServices.Default.Application.ApplicationName);

			if ((object)exports != null)
			{
				foreach (var metadataReference in exports.MetadataReferences)
				{
					references.Add(ConvertMetadataReference(metadataReference));
				}
			}

			string assemblyName = Guid.NewGuid().ToString("N");

			CSharpCompilation compilation = CSharpCompilation.Create(
				assemblyName,
				syntaxTrees: new[] { syntaxTree },
				references: references,
				options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			using (var dllMs = new MemoryStream())
			{
				using (var pdbMs = new MemoryStream())
				{
					EmitResult result = compilation.Emit(dllMs, pdbMs);

					if (!result.Success)
					{
						IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
							diagnostic.IsWarningAsError ||
							diagnostic.Severity == DiagnosticSeverity.Error);

						foreach (Diagnostic diagnostic in failures)
						{
							Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
						}
					}
					else
					{
						dllMs.Seek(0, SeekOrigin.Begin);
						pdbMs.Seek(0, SeekOrigin.Begin);

						Assembly assembly = PlatformServices.Default.AssemblyLoadContextAccessor.Default.LoadStream(dllMs, pdbMs);
						Type type = assembly.GetType("RoslynCompileSample.Writer");
						object obj = Activator.CreateInstance(type);
						MethodInfo method = type.GetMethod("Write", BindingFlags.Public | BindingFlags.Instance);
						method.Invoke(obj, new object[] { "Hello World" });
					}
				}
			}

			return 0;
		}

		private static MetadataReference ConvertMetadataReference(
			IMetadataReference metadataReference)
		{
			var roslynReference = metadataReference as IRoslynMetadataReference;

			if (roslynReference != null)
			{
				return roslynReference.MetadataReference;
			}

			var embeddedReference = metadataReference as IMetadataEmbeddedReference;

			if (embeddedReference != null)
			{
				return MetadataReference.CreateFromImage(embeddedReference.Contents);
			}

			var fileMetadataReference = metadataReference as IMetadataFileReference;

			if (fileMetadataReference != null)
			{
				return CreateMetadataFileReference(fileMetadataReference.Path);
			}

			var projectReference = metadataReference as IMetadataProjectReference;
			if (projectReference != null)
			{
				using (var ms = new MemoryStream())
				{
					projectReference.EmitReferenceAssembly(ms);

					return MetadataReference.CreateFromImage(ms.ToArray());
				}
			}

			throw new NotSupportedException();
		}

		private static MetadataReference CreateMetadataFileReference(string path)
		{
			using (var stream = File.OpenRead(path))
			{
				var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
				var metadata = AssemblyMetadata.Create(moduleMetadata);
				return metadata.GetReference(filePath: path);
			}
		}

		#endregion
	}
}