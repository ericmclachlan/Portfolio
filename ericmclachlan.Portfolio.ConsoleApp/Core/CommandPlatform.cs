using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ericmclachlan.Portfolio.ConsoleApp
{
    internal enum CommandParameterType
    {
        BasicType
        , InputFile
        , NonNegativeInteger
        , PositiveInteger
    }

    /// <summary>
    /// <para>The command platform acts as the platform from which commands can be executed.</para>
    /// <para>Loading of commands is dynamic; requiring only that classes implement the ICommand interface. </para>
    /// </summary>
    public static class CommandPlatform
    {   
        // Private Members

        internal static Dictionary<string, Command> SupportedCommands = new Dictionary<string, Command>();


        // Construction

        /// <summary>Initializes the list of supported commands.</summary>
        static CommandPlatform()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsAbstract && typeof(Command).IsAssignableFrom(type))
                {
                    var instance = Activator.CreateInstance(Assembly.GetExecutingAssembly().FullName, type.FullName);
                    if (instance.Unwrap() is Command command)
                    {
                        SupportedCommands[command.CommandName] = command;
                    }
                }
            }
        }


        // Methods

        public static object Execute(params string[] args)
        {
            // Rare Case: No command is specified.
            if (args.Length == 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("No command has been specified.{0}", Environment.NewLine);
                sb.AppendFormat("Run '{0} {1}' for a list of supported commands.{2}", Path.GetFileName(Assembly.GetEntryAssembly().Location), "help", Environment.NewLine);
                throw new Exception(sb.ToString());
            }

            // General Case: A command is specified:
            Command command;
            // Rare Case: The command is not supported:
            if (!SupportedCommands.TryGetValue(args[0], out command))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("The '{0}' command is not supported by this version of the application.{1}", args[0], Environment.NewLine);
                sb.AppendFormat("Run '{0} {1}' for a list of supported commands.{2}", Path.GetFileName(Assembly.GetEntryAssembly().Location), "help", Environment.NewLine);
                throw new Exception(sb.ToString());
            }

            // General Case: The command is supported:
            Type type = command.GetType();
            Console.Error.WriteLine($"Running '{command.CommandName}':");
            int maxIndex = -1;
            var commandParameters = new Dictionary<int, CommandParameterAttribute>();
            var properties = new Dictionary<int, PropertyInfo>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                foreach (CommandParameterAttribute commandParameter in property.GetCustomAttributes<CommandParameterAttribute>())
                {
                    if (commandParameter.Index > maxIndex)
                        maxIndex = commandParameter.Index;

                    // Verify that this parameter is being set once and only once.
                    if (commandParameters.ContainsKey(commandParameter.Index))
                    {
                        string errorMessage = string.Format($"There appears to be a problem with the definition of the '{command.CommandName}' command as there are two parameters registered with Index='{commandParameter.Index}'.");
                        throw new Exception(errorMessage);
                    }

                    commandParameters[commandParameter.Index] = commandParameter;
                    properties[commandParameter.Index] = property;
                }
            }

            // Go through each parameter in sequence:
            var results = from parameter in commandParameters.Values
                          orderby parameter.Index
                          select parameter;
            foreach (CommandParameterAttribute commandParameter in results)
            {
                PropertyInfo property = properties[commandParameter.Index];
                int argIndex = commandParameter.Index + 1;

                string text;
                if (argIndex >= args.Length)
                {
                    if (commandParameter.IsRequired)
                        throw new Exception($"Parameter '{property.Name}' has not been specified.");
                    else
                        text = string.Empty;
                }
                else
                {
                    text = args[argIndex].Trim();
                }
                ValidateAndSetPropertyValue(command, property, commandParameter.Type, ref text);

                Console.Error.WriteLine($"\t{property.Name}\t= '{text}'");
            }

            // Output the parameter values to the console.
            for (int i = 0; i <= maxIndex; i++)
            {
                CommandParameterAttribute commandParameter;
                if (!commandParameters.TryGetValue(i, out commandParameter) && commandParameter.IsRequired)
                {
                    throw new Exception($"No value has been specified for required parameter '{properties[i].Name}'.");
                }
            }
            return command.Execute();
        }

        /// <summary>Sets <c>property</c> to the value of <c>valueAsText</c>.</summary>
        private static void ValidateAndSetPropertyValue(
            Command command
            , PropertyInfo property
            , CommandParameterType commandParameterType
            , ref string valueAsText)
        {
            if (property.PropertyType.IsAssignableFrom(typeof(string)))
            {
                if (!string.IsNullOrWhiteSpace(valueAsText) && commandParameterType == CommandParameterType.InputFile)
                {
                    FileInfo fi = new FileInfo(valueAsText);
                    Uri absoluteUri = new Uri(fi.FullName, UriKind.Absolute);
                    var referenceUri = new Uri(Environment.CurrentDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
                    var relativeUri = referenceUri.MakeRelativeUri(absoluteUri);
                    valueAsText = Uri.UnescapeDataString(relativeUri.ToString());
                    if (!File.Exists(valueAsText))
                    {
                        string errorMessage = string.Format($"Unable to find the file specified for parameter '{property.Name}'.");
                        throw new Exception(errorMessage);
                    }
                }
                property.SetValue(command, valueAsText);
            }
            else if (property.PropertyType.IsAssignableFrom(typeof(int)))
            {
                int value;
                if (!int.TryParse(valueAsText, out value))
                {
                    string errorMessage = string.Format($"The '{property.Name}' parameter is not a valid integer value.");
                    throw new Exception(errorMessage);
                }
                if (commandParameterType == CommandParameterType.NonNegativeInteger && value < 0)
                {
                    string errorMessage = string.Format($"The '{property.Name}' parameter must be a non-negative integer.");
                    throw new Exception(errorMessage);
                }
                else if (commandParameterType == CommandParameterType.PositiveInteger && value <= 0)
                {
                    string errorMessage = string.Format($"The '{property.Name}' parameter must be a positive integer.");
                    throw new Exception(errorMessage);
                }
                property.SetValue(command, value);
            }
            else if (property.PropertyType.IsAssignableFrom(typeof(double)))
            {
                double value;
                if (!double.TryParse(valueAsText, out value))
                {
                    string errorMessage = string.Format($"Parameter '{property.Name}' is not a valid real value.");
                    throw new Exception(errorMessage);
                }
                property.SetValue(command, value);
            }
            else
            {
                throw new Exception($"Field '{property.Name}' has an unanticipated type.");
            }
        }


        // Inner Classes

        internal abstract class BuiltInCommand<T>: Command<T>
        {
            // Nothing else needs to be done.
        }

        /// <summary>This command lists all available commands.</summary>
        internal class Command_help : BuiltInCommand<bool>
        {
            // Properties

            public override string CommandName { get { return "help"; } }

            public override bool ExecuteCommand()
            {
                Debug.Assert(SupportedCommands.Count > 0);

                // Writes a list of supported command types to console.
                Console.Error.WriteLine("The following commands are supported by this version of the application:");
                var commandList = from command in SupportedCommands.Values
                                  orderby command.CommandName
                                  select command;
                foreach (Command command in commandList)
                {
                    Console.Error.WriteLine("\t{0}", command.CommandName);
                }
                return true;
            }
        }


        /// <summary>This command generates bash scrtips for commands that are not derived from BuildInCommand.</summary>
        internal class Command_generateScripts : BuiltInCommand<bool>
        {
            // Properties

            public override string CommandName { get { return "generate_scripts"; } }

            public override bool ExecuteCommand()
            {
                Debug.Assert(SupportedCommands.Count > 0);

                // Writes a list of supported command types to console.
                var commandList = from command in SupportedCommands.Values
                                  orderby command.CommandName
                                  select command;
                foreach (Command command in commandList)
                {
                    // Don't generate scripts for the built-in commands.
                    if (command is BuiltInCommand<bool>)
                        continue;

                    StringBuilder script = new StringBuilder();
                    script.AppendLine("#!/bin/sh");
                    script.AppendLine();

                    // Create a list of parameters:
                    var parameters = new List<CommandParameterAttribute>();
                    foreach (PropertyInfo property in command.GetType().GetProperties())
                    {
                        foreach (CommandParameterAttribute commandParameter in property.GetCustomAttributes<CommandParameterAttribute>())
                        {
                            parameters.Add(commandParameter);
                        }
                    }

                    // Output the parameter descriptions:
                    foreach (CommandParameterAttribute parameter in parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(parameter.Description))
                            script.AppendLine($"#\t${parameter.Index + 1}:\t{parameter.Description}");
                    }
                    script.AppendLine();
                    script.AppendLine("#set -x	# echo on");
                    script.Append("time mono ");
                    script.Append(Path.GetFileName(Assembly.GetEntryAssembly().Location));
                    script.Append(" ");
                    script.Append(command.CommandName);

                    int i = 1;
                    foreach (PropertyInfo property in command.GetType().GetProperties())
                    {
                        foreach (CommandParameterAttribute commandParameter in property.GetCustomAttributes<CommandParameterAttribute>())
                        {
                            script.AppendFormat(" ${0}", i++);
                        }
                    }

                    script.AppendLine();

                    string fileName = command.CommandName + ".sh";
                    File.WriteAllText(fileName, script.ToString());
                    Console.Error.WriteLine($"\tGenerating '{fileName}'");
                }
                return true;
            }
        }
    }
}
