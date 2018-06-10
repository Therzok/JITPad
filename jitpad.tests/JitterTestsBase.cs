using System;
using NUnit.Framework;

namespace JitPad
{
	public class JitterTestsBase
	{
		protected void Test(Jitter jitter, string testCode, MethodDescription description, string expectedAsm)
		{
			var compiler = new RoslynCompiler();
			using (var result = compiler.Compile(testCode))
			{
				foreach (var jitResult in jitter.Jit(result, description))
				{
					Console.WriteLine(jitResult.Asm);
					Assert.AreEqual(expectedAsm, jitResult.Asm);
				}
			}
		}
	}
}
