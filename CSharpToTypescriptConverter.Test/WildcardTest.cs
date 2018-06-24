using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpToTypescriptConverter.Test
{
	[TestClass]
	public class WildcardTest
	{
		[TestMethod]
		public void EmptyWildCardTest()
		{
			var wildcard = "";
			var result = Wildcard.ToRegexString(wildcard);

			Assert.AreEqual(@"\A\Z", result);
			AssertWildcard(wildcard, new[] { "" }, new[] { "A", "123" });
		}

		[TestMethod]
		public void MatchAllWildCardTest()
		{
			var wildcard = "*";
			var result = Wildcard.ToRegexString(wildcard);

			Assert.AreEqual(@"\A.*?\Z", result);
			AssertWildcard(wildcard, new[] { "", "A", "123" }, new string[0]);
		}

		[TestMethod]
		public void MatchStartTest()
		{
			var wildcard = "123*";
			var result = Wildcard.ToRegexString(wildcard);

			Assert.AreEqual(@"\A123.*?\Z", result);
			AssertWildcard(wildcard, new[] { "123", "1234" }, new[] { "12", "A123", "GG" });
		}

		[TestMethod]
		public void MatchEndTest()
		{
			var wildcard = "*123";
			var result = Wildcard.ToRegexString(wildcard);

			Assert.AreEqual(@"\A.*?123\Z", result);
			AssertWildcard(wildcard, new[] { "123", "0123" }, new[] { "1234", "123A", "GG" });
		}

		[TestMethod]
		public void MatchContainsTest()
		{
			var wildcard = "*abc*";
			var result = Wildcard.ToRegexString(wildcard);

			Assert.AreEqual(@"\A.*?abc.*?\Z", result);
			AssertWildcard(wildcard, new[] { "abc", "0abc", "abc0", "0abc0" }, new[] { "a0b0c", "123A", "GG" });
		}

		private static void AssertWildcard(string wildcard, string[] expectMatch, string[] expectNoMatch)
		{
			var regex = Wildcard.ToRegex(wildcard);
			foreach (var match in expectMatch)
			{
				Assert.IsTrue(regex.IsMatch(match), match);
			}
			foreach (var noMatch in expectNoMatch)
			{
				Assert.IsFalse(regex.IsMatch(noMatch), noMatch);
			}
		}
	}
}
