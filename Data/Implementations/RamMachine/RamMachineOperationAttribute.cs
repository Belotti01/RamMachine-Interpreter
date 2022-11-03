namespace RamMachineInterpreter.Data;

public class RamMachineOperationAttribute : OperationAttribute {
	public bool RequiresNumericArgument { get; protected init; }
	public bool AllowDirect { get; protected init; }
	public bool AllowPointer { get; protected init; }
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
