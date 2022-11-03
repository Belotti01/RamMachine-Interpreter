namespace RamMachineInterpreter.Data;

[AttributeUsage(AttributeTargets.Method)]
public class OperationAttribute : Attribute {
	public string Command { get; }
	public string? Description { get; }

	public OperationAttribute(string command)
	{
		Command = command;
	}
	
	public OperationAttribute(string command, string description)
	{
		Command = command;
		Description = description;
	}
}
