using System;
using System.Diagnostics.CodeAnalysis;

namespace RamMachineInterpreter.Data;

public interface IInterpreter<TMemory, TInstructionSet, TOperation, TAttribute, T>
	where TOperation : IOperation<T>
	where T : struct
	where TMemory : IMemory<T>
	where TAttribute : OperationAttribute
	where TInstructionSet : IInstructionSet<TOperation, TAttribute, T> {
	public TInstructionSet InstructionSet { get; }
    public TMemory Memory { get; }
    public List<TOperation> Instructions { get; }
	public int Delay { get; set; }
	public int InstructionsCount { get; }
    public bool IsExecuting { get; }
    public string[]? Inputs { get; }
	public int NextInputIndex { get; set; }

    public void Reset();
    public string[] LoadCode(IEnumerable<string> codeLines);
    public TOperation ParseCodeLine(string codeLine, int lineNumber);
	public string? ExecuteNext();
    public Task<string[]> ExecuteAsync(IEnumerable<string> inputs);
    public Task StopExecutionAsync();
}
