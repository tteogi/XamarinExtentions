using System;
using System.Collections.Generic;
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
			IdeApp.Workspace.ItemRemovedFromSolution += Workspace_ItemRemovedFromSolution;
	
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
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
			}
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
						item.ProjectProperties.SetValue("CommandFilePathKey", "../../test.config");
						//var monitor = new MonoDevelop.Core.ProgressMonitor();
						//var task = item.SaveAsync(monitor);
						//while (true)
						//{
						//	if (task.IsCompleted || task.IsCanceled || task.IsFaulted)
						//		break;
						//	task.Wait(100);
						//}
						App.Property.Projects.Add(new CommandProperty.Project(item.Name, item.BaseDirectory,
																		   new GMonoDevelop.UniversalPropertySet(item.ProjectProperties)));
					}
		}

		void Workspace_ItemRemovedFromSolution (object sender, SolutionItemChangeEventArgs e)
		{
			var project = e.SolutionItem as Project;
			if (project != null)
			{
				App.Property.Projects.RemoveAll((obj) => obj.Name == project.Name);
			}
		}
	}
}
