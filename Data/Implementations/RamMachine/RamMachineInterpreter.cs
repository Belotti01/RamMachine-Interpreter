namespace RamMachineInterpreter.Data;

public class RamMachineInterpreter : Interpreter<RamMachineMemory, RamMachineInstructionSet, RamMachineOperation, RamMachineOperationAttribute, long> {
	public override RamMachineOperation ParseCodeLine(string codeLine, int lineNumber)
	{
		return new RamMachineOperation(codeLine, InstructionSet, lineNumber);
	}
}
