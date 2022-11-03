namespace RamMachineInterpreter.Data;

public class CommandException : Exception {
	public int CodeLine { get; } = -1;

	public CommandException(string message, object command) : base($"{message}\n{command}") { }
}
