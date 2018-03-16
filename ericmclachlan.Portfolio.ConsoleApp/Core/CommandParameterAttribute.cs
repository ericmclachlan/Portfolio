using System;

namespace ericmclachlan.Portfolio.ConsoleApp
{
    /// <summary>
    /// Decorating properties with this attribute makes them usable with implementors of ICommand.
    /// </summary>
    internal class CommandParameterAttribute : Attribute
    {
        /// <summary>Zero based index of the parameter.</summary>
        public int Index { get; set; }

        /// <summary>By default, all parameters are required.</summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>
        /// <para>
        /// This parameter is useful in situations where the type requires special validation.
        /// </para><para>
        /// For example, InputFiles are just strings but the command platform will 
        /// check that the file exists during validation.
        /// </para>
        /// </summary>
        public CommandParameterType Type { get; set; } = CommandParameterType.BasicType;

        /// <summary>
        /// This description will be used by the generate_scripts and help commands to better describe parameters.
        /// </summary>
        public string Description { get; set; } = String.Empty;
    }
}