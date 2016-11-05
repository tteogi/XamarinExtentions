using System.Collections.Generic;

namespace XamarinUncrustify
{
	public class Config
	{
		public class ProjectOption
		{
			public string Name;
			public bool IsRunOnSave = false;
			public string CommandConfigPath = "";
		}
		public int Port;
		public List<ProjectOption> ProjectOptions = new List<ProjectOption>();
	}
}

