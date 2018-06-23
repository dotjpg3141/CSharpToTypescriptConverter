using System;
using System.Linq;
using CSharpToTypescriptConverter.Reflection;
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
			Assert.AreEqual(MemberInfoType.Property, member.MemberInfoType);
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
			Assert.AreEqual(MemberInfoType.Property, members[0].MemberInfoType);

			Assert.AreEqual(typeof(string), members[1].Type);
			Assert.AreEqual(nameof(PropertyMemberClassMock.PublicProperty), members[1].Name);
			Assert.IsNull(members[1].Value);
			Assert.AreEqual(MemberInfoType.Property, members[1].MemberInfoType);
		}


		[TestMethod]
		public void GetClassPublicFieldTest()
		{
			var provider = new MemberProvider()
			{
				IncludeProperties = false,
				IncludeFields = true,
				IncludeInternal = false,
			};

			var member = provider.GetMembers(typeof(FieldMemberClassMock)).Single();

			Assert.AreEqual(typeof(string), member.Type);
			Assert.AreEqual(nameof(FieldMemberClassMock.PublicField), member.Name);
			Assert.IsNull(member.Value);
			Assert.AreEqual(MemberInfoType.Field, member.MemberInfoType);
		}

		[TestMethod]
		public void GetClassProtectedFieldTest()
		{
			var provider = new MemberProvider()
			{
				IncludeProperties = false,
				IncludeFields = true,
				IncludeInternal = true,
			};

			var members = provider.GetMembers(typeof(FieldMemberClassMock)).OrderBy(m => m.Name).ToList();
			Assert.AreEqual(2, members.Count);

			Assert.AreEqual(typeof(string), members[0].Type);
			Assert.AreEqual(nameof(FieldMemberClassMock.InternalField), members[0].Name);
			Assert.IsNull(members[0].Value);
			Assert.AreEqual(MemberInfoType.Field, members[0].MemberInfoType);

			Assert.AreEqual(typeof(string), members[1].Type);
			Assert.AreEqual(nameof(FieldMemberClassMock.PublicField), members[1].Name);
			Assert.IsNull(members[1].Value);
			Assert.AreEqual(MemberInfoType.Field, members[1].MemberInfoType);
		}

		[TestMethod]
		public void GetEnumMemberTest()
		{
			var provider = new MemberProvider();

			var member = provider.GetMembers(typeof(EnumMemberMock)).Single();

			Assert.IsNull(member.Type);
			Assert.AreEqual(nameof(EnumMemberMock.EnumMember), member.Name);
			Assert.AreEqual(77, member.Value);
			Assert.AreEqual(MemberInfoType.EnumMember, member.MemberInfoType);
		}

	}

	internal class PropertyMemberClassMock
	{
		public string PublicProperty { get; set; }
		internal string InternalProperty { get; set; }
		protected string ProtectedProperty { get; set; }
		private string PrivateProperty { get; set; }
	}

#pragma warning disable 649
#pragma warning disable 169
	internal class FieldMemberClassMock
	{
		public string PublicField;
		internal string InternalField;
		protected string ProtectedField;
		private string PrivateField;
	}
#pragma warning restore 169
#pragma warning restore 649

	internal enum EnumMemberMock
	{
		EnumMember = 77,
	}
}
