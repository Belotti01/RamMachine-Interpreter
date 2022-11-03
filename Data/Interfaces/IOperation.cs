namespace RamMachineInterpreter.Data;

public interface IOperation<T> where T : struct {
	public int OperationNumber { get; }
	public string? Operation { get; }
}
