using System;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;
using MonoDevelop.Projects;
using MonoDevelop.Ide;

namespace XamarinUncrustify
{
	public class ConfigurePanel : OptionsPanel
	{
		private Xwt.CheckBox _cmdOnSave;
		private Xwt.SearchTextEntry _cmdFilePath;

		public override void ApplyChanges()
		{
			var project = App.Properties[IdeApp.ProjectOperations.CurrentSelectedSolution.Name]
			                 .GetProject(IdeApp.ProjectOperations.CurrentSelectedProject.Name);
			project.IsRunOnSave = _cmdOnSave.Active;
			project.CommandConfigPath = _cmdFilePath.Text;
			project.SaveConfig();
			App.Properties[IdeApp.ProjectOperations.CurrentSelectedSolution.Name].SaveProperties(IdeApp.ProjectOperations.CurrentSelectedSolution.BaseDirectory);
		}

		public override Control CreatePanelWidget()
		{
			Xwt.VBox box = new Xwt.VBox();
			box.Spacing = 6;
			box.Margin = 12;

			var sol = IdeApp.ProjectOperations.CurrentSelectedSolution;
			var pro = IdeApp.ProjectOperations.CurrentSelectedProject;

			var project = App.Properties[sol.Name].GetProject(IdeApp.ProjectOperations.CurrentSelectedProject.Name);
			_cmdOnSave = new Xwt.CheckBox(GettextCatalog.GetString("run on save"))
			{
				Active = project == null || project.IsRunOnSave
			};

			_cmdFilePath = new Xwt.SearchTextEntry()
			{
				Text = project == null ? "" : project.CommandConfigPath
			};

			box.PackStart(_cmdOnSave);
			box.PackStart(_cmdFilePath);
			box.Show();
			return box.ToGtkWidget();
		}
	}
}
