#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypescriptConverter.TestMock
{
	public class GenericClass<T>
	{
		public T Foo { get; set; }
		public int? NullableInt { get; set; }
		public KeyValuePair<string, int> KeyValuePair { get; set; }
	}
}
