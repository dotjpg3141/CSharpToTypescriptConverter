using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpToTypescriptConverter
{
	public class DocumentationProvider
	{
		private readonly Dictionary<string, DocumentationInfo> loadedMembers = new Dictionary<string, DocumentationInfo>();

		public void LoadFile(string file)
		{
			var root = XDocument.Load(file).Root ?? throw new InvalidOperationException($"Cannot load document '{file}'");

			var newMembers = root
				.Elements("members")
				.Elements("member")
				.Where(element => element.Attribute("name") != null)
				.ToDictionary(
					element => element.Attribute("name")?.Value,
					element => DocumentationInfo.Parse(element.Descendants()));

			foreach (var kvp in newMembers)
			{
				this.loadedMembers.Add(kvp.Key, kvp.Value);
			}
		}

		public bool TryGetDocumentation(Type type, out DocumentationInfo documentation)
			=> TryGetTypeDocumentation(type.FullName, out documentation);

		internal bool TryGetTypeDocumentation(string fullTypeName, out DocumentationInfo documentation)
		{
			var identifier = "T:" + fullTypeName;
			return this.loadedMembers.TryGetValue(identifier, out documentation);
		}
	}
}
