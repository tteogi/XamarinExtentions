using System;
using System.Diagnostics;
using MonoDevelop.Ide;

namespace XamarinUncrustify
{
	public class ProcessExecuteOnSave
	{
		public ProcessExecuteOnSave()
		{
			IdeApp.Workbench.ActiveDocument.Saved += OnSaved;
		}

		private void OnSaved(object sender, EventArgs e)
		{
			//IdeApp.ProjectOperations.CurrentSelectedObject
			      
			//var process = new Process();
			//if (Environment.OSVersion.Platform == PlatformID.Unix)
			//{
			//}
			//process.Start();
		}
	}
}
