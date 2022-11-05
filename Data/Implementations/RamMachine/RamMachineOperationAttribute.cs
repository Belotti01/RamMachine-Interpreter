using System.ComponentModel;

namespace RamMachineInterpreter.Data;

public class RamMachineOperationAttribute : OperationAttribute {

	[DisplayName("Requires numeric argument")]
	public bool RequiresNumericArgument { get; protected init; }

	[DisplayName("Allows direct value argument")]
	public bool AllowDirect { get; protected init; }

	[DisplayName("Allows pointer value argument")]
	public bool AllowPointer { get; protected init; }

	[DisplayName("Requires label argument")]
	public bool RequiresLabelArgument { get; protected init; }

	public RamMachineOperationAttribute(string command, bool requiresNumericArgument, bool allowDirect, bool allowPointer) : base(command)
	{
		this.RequiresNumericArgument = requiresNumericArgument;
		this.AllowDirect = allowDirect;
		this.AllowPointer = allowPointer;
	}

	public RamMachineOperationAttribute(string command, bool requiresNumericArgument, bool allowDirect, bool allowPointer, bool requiresLabelArgument) : this(command, requiresNumericArgument, allowDirect, allowPointer)
	{
		this.RequiresLabelArgument = requiresLabelArgument;
	}
}