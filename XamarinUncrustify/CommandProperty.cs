using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using MonoDevelop.Projects;
using GMonoDevelop;

namespace XamarinUncrustify
{
	public class CommandProperty
	{
		private const string IsCommandOnSaveKey = "IsCommandOnSaveKey";
		private const string CommandFilePathKey = "CommandFilePathKey";

		public class Command
		{
			public string Cmd;
			public string Argument;
			public string FileMatch;
		}

		public class Project
		{
			private UniversalPropertySet _propertySet;
			private FileSystemWatcher _commandFilewatcher;
			private string _name;

			public bool IsCommandOnSave
			{
				get
				{
					if (_propertySet.HasValue(IsCommandOnSaveKey) == false)
						return true;
					return _propertySet.GetValue<bool>(IsCommandOnSaveKey);
				}
			}
			public string CommandFilePath
			{
				get
				{
					if (_propertySet.HasValue(CommandFilePathKey) == false)
						return null;
					return _propertySet.GetValue<string>(CommandFilePathKey);
				}
			}

			private List<Command> _commands;
			public IList<Command> Commands { get { return _commands; } }

			public string Name
			{
				get { return _name; }
			}

			public Project(string name, string workspace, UniversalPropertySet set)
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

			public void Reset(string name, string workspace, UniversalPropertySet set)
			{
				_name = name;
				_propertySet = set;

				if (string.IsNullOrEmpty(CommandFilePath) == false)
				{
					var commandFilePath = Path.GetFullPath(Path.Combine(workspace, CommandFilePath));
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
						_commandFilewatcher.Filter = "*"+Path.GetExtension(commandFilePath);
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
					var text = File.ReadAllText(e.FullPath);
					_commands = JsonConvert.DeserializeObject<List<Command>>(text);
				}
			}
		}

		private List<Project> _projects = new List<Project>();

		public List<Project> Projects
		{
			get { return _projects; }
		}

		public Project GetProject(string name)
		{
			return _projects.Find((obj) => obj.Name == name);
		}

		//public void SaveProperties()
		//{
		//	if (_properties != null)
		//	{
		//		JsonConvert.SerializeObject(_properties);
		//	}
		//}

		//public void LoadProperties(IPropertySet properties)
		//{
		//	if (properties.HasProperty(PropertiesKey))
		//	{
		//		var jsonStr = properties.GetValue(PropertiesKey);
		//		_properties = JsonConvert.DeserializeObject<CommandProperty>(jsonStr);
		//	}
		//	_properties = new CommandProperty();
		//}
	}
}
