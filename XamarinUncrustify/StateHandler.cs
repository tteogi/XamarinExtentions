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
			try
			{
				IdeApp.Workspace.WorkspaceItemOpened += OnSoultionOpened;
				IdeApp.Workspace.ItemRemovedFromSolution += OnProjectRemoved;
				IdeApp.Workspace.ItemRemovedFromSolution += OnProjectRemoved;
				IdeApp.Workspace.ItemAddedToSolution += OnProjectAdded;
			}
			catch
			{
			}
		}

		private void OnRemove(object sender, SolutionItemChangeEventArgs e)
		{
			var name = e.SolutionItem.Name;
			App.Properties[e.Solution.Name].Projects.RemoveAll((obj) => obj.Name == name);
			name = e.SolutionItem.Name;
		}

		private void OnSaved(object sender, EventArgs e)
		{
			var appProject = IdeApp.Workbench.ActiveDocument.Project;
			var name = appProject.Name;
			var project = App.Properties[appProject.ParentSolution.Name]
			                 .Projects.Find((obj) => obj.Name == name);
			if (project.IsRunOnSave)
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
				if (project.Commands != null)
				{
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

			e.Item.NameChanged += (nameChange, changedArg) =>
			{
				CommandProperty prop;
				if (App.Properties.TryGetValue(changedArg.OldName, out prop))
				{
					App.Properties.Remove(changedArg.OldName);
					App.Properties[changedArg.NewName] = prop;
				}
			};

			var property = new CommandProperty();
			App.Properties[e.Item.Name] = property;
			property.LoadProperties(e.Item.BaseDirectory);

			foreach (var item in IdeApp.Workspace.GetAllItems<Project>())
			{
				AddProject(item);
			}
		}

		void OnProjectRemoved(object sender, SolutionItemChangeEventArgs e)
		{
			var project = e.SolutionItem as Project;
			if (project != null)
			{
				App.Properties[e.Solution.Name].Projects.RemoveAll((obj) => obj.Name == project.Name);
			}
		}

		private void OnProjectAdded(object sender, SolutionItemChangeEventArgs e)
		{
			var project = e.SolutionItem as Project;
			if (project != null)
			{
				AddProject(project);
			}
		}

		static void AddProject(Project project)
		{
			if (project.ProjectProperties.HasProperty("CommandFilePathKey") == false)
				project.ProjectProperties.SetValue("CommandFilePathKey", "../../Env/RunOnSave.config");
			App.Properties[project.ParentSolution.Name].AddProject(project.Name, project.BaseDirectory);
		}
	}
}
