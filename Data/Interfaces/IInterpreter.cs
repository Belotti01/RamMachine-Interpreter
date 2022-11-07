using System.Diagnostics.CodeAnalysis;

namespace RamMachineInterpreter.Data;

public interface IInterpreter<TMemory, TInstructionSet, TOperation, TAttribute, T>
	where TOperation : IOperation<T>
	where T : struct
	where TMemory : IMemory<T>
	where TAttribute : OperationAttribute
	where TInstructionSet : IInstructionSet<TOperation, TAttribute, T> {

	/// <summary>
	/// The set of instructions that can be parsed and executed by this interpreter.
	/// </summary>
	public TInstructionSet InstructionSet { get; }

	/// <summary>
	/// The data container used by this interpreter.
	/// </summary>
	public TMemory Memory { get; }

	/// <summary>
	/// The list of instructions parsed through <see cref="LoadCode(IEnumerable{string})"/>.
	/// </summary>
	public List<TOperation> Instructions { get; }

	/// <summary>
	/// The delay (in milliseconds) between each instruction execution in <see cref="ExecuteAsync(IEnumerable{string})"/>.
	/// Will be applied even if the execution is already running.
	/// </summary>
	public int Delay { get; set; }

	/// <summary>
	/// The number of instructions parsed through <see cref="LoadCode(IEnumerable{string})"/>.
	/// </summary>
	public int InstructionsCount { get; }

	/// <summary>
	/// Whether the interpreter is currently doing asynchronous work.
	/// </summary>
	public bool IsExecuting { get; }

	/// <summary>
	/// The data to be issued when required by an input-access instruction.
	/// </summary>
	public string[]? Inputs { get; set; }

	/// <summary>
	/// The index of the next element inside <see cref="Inputs"/> that will be issued when required by an input-access instruction.
	/// </summary>
	public int NextInputIndex { get; set; }

	/// <summary>
	/// Reset the state of this interpreter and its <see cref="Memory"/>.
	/// </summary>
	public void Reset();

	/// <summary>
	/// Attempt to parse the code in <paramref name="codeLines"/>.
	/// </summary>
	/// <param name="codeLines">The code to parse.</param>
	/// <param name="errors"><see langword="null"/> if the code parsing is successfull, or an array of all error messages if not.</returns>
	/// <returns><see langword="true"/> if the code is parsed successfully, <see langword="false"/> if not.</returns>
	public bool TryLoadCode(IEnumerable<string> codeLines, [NotNullWhen(false)] out string[]? errors);

	/// <summary>
	/// Parse a single code line and get the resulting <see cref="TOperation"/>.
	/// </summary>
	/// <param name="codeLine">The code segment to parse.</param>
	/// <param name="lineNumber">The segment number of the code to parse.</param>
	/// <returns>The parsed <see cref="TOperation"/>.</returns>
	public TOperation ParseCodeLine(string codeLine, int lineNumber);

	/// <summary>
	/// Execute a single operation in <see cref="Instructions"/>, resuming from the last executed one.
	/// </summary>
	/// <returns>The output of the operation, or <see langword="null"/> if no output is issued.</returns>
	public string? ExecuteNext();

	/// <summary>
	/// Execute all <see cref="Instructions"/> asynchronously, with <see cref="Delay"/> milliseconds between
	/// each <see cref="TOperation"/>.
	/// </summary>
	/// <param name="inputs">The inputs to use for this execution.</param>
	/// <returns>An array of all non-<see langword="null"/> outputs issued during the execution.</returns>
	public Task<string[]> ExecuteAsync(IEnumerable<string> inputs);

	/// <summary>
	/// Issue the cancellation of <see cref="ExecuteAsync(IEnumerable{string})"/>, and wait
	/// for the interruption to take place. Does nothing if the interpreter has already concluded execution or is
	/// not running.
	/// </summary>
	public Task StopExecutionAsync();
}