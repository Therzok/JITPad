using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace JitPad
{
	public class RoslynCompiler : Compiler
	{
		protected override string OnCompile(string file)
		{
			var outFile = Path.ChangeExtension(file, "exe");

			var psi = new ProcessStartInfo("csc", $"\"{file}\" /fullpaths /out:\"{outFile}\"")
			{
				WorkingDirectory = Path.GetDirectoryName(outFile),
			};

			// Compile the actual file.
			Process.Start(psi).WaitForExit();

			return outFile;
		}
	}

	[TestFixture]
	public class RoslynCompilerTests
	{
		[Test]
		public void TestRoslynCompile()
		{
			var compiler = new RoslynCompiler();

			using (var result = compiler.Compile(CSharpTestData.ReadCharTest))
			{
				Assert.That(result.AssemblyFilePath, Is.StringEnding(".exe"));
				Assert.AreEqual(true, File.Exists(result.AssemblyFilePath));
			}
		}
	}
}
