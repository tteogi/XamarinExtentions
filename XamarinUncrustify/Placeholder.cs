using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace XamarinUncrustify
{
	public class Placeholder
	{
		private Dictionary<string, string> _replaceParttens;

		public Placeholder(Dictionary<string, string> replaceParttens)
		{
			_replaceParttens = replaceParttens;
		}

		public string ReplacePlaceholder(string str)
		{
			var outStr = str;
			foreach (var patten in _replaceParttens)
			{
				outStr = Regex.Replace(outStr, patten.Key, patten.Value);
			}

			return outStr;
		}
	}

}
