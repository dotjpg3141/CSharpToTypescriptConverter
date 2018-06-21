using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpToTypescriptConverter
{
	public class DocumentationInfo
	{
		private static readonly Regex WhiteSpace = new Regex(@"\s+");

		public string Summary { get; set; }

		public static DocumentationInfo Parse(IEnumerable<XElement> elements)
		{
			var result = new DocumentationInfo();
			if (elements.FirstOrDefault(element => element.Name == "summary") is XElement summary)
			{
				result.Summary = NormalizeText(summary.Value);
			}

			return result;
		}

		private static string NormalizeText(string text)
		{
			return WhiteSpace.Replace(text, " ").Trim();
		}
	}
}
