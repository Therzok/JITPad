using NUnit.Framework;
using System;
using System.IO;

namespace JitPad
{
	[TestFixture]
	public class CompilerTests
	{
		class MockCompiler : Compiler
		{
			protected override string OnCompile(string file)
			{
				Compiled?.Invoke(this, file);

				var compiledFile = Path.GetTempFileName();
				File.WriteAllText(compiledFile, nameof(MockCompiler));

				return compiledFile;
			}

			public event EventHandler<string> Compiled;
		}

		[Test]
		public void TestCompilerCompile()
		{
			var compiler = new MockCompiler();
			compiler.Compiled += (o, args) => Assert.AreEqual("test", File.ReadAllText (args));

			string assemblyFilePath;
			using (var result = compiler.Compile("test"))
			{
				assemblyFilePath = result.AssemblyFilePath;

				Assert.IsNotNullOrEmpty(assemblyFilePath);
				Assert.AreEqual(nameof(MockCompiler), File.ReadAllText(assemblyFilePath));
			}

			Assert.AreEqual(false, File.Exists(assemblyFilePath));
		}
	}
}
