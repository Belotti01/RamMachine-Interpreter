using System.ComponentModel;

namespace RamMachineInterpreter.Data;

[AttributeUsage(AttributeTargets.Method)]
public class OperationAttribute : Attribute {
	[DisplayName("Operation")]
	public string Command { get; }
	[DisplayName("Description")]
	public virtual string? Description { get; }

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
