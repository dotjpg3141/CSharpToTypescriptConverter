using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpToTypescriptConverter
{
	public class IndentWriter
	{
		private static Regex NewLine = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);

		public TextWriter InnerWriter { get; }
		public string IdentStringPerLevel { get; set; } = "\t";
		public int Level { get; set; }
		public string IndentString
		{
			get
			{
				var sb = new StringBuilder();
				for (int i = 0; i < this.Level; i++)
				{
					sb.Append(this.IdentStringPerLevel);
				}
				return sb.ToString();
			}
		}

		public IndentWriter(TextWriter innerWriter)
		{
			Debug.Assert(innerWriter != null);
			this.InnerWriter = innerWriter;
		}

		public void BeginLine()
		{
			this.InnerWriter.Write(this.IndentString);
		}

		public void Write(string text)
		{
			if (text != null)
			{
				var lines = NewLine.Split(text);
				var indent = this.IndentString;

				for (int i = 0; i < lines.Length; i++)
				{
					if (i != 0)
					{
						this.InnerWriter.WriteLine();
					}
					if (i != lines.Length - 1)
					{
						this.InnerWriter.Write(indent);
					}
					this.InnerWriter.Write(lines[i]);
				}
			}
		}

		public void Write(object value)
		{
			this.Write(value?.ToString());
		}

		public void EndLine()
		{
			this.InnerWriter.WriteLine();
		}

		public void WriteLine(string text)
		{
			this.BeginLine();
			this.Write(text);
			this.EndLine();
		}

		public void WriteLine(object value)
		{
			this.WriteLine(value?.ToString());
		}

		internal void WriteLine()
		{
			this.InnerWriter.WriteLine();
		}

		public void Indent()
		{
			this.Level++;
		}

		public void Unindent()
		{
			this.Level--;
		}


	}
}
