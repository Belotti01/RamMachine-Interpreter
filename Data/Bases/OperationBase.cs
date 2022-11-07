namespace RamMachineInterpreter.Data;

public class OperationBase<T> : IOperation<T> where T : struct {
	public string RawLine { get; protected set; }
	public int OperationNumber { get; protected set; } = -1;
	public string? Instruction { get; protected set; }

	public OperationBase(string codeLine, int operationNumber = -1)
	{
		RawLine = codeLine.Trim();
		OperationNumber = operationNumber;
	}

	public override string ToString()
	{
		return OperationNumber < 1 
			? RawLine
			: $"{OperationNumber}: {RawLine}";
	}
}