using System;
namespace XamarinUncrustify
{
	public class App
	{
		static CommandProperty _property = new CommandProperty();

		public static CommandProperty Property
		{
			get { return _property; }
		}
	}
}
