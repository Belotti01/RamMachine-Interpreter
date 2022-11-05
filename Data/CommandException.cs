namespace RamMachineInterpreter.Data;

public class CommandException : Exception {

	public CommandException(string message, object command) : base($"{message}\n{command}")
	{
	}
}