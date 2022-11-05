namespace RamMachineInterpreter.Data;

public class InterpreterLanguage<T>
	where T : struct {
	public string Name { get; protected set; }
	public string? Description { get; set; }
	public IInterpreter<IMemory<T>, IInstructionSet<IOperation<T>, OperationAttribute, T>, IOperation<T>, OperationAttribute, T> Interpreter { get; protected set; }

	public InterpreterLanguage(string name, IInterpreter<IMemory<T>, IInstructionSet<IOperation<T>, OperationAttribute, T>, IOperation<T>, OperationAttribute, T> interpreter)
	{
		Name = name;
		Interpreter = interpreter;
	}
}