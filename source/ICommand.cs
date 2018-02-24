namespace ericmclachlan.Portfolio
{
    /// <summary>Implementors are executable as sub-commands from the command line.</summary>
    internal interface ICommand
    {
        /// <summary>The name of the command. Also, this name is required as the first parameter of the command.</summary>
        string CommandName { get; }

        void ExecuteCommand();
    }
}
