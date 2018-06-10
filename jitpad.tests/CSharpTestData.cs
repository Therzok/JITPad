using System;
namespace JitPad
{
	public static class CSharpTestData
	{
		public const string ReadCharTest = @"
class MainClass
{
	int i;
	public void ReadChar() => ++i;
	public void ReadChar(int x) => i += x;
	public void GetCounts(int[] x) {}

	public static void Main() {}
}";
	}
}
