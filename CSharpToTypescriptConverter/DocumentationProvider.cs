using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CSharpToTypescriptConverter.Reflection;

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

		public bool TryGetDocumentation(TypeInfo type, out DocumentationInfo documentation)
			=> TryGetDocumentation('T', type.FullName, out documentation);

		internal bool TryGetDocumentation(TypeInfo type, MemberInfo member, out DocumentationInfo documentation)
		{
			var fullName = type.FullName + "." + member.Name;

			switch (member.MemberInfoType)
			{
				case MemberInfoType.EnumMember:
				case MemberInfoType.Field:
					return TryGetDocumentation('F', fullName, out documentation);
				case MemberInfoType.Property:
					return TryGetDocumentation('P', fullName, out documentation);
				default:
					throw new InvalidOperationException(member.MemberInfoType.ToString());
			}
		}

		internal bool TryGetDocumentation(char type, string fullName, out DocumentationInfo documentation)
		{
			var identifier = $"{type}:{fullName}";
			return this.loadedMembers.TryGetValue(identifier, out documentation);
		}
	}
}
