using System;
using System.Diagnostics;
using System.IO;

namespace JitPad
{
	public class MonoJIT : Jitter
	{
		protected override string GenerateMethodName(MethodDescription description) => description.GetMonoJitDescription();

		protected override string OnJit(CompileResult compileResult, string methodName)
		{
			var psi = new ProcessStartInfo("mono", $"--compile '{methodName}' \"{compileResult.AssemblyFilePath}\"") {
				RedirectStandardOutput = true,
				UseShellExecute = false,
			};
			psi.EnvironmentVariables.Add("MONO_VERBOSE_METHOD", methodName);

			// Run the JIT on the assembly.
			var proc = Process.Start(psi);
			var jit = proc.StandardOutput.ReadToEnd();

			proc.WaitForExit();

			var lines = jit.Split('\n');
			int jitAfter = Array.IndexOf(lines, "(__TEXT,__text) section");
			jitAfter += 2;

			return string.Join(Environment.NewLine, lines, jitAfter, lines.Length - jitAfter);
		}
	}
}
