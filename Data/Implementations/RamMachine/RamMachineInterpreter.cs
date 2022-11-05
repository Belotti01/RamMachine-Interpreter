namespace RamMachineInterpreter.Data;

public class RamMachineInterpreter : Interpreter<RamMachineMemory, RamMachineInstructionSet, RamMachineOperation, RamMachineOperationAttribute, long> {
	public override RamMachineInstructionSet InstructionSet { get; protected set; }
	public override RamMachineMemory Memory { get; protected set; }

	public RamMachineInterpreter()
	{
		Memory = new();
		InstructionSet = new(this);
	}

	public override RamMachineOperation ParseCodeLine(string codeLine, int lineNumber)
	{
		return new RamMachineOperation(codeLine, InstructionSet, lineNumber);
	}
}