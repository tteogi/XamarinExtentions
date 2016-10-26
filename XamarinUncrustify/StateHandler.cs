using System;
using System.Collections.Generic;
using System.IO;
using Mono.Addins;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Newtonsoft.Json;

namespace XamarinUncrustify
{
	public class StateHandler : CommandHandler
	{
		protected override void Run()
		{
			IdeApp.Workspace.WorkspaceItemOpened += OnSoultionOpened;
			IdeApp.Workspace.ItemRemovedFromSolution += OnProjectRemoved;
		}

		private void OnRemove(object sender, SolutionItemChangeEventArgs e)
		{
			var name = e.SolutionItem.Name;
			App.Property.Projects.RemoveAll((obj) => obj.Name == name);
			name = e.SolutionItem.Name;
		}

		private void OnSaved(object sender, EventArgs e)
		{
			var name = IdeApp.Workbench.ActiveDocument.Project.Name;
			var project = App.Property.Projects.Find((obj) => obj.Name == name);

			var placeholder = new Placeholder(new Dictionary<string, string>
			{
				{
					@"\${file}",
					Path.GetFullPath(IdeApp.Workbench.ActiveDocument.FileName)
				},
				{
					@"\${workspace}",
					Path.GetFullPath(IdeApp.ProjectOperations.CurrentSelectedProject.BaseDirectory)
				},
			});

			foreach (var cmd in project.Commands)
			{
				CommandExecuter.Execute(cmd, IdeApp.ProjectOperations.CurrentSelectedProject.BaseDirectory, placeholder);
			}
			IdeApp.Workbench.ActiveDocument.Reload();

			//if (Environment.OSVersion.Platform == PlatformID.Unix)
			//{
			//}
		}

		private void OnDocumentOpened(object sender, DocumentEventArgs e)
		{
			e.Document.Saved += OnSaved;
		}

		private void OnDocumentClosed(object sender, DocumentEventArgs e)
		{
			e.Document.Saved -= OnSaved;
		}

		private void OnSoultionOpened(object sender, WorkspaceItemEventArgs e)
		{
			IdeApp.Workbench.DocumentOpened += OnDocumentOpened;
			IdeApp.Workbench.DocumentClosed += OnDocumentClosed;
			IdeApp.Workspace.ItemRemovedFromSolution += OnRemove;

			foreach (var item in IdeApp.Workspace.GetAllItems<Project>())
			{
				item.ProjectProperties.SetValue("CommandFilePathKey", "../../test.txt");
				App.Property.Projects.Add(new CommandProperty.Project(item.Name, item.BaseDirectory,
																   new GMonoDevelop.UniversalPropertySet(item.ProjectProperties)));
			}
		}

		void OnProjectRemoved(object sender, SolutionItemChangeEventArgs e)
		{
			var project = e.SolutionItem as Project;
			if (project != null)
			{
				App.Property.Projects.RemoveAll((obj) => obj.Name == project.Name);
			}
		}
	}
}
