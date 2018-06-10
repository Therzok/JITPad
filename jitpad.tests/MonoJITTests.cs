using System;
using NUnit.Framework;

namespace JitPad
{
	[TestFixture]
	public class MonoJitTests : JitterTestsBase
	{
		const string expectedMonoJitReadChar =  @"0000000000000000	pushq	%rbp
0000000000000001	movq	%rsp, %rbp
0000000000000004	subq	$0x10, %rsp
0000000000000008	movq	%r15, -0x8(%rbp)
000000000000000c	movq	%rdi, %r15
000000000000000f	movslq	0x10(%r15), %rax
0000000000000013	incl	%eax
0000000000000015	movl	%eax, 0x10(%r15)
0000000000000019	movq	-0x8(%rbp), %r15
000000000000001d	leave
000000000000001e	retq
";
		const string expectedMonoJitReadCharInt = @"0000000000000000	pushq	%rbp
0000000000000001	movq	%rsp, %rbp
0000000000000004	subq	$0x10, %rsp
0000000000000008	movq	%r14, -0x8(%rbp)
000000000000000c	movq	%rdi, %r14
000000000000000f	movq	%rsi, -0x10(%rbp)
0000000000000013	movslq	0x10(%r14), %rax
0000000000000017	addl	-0x10(%rbp), %eax
000000000000001a	movl	%eax, 0x10(%r14)
000000000000001e	movq	-0x8(%rbp), %r14
0000000000000022	leave
0000000000000023	retq
";
		const string expectedMonoAotReadChar = @"     b60:	55 	pushq	%rbp
     b61:	48 8b ec 	movq	%rsp, %rbp
     b64:	48 83 ec 10 	subq	$16, %rsp
     b68:	48 89 7d f8 	movq	%rdi, -8(%rbp)
     b6c:	48 8b c7 	movq	%rdi, %rax
     b6f:	48 63 48 10 	movslq	16(%rax), %rcx
     b73:	ff c1 	incl	%ecx
     b75:	89 48 10 	movl	%ecx, 16(%rax)
     b78:	c9 	leave
     b79:	c3 	retq
     b7a:	66 0f 1f 44 00 00 	nopw	(%rax,%rax)
";

		const string expectedMonoAotReadCharInt = @"     b80:	55 	pushq	%rbp
     b81:	48 8b ec 	movq	%rsp, %rbp
     b84:	48 83 ec 10 	subq	$16, %rsp
     b88:	48 89 7d f8 	movq	%rdi, -8(%rbp)
     b8c:	48 89 75 f0 	movq	%rsi, -16(%rbp)
     b90:	48 8b c7 	movq	%rdi, %rax
     b93:	48 63 48 10 	movslq	16(%rax), %rcx
     b97:	03 4d f0 	addl	-16(%rbp), %ecx
     b9a:	89 48 10 	movl	%ecx, 16(%rax)
     b9d:	c9 	leave
     b9e:	c3 	retq
     b9f:	90 	nop
";

		[TestCase("MainClass", "ReadChar", null, expectedMonoJitReadChar)]
		[TestCase("MainClass", "ReadChar", "int", expectedMonoJitReadCharInt)]
		public void TestSimpleJit(string className, string methodName, string argName, string expected)
		{
			Test(Jitter.Mono, CSharpTestData.ReadCharTest, new MethodDescription(className, methodName, argName != null ? new string[] { argName } : Array.Empty<string> ()), expected);
		}

		[TestCase("MainClass", "ReadChar", null, expectedMonoAotReadChar)]
		[TestCase("MainClass", "ReadChar", "int", expectedMonoAotReadCharInt)]
		public void TestSimpleAot(string className, string methodName, string argName, string expected)
		{
			Test(Jitter.MonoAOT, CSharpTestData.ReadCharTest, new MethodDescription(className, methodName, argName != null ? new string[] { argName } : Array.Empty<string>()), expected);
		}
	}
}
