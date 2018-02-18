using System;

namespace ericmclachlan.Portfolio
{
    internal class CommandParameterAttribute : Attribute
    {
        public int Index { get; set; }

        public bool IsRequired { get; set; } = true;

        public CommandParameterType Type { get; set; } = CommandParameterType.BasicType;

        public string Description { get; set; } = String.Empty;
    }
}