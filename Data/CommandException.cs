namespace RamMachineInterpreter.Data;

public class CommandException : Exception {


	public CommandException(string message, Command command) : base($"{message}\n{command}") { }

}
