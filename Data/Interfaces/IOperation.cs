namespace RamMachineInterpreter.Data;

public interface IOperation<T> where T : struct {

	/// <summary>
	/// The number of the code segment containing this operation.
	/// </summary>
	public int OperationNumber { get; }

	/// <summary>
	/// The unique identifier of the instruction to execute.
	/// </summary>
	public string? InstructionId { get; }
}