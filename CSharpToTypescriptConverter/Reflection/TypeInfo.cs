﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypescriptConverter.Reflection
{
	[Serializable]
	public class TypeInfo
	{
		public string Namespace { get; set; }
		public string Name { get; set; }
		public TypeInfoType Type { get; set; }
		public IEnumerable<MemberInfo> Members { get; set; }

		public TypeInfo(Type type)
		{
			this.Namespace = type.Namespace;
			this.Name = type.Name;
			this.Type = TypeInfoType.Class; // TODO(jpg)
		}
	}
}