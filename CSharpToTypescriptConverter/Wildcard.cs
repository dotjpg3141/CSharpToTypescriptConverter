using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpToTypescriptConverter
{
	public class Wildcard
	{
		private static readonly Regex WildcardMatcher = new Regex(@"(?=\*)|(?<=\*)");

		public static Regex ToRegex(string wildcard, RegexOptions options = RegexOptions.None)
		{
			return new Regex(ToRegexString(wildcard), options);
		}

		public static string ToRegexString(string wildcard)
		{
			var items = WildcardMatcher.Split(wildcard).Where(str => str != "").ToArray();
			var sb = new StringBuilder();
			sb.Append(@"\A");
			foreach (var part in items)
			{
				if (part == "*")
				{
					sb.Append(".*?");
				}
				else
				{
					sb.Append(Regex.Escape(part));
				}
			}
			sb.Append(@"\Z");
			return sb.ToString();
		}
	}
}
