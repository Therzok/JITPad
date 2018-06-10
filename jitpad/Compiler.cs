using System;
using System.IO;

namespace JitPad
{
	public abstract class Compiler
	{
		// Should work with simple text.
		public CompileResult Compile (string text)
		{
			var sourceFile = Path.GetTempFileName();

			sourceFile = Path.GetFullPath(sourceFile);

			// TODO: Add a smarter mechanism
			File.WriteAllText(sourceFile, text);

			var assemblyFile = OnCompile(sourceFile);

			File.Delete(sourceFile);
			return new CompileResult(assemblyFile);
		}

		protected abstract string OnCompile(string file);
	}

	public class CompileResult : IDisposable
	{
		public string AssemblyFilePath { get; }

		public CompileResult (string assemblyFilePath)
		{
			AssemblyFilePath = assemblyFilePath;
		}

		public void Dispose()
		{
			try
			{
				File.Delete(AssemblyFilePath);
			} catch (Exception e) {
				// TODO: Add logger.
				Console.WriteLine(e);
			}
		}
	}
}
