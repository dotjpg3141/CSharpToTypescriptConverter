using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpToTypescriptConverter.Test
{
	[TestClass]
	public class MemberProviderTest
	{
		[TestMethod]
		public void GetClassPublicPropertyTest()
		{
			var provider = new MemberProvider()
			{
				IncludeProperties = true,
				IncludeFields = false,
				IncludeInternal = false,
			};

			var member = provider.GetMembers(typeof(PropertyMemberClassMock)).Single();

			Assert.AreEqual(typeof(string), member.Type);
			Assert.AreEqual(nameof(PropertyMemberClassMock.PublicProperty), member.Name);
			Assert.IsNull(member.Value);
			Assert.AreEqual(MemberType.Property, member.MemberType);
		}

		[TestMethod]
		public void GetClassProtectedPropertyTest()
		{
			var provider = new MemberProvider()
			{
				IncludeProperties = true,
				IncludeFields = false,
				IncludeInternal = true,
			};

			var members = provider.GetMembers(typeof(PropertyMemberClassMock)).OrderBy(m => m.Name).ToList();
			Assert.AreEqual(2, members.Count);

			Assert.AreEqual(typeof(string), members[0].Type);
			Assert.AreEqual(nameof(PropertyMemberClassMock.InternalProperty), members[0].Name);
			Assert.IsNull(members[0].Value);
			Assert.AreEqual(MemberType.Property, members[0].MemberType);

			Assert.AreEqual(typeof(string), members[1].Type);
			Assert.AreEqual(nameof(PropertyMemberClassMock.PublicProperty), members[1].Name);
			Assert.IsNull(members[1].Value);
			Assert.AreEqual(MemberType.Property, members[1].MemberType);
		}

		[TestMethod]
		public void GetEnumMemberTest()
		{
			var provider = new MemberProvider();

			var member = provider.GetMembers(typeof(EnumMemberMock)).Single();

			Assert.IsNull(member.Type);
			Assert.AreEqual(nameof(EnumMemberMock.EnumMember), member.Name);
			Assert.AreEqual(77, member.Value);
			Assert.AreEqual(MemberType.EnumMember, member.MemberType);
		}

	}

	internal class PropertyMemberClassMock
	{
		public string PublicProperty { get; set; }
		internal string InternalProperty { get; set; }
		protected string ProtectedProperty { get; set; }
		private string PrivateProperty { get; set; }
	}

	internal enum EnumMemberMock
	{
		EnumMember = 77,
	}
}
