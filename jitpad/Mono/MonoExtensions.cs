using System;
using System.Linq;
using System.Text;

namespace JitPad
{
	static class MonoExtensions
	{
		public static string GetMonoJitDescription (this MethodDescription description)
		{
			return $"{description.FullTypeName}:{description.MethodName}{description.ArgumentTypes.GetSuffixJitMetadataName()}";
		}

		static string GetSuffixJitMetadataName(this string[] parameterTypeList)
		{
			// Mono bug: https://github.com/mono/mono/pull/9087/files
			const bool bugFixed = false;

			if (!bugFixed || parameterTypeList.Length == 0)
				return string.Empty;

			return "(" + string.Join(",", parameterTypeList) + ")";
		}

		public static string GetMonoAotDescription (this MethodDescription description)
		{
			return $"{description.FullTypeName.Replace('.', '_')}_{description.MethodName}{description.ArgumentTypes.GetSuffixAotMetadataName()}";
		}

		static string GetSuffixAotMetadataName(this string[] parameterTypeList)
		{
			if (parameterTypeList.Length == 0)
				return string.Empty;

			var sb = new StringBuilder();
			parameterTypeList = parameterTypeList.Select(x =>
			{
				sb.Clear();
				sb.Append(x);
				sb.Replace('[', '_');
				sb.Replace(']', '_');

				return sb.ToString();
			}).ToArray ();

			return "_" + string.Join("_", parameterTypeList);
		}
	}
}
