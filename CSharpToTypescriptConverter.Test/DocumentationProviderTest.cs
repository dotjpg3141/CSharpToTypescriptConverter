using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CSharpToTypescriptConverter.Test.FileHelper;

namespace CSharpToTypescriptConverter.Test
{
	[TestClass]
	public class DocumentationProviderTest
	{
		[TestMethod]
		public void LoadTypeTest()
		{
			var provider = new DocumentationProvider();
			provider.LoadFile(GetProjectFile(MockNamespace, "xml"));

			Assert.IsTrue(provider.TryGetTypeDocumentation(MockNamespace + ".PublicClass", out var documentation));
			Assert.AreEqual("Class Documentation", documentation.Summary);
		}
	}
}
