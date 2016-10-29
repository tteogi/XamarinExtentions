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
			IdeApp.Workspace.ItemAddedToSolution += OnProjectAdded;
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
			if (project.IsCommandOnSave)
			{
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


				var editor = IdeApp.Workbench.ActiveDocument.Editor;
				var offset = editor.CaretOffset;
				var line = editor.CaretLine;
				var col = editor.CaretColumn;

				IdeApp.Workbench.ActiveDocument.Reload();

				editor.CaretLine = line;
				editor.CaretColumn = col;
				editor.CenterTo(offset);
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
				item.ProjectProperties.SetValue("CommandFilePathKey", "../../Env/RunOnSave.config");
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

		private void OnProjectAdded(object sender, SolutionItemChangeEventArgs e)
		{
			var project = e.SolutionItem as Project;
			if (project != null)
			{
				project.ProjectProperties.SetValue("CommandFilePathKey", "../../Env/RunOnSave.config");
				App.Property.Projects.Add(new CommandProperty.Project(
					project.Name, project.BaseDirectory, new GMonoDevelop.UniversalPropertySet(project.ProjectProperties)));
			}
		}
	}
}
