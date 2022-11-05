namespace RamMachineInterpreter.Data;

public class OperationBase<T> : IOperation<T> where T : struct {
	public string RawLine { get; protected set; }
	public int OperationNumber { get; protected set; } = -1;
	public string? InstructionId { get; protected set; }

	public OperationBase(string codeLine, int operationNumber = -1)
	{
		RawLine = codeLine.Trim();
		OperationNumber = operationNumber;
	}
}