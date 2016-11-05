using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using MonoDevelop.Projects;
using GMonoDevelop;
using MonoDevelop.Core;

namespace XamarinUncrustify
{
	public class CommandProperty
	{
		//private const string IsCommandOnSaveKey = "IsCommandOnSaveKey";
		//private const string CommandFilePathKey = "CommandFilePathKey";

		public class Command
		{
			public string Cmd;
			public string Argument;
			public string FileMatch;
			public bool TextChange;
		}

		public class Project
		{
			private Config.ProjectOption _config = new Config.ProjectOption();
			private FileSystemWatcher _commandFilewatcher;
			private string _workspace;

			public bool IsRunOnSave = false;
			public string CommandConfigPath = "";

			private List<Command> _commands;
			public IList<Command> Commands { get { return _commands; } }

			public string Name
			{
				get { return Config.Name; }
			}

			public Config.ProjectOption Config
			{
				get { return _config; }
			}

			public Project(string name, string workspace, Config.ProjectOption set)
			{
				Reset(name, workspace, set);
			}

			~Project()
			{
				if (_commandFilewatcher != null)
				{
					_commandFilewatcher.Changed -= OnCommandsFileChanged;
					_commandFilewatcher.Dispose();
				}
			}

			public void SaveConfig()
			{
				_config.Name = Name;
				_config.IsRunOnSave = IsRunOnSave;
				_config.CommandConfigPath = CommandConfigPath;
				Reset(Name, _workspace, Config);
			}

			public void Reset(string name, string workspace, Config.ProjectOption set)
			{
				_config = set == null ? new Config.ProjectOption() : set;
				_config.Name = name;

				IsRunOnSave = _config.IsRunOnSave;
				CommandConfigPath = _config.CommandConfigPath;
				_workspace = workspace;

				if (string.IsNullOrEmpty(CommandConfigPath) == false)
				{
					var commandFilePath = Path.GetFullPath(Path.Combine(workspace, CommandConfigPath));
					if (File.Exists(commandFilePath))
					{
						var text = File.ReadAllText(commandFilePath);
						_commands = JsonConvert.DeserializeObject<List<Command>>(text);

						if (_commandFilewatcher != null)
						{
							_commandFilewatcher.Changed -= OnCommandsFileChanged;
							_commandFilewatcher.Dispose();
						}
						_commandFilewatcher = new FileSystemWatcher();
						_commandFilewatcher.Path = Path.GetDirectoryName(commandFilePath);
						_commandFilewatcher.Filter = "*" + Path.GetExtension(commandFilePath);
						_commandFilewatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
		   					| NotifyFilters.FileName | NotifyFilters.DirectoryName;

						_commandFilewatcher.Changed += OnCommandsFileChanged;

						_commandFilewatcher.EnableRaisingEvents = true;
					}
				}
			}

			private void OnCommandsFileChanged(object source, FileSystemEventArgs e)
			{
				Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
				if (File.Exists(e.FullPath))
				{
					var stream = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					StreamReader reader = new StreamReader(stream);
					_commands = JsonConvert.DeserializeObject<List<Command>>(reader.ReadToEnd());
				}
			}
		}

		private Config _config = new Config();

		private List<Project> _projects = new List<Project>();

		public List<Project> Projects
		{
			get { return _projects; }
		}

		public Project GetProject(string name)
		{
			return _projects.Find((obj) => obj.Name == name);
		}

		public void AddProject(string projectName, string projectDirectory)
		{
			Projects.Add(new CommandProperty.Project(
				projectName, projectDirectory, _config.ProjectOptions.Find((obj) => obj.Name == projectName)));
		}

		public void SaveProperties(string workspace)
		{
			try
			{
				_config.ProjectOptions = _projects.ConvertAll((input) => input.Config);
				var configFilePath = Path.Combine(workspace, "XExtension.config");
				var str = JsonConvert.SerializeObject(_config, Formatting.Indented);
				File.WriteAllText(configFilePath, str);

			}
			catch (Exception ex)
			{
				LoggingService.LogError("Unity Tools: error reading config", ex);
			}
		}

		public void LoadProperties(string workspace)
		{
			var configFilePath = Path.Combine(workspace, "XExtension.config");
			if (File.Exists(configFilePath))
			{
				try
				{
					_config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFilePath));
				}
				catch (Exception ex)
				{
					LoggingService.LogError("Unity Tools: error reading config", ex);
				}
			}
			else
			{
				LoggingService.LogInfo("Unity Tools: config not found, using default");
			}
		}
	}
}
