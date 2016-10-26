using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace XamarinUncrustify
{
	public class CommandExecuter
	{
		private string _workspace;
		public CommandExecuter(string workspace = "")
		{
			_workspace = workspace;
		}

		public void Execute(CommandProperty.Command command, Placeholder placeholder = null)
		{
			Execute(command, _workspace, placeholder);
		}

		public static void Execute(CommandProperty.Command command, string workspaceDirectory, Placeholder placeholder = null)
		{
			string cmd = command.Cmd;
			string arg = command.Argument;
			if (placeholder != null)
			{
				cmd = placeholder.ReplacePlaceholder(command.Cmd);
				arg = placeholder.ReplacePlaceholder(command.Argument);
			}

			string absoluteCmdPatch = cmd;//Path.GetFullPath(cmd.Cmd);
			if (GCore.Path.IsAbsolutePatch(absoluteCmdPatch))
			{
				absoluteCmdPatch = Path.Combine(workspaceDirectory, absoluteCmdPatch);
				absoluteCmdPatch = Path.GetFullPath(absoluteCmdPatch);
			}

			var process = Process.Start(new ProcessStartInfo(absoluteCmdPatch, arg)
			{
				WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
				CreateNoWindow = true,
				UseShellExecute = false,
			});
			process.WaitForExit();
		}
	}
}
