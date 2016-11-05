using System;
using System.Collections.Generic;
namespace XamarinUncrustify
{
	public class App
	{
		static Dictionary<string, CommandProperty> _properties = new Dictionary<string, CommandProperty>();

		public static Dictionary<string, CommandProperty> Properties
		{
			get { return _properties; }
		}

		static CommandExecuter _commandOnSaveDocumentExecuter = new CommandExecuter();
	}
}
