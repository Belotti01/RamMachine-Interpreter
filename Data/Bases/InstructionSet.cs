using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace RamMachineInterpreter.Data;

public abstract class InstructionSet<TInterpreter, TMemory, TSelf, TOperation, TAttribute, T>
	: IInstructionSet<TOperation, TAttribute, T>
	where TInterpreter : IInterpreter<TMemory, TSelf, TOperation, TAttribute, T>
	where TOperation : IOperation<T>
	where T : struct
	where TMemory : IMemory<T>
	where TAttribute : OperationAttribute
	where TSelf : IInstructionSet<TOperation, TAttribute, T> {
	public ReadOnlyDictionary<string, Func<TOperation, string?>> Operations { get; private set; }
	public ReadOnlyDictionary<string, TAttribute> OperationAttributes { get; private set; }
	protected IMemory<T> Memory => Interpreter.Memory;
	protected TInterpreter Interpreter { get; set; }

	public InstructionSet(TInterpreter interpreter, bool isCaseSensitive = false)
	{
		Interpreter = interpreter;

		StringComparer comparer = isCaseSensitive
			? StringComparer.Ordinal
			: StringComparer.OrdinalIgnoreCase;
		var operations = new Dictionary<string, Func<TOperation, string?>>(comparer);
		var operationAttributes = new Dictionary<string, TAttribute>(comparer);
		var methods = GetType().GetMethods();

		foreach(var method in methods)
		{
			var cmds = method.GetCustomAttributes<TAttribute>();
			if(!cmds.Any())
				continue;

			Debug.Assert(method.ReturnType == typeof(string), $"Method \"{method.Name}\" of type \"{GetType().Name}\" must return a value of type <string?>.");
			var param = method.GetParameters();
			Debug.Assert(param.Length == 0 || (param.Length == 1 && param[0].ParameterType != typeof(TOperation)), $"Method \"{method.Name}\" of type \"{GetType().Name}\" must have exactly one parameter of type {nameof(TOperation)}.");

			foreach(var attr in cmds)
			{
				Debug.Assert(!operations.ContainsKey(attr.Command), $"Duplicate command definition for \"{attr.Command}\" in {GetType().Name}.");
				// Allow parameterless methods, in case the operation data is not needed.
				Func<TOperation, string?> func = param.Length == 1
					? method.CreateDelegate<Func<TOperation, string?>>(this)
					: (op) => method.Invoke(this, null) as string;
				
				operations.Add(attr.Command, func);
				operationAttributes.Add(attr.Command, attr);
			}
		}

		Operations = new(operations);
		OperationAttributes = new(operationAttributes);
		Debug.WriteLine($"Loaded {Operations.Count} operations for type {GetType().Name}");
	}

	public string? Execute(TOperation operation)
	{
		if(operation.InstructionId is null)
			throw new CommandException("No operation specified.", operation);
		if(!Operations.TryGetValue(operation.InstructionId, out var action))
			throw new CommandException($"Unknown operation:", operation);

		return action(operation);
	}
}