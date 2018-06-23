using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpToTypescriptConverter.Reflection;

namespace CSharpToTypescriptConverter
{
	class Program
	{
		static void Main(string[] args)
		{
			var typeProvider = new TypeProvider();
			var memberProvider = new MemberProvider();
			var types = typeProvider.GetTypes(memberProvider);
			Console.WriteLine(types);
		}
	}
}
