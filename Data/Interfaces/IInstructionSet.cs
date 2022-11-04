using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace RamMachineInterpreter.Data;

public interface IInstructionSet<TOperation, TAttribute, T>
	where TOperation : IOperation<T>
	where TAttribute : OperationAttribute
	where T : struct
{
	public static bool IsCaseSensitive { get; }
	public ReadOnlyDictionary<string, Func<TOperation, string?>> Operations { get; }
	public ReadOnlyDictionary<string, TAttribute> OperationAttributes { get; }

	public string? Execute(TOperation operation);
}
