using System;
using System.Diagnostics;

namespace JitPad
{
	public class MonoAOT : Jitter
	{
		protected override string GenerateMethodName(MethodDescription description) => description.GetMonoAotDescription();

		protected override string OnJit(CompileResult compileResult, string methodName)
		{
			var psi = new ProcessStartInfo("mono", $"--aot \"{compileResult.AssemblyFilePath}\"");

			// Run the JIT on the assembly.
			var proc = Process.Start(psi);

			proc.WaitForExit();

			psi = new ProcessStartInfo("objdump", $"-disassemble -dis-symname={methodName} -macho {compileResult.AssemblyFilePath}.dylib") {
				RedirectStandardOutput = true,
				UseShellExecute = false,
			};

			proc = Process.Start(psi);
			var jit = proc.StandardOutput.ReadToEnd();

			proc.WaitForExit();

			var lines = jit.Split('\n');
			int jitAfter = Array.IndexOf(lines, "(__TEXT,__text) section");
			jitAfter += 2;

			return string.Join(Environment.NewLine, lines, jitAfter, lines.Length - jitAfter);
		}
	}
}
