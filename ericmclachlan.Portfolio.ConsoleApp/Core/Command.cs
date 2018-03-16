namespace ericmclachlan.Portfolio.ConsoleApp
{
    /// <summary>
    /// <para>
    /// Commands that implement this interface are detected and made executable through the CommandPlatform framework.
    /// Commands are identified by passing the <c>CommandName</c> as the first parameter in a series of arguments.
    /// </para><para>
    /// Parameters can be automatically initialized and passed to the command. 
    /// These parameters can be passed to properties marked with the <c>CommandParameterAttribute</c>.
    /// </para>
    /// </summary>
    public abstract class Command
    {
        /// <summary>The name of the command. Also, this name must be passed as the first parameter of the command.</summary>
        public abstract string CommandName { get; }

        /// <summary>
        /// This method defines the entry-point to the command. 
        /// The framework will ensure that you enter this method with parameters initialized.
        /// </summary>
        /// <returns>Any output associated with this command. (This is particularly useful for unit testing.)</returns>
        public abstract object Execute();
    }

    /// <summary>
    /// <para>
    /// Commands that implement this interface are detected and made executable through the CommandPlatform framework.
    /// Commands are identified by passing the <c>CommandName</c> as the first parameter in a series of arguments.
    /// </para><para>
    /// Parameters can be automatically initialized and passed to the command. 
    /// These parameters can be passed to properties marked with the <c>CommandParameterAttribute</c>.
    /// </para>
    /// </summary>
    public abstract class Command<T> : Command
    {
        /// <summary>
        /// This method defines the entry-point to the command. 
        /// The framework will ensure that you enter this method with parameters initialized.
        /// </summary>
        /// <returns>Any output associated with this command. (This is particularly useful for unit testing.)</returns>
        public abstract T ExecuteCommand();

        public override sealed object Execute()
        {
            return ExecuteCommand();
        }
    }
}
