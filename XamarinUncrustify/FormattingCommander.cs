using System;
using MonoDevelop.Ide;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Core.Logging;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace XamarinUncrustify
{
	public enum GistCommands
	{
		GistIdeInformation,
		CopyGistIdeInformation
	}

	public class FormattingCommander : CommandHandler
	{
		protected override void Run()
		{
			CommandProperty.Command cmd = new CommandProperty.Command();
			cmd.Cmd = "../../Middleware/uncrustify/uncrustify";
			cmd.Argument = "-c ${workspace}/../../Middleware/uncrustify/cfg/uncrustify.cfg --replace --no-backup ${file}";

			cmd.Cmd = ReplacePlaceholder(cmd.Cmd);
			cmd.Argument = ReplacePlaceholder(cmd.Argument);

			string absoluteCmdPatch = cmd.Cmd;//Path.GetFullPath(cmd.Cmd);
			if (GCore.Path.IsAbsolutePatch(absoluteCmdPatch))
			{
				absoluteCmdPatch = Path.Combine(IdeApp.ProjectOperations.CurrentSelectedProject.BaseDirectory,
				                                absoluteCmdPatch);
				absoluteCmdPatch = Path.GetFullPath(absoluteCmdPatch);
			}

			var process = Process.Start(absoluteCmdPatch, cmd.Argument);
			process.WaitForExit();
			IdeApp.Workbench.ActiveDocument.Reload();
			//var directory = IdeApp.ProjectOperations.CurrentSelectedProject.BaseDirectory;
			//var ssss = IdeApp.ProjectOperations.CurrentSelectedProject.ProjectProperties.GetValue("UncrustifyConfig", "akf");
			//var pro = IdeApp.ProjectOperations.CurrentSelectedSolution.ExtendedProperties;
			//pro.Add("fsfsdf", "fsfdsf");
			//IdeApp.ProjectOperations.CurrentSelectedSolution.UserProperties.SetValue("TestValue", "valusess");
			//IdeApp.ProjectOperations.CurrentSelectedSolution.SaveAsync(new ProgressMonitor());

			//IdeApp.ProjectOperations.CurrentSelectedProject.ProjectProperties.SetValue("UncrustifyConfig", "dasklfjsadklfjsd;lakf");
			//IdeApp.ProjectOperations.CurrentSelectedProject.SaveAsync(new ProgressMonitor());
			//Console.WriteLine(directory);;
		}

		private string ReplacePlaceholder(string str)
		{
			var outStr = str;
			outStr = Regex.Replace(outStr, @"\${file}", Path.GetFullPath(IdeApp.Workbench.ActiveDocument.FileName));
			outStr = Regex.Replace(outStr, @"\${workspace}", Path.GetFullPath(IdeApp.ProjectOperations.CurrentSelectedProject.BaseDirectory));
			return outStr;
		}

		protected override void Update(CommandInfo info)
		{
			Console.WriteLine("Update");
		}
	}
}
