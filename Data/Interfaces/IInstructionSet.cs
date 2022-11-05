using System.Collections.ObjectModel;

namespace RamMachineInterpreter.Data;

public interface IInstructionSet<TOperation, TAttribute, T>
	where TOperation : IOperation<T>
	where TAttribute : OperationAttribute
	where T : struct {

	/// <summary>
	/// Whether the instructions of this Set are case sensitive.
	/// </summary>
	public static bool IsCaseSensitive { get; }

	/// <summary>
	/// Collection of operands and their corresponding handlers.
	/// </summary>
	public ReadOnlyDictionary<string, Func<TOperation, string?>> Operations { get; }

	/// <summary>
	/// Collection of operands and their corresponding definitions.
	/// </summary>
	public ReadOnlyDictionary<string, TAttribute> OperationAttributes { get; }

	/// <summary>
	/// Execute an operation and return eventual outputs.
	/// </summary>
	/// <param name="operation">Identifier and data of the operation to execute.</param>
	/// <returns>The output resulting from the execution of the <paramref name="operation"/>.
	/// <see langword="null"/> if no output is issued by the operation.</returns>
	public string? Execute(TOperation operation);
}