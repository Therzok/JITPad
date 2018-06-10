using System;
using System.Collections.Generic;

namespace JitPad
{
	public abstract class Jitter
	{
		public static Jitter Mono { get; } = new MonoJIT();
		public static Jitter MonoAOT { get; } = new MonoAOT();

		public IEnumerable<JitResult> Jit(CompileResult compileResult, params MethodDescription[] methodDescriptions)
		{
			foreach (var methodDesc in methodDescriptions) {
				var methodName = GenerateMethodName(methodDesc);
				yield return new JitResult(OnJit(compileResult, GenerateMethodName(methodDesc)));
			}
		}

		protected abstract string OnJit(CompileResult compileResult, string methodName);
		protected abstract string GenerateMethodName(MethodDescription description);
	}

	public class MethodDescription
	{
		public string FullTypeName { get; }
		public string MethodName { get; }
		public string[] ArgumentTypes { get; }

		public MethodDescription(string fullTypeName, string methodName, string[] argumentTypes)
		{
			FullTypeName = fullTypeName;
			MethodName = methodName;
			ArgumentTypes = argumentTypes;
		}
	}

	public class JitResult
	{
		public string Asm { get; }

		internal JitResult(string asm)
		{
			Asm = asm;
		}
	}
}
