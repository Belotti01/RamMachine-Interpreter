using System.Collections.ObjectModel;
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

	public InstructionSet(TInterpreter interpreter, bool isCaseSensitive = false) {
		Interpreter = interpreter;
		
		StringComparer comparer = isCaseSensitive 
			? StringComparer.Ordinal 
			: StringComparer.OrdinalIgnoreCase;
		var operations = new Dictionary<string, Func<TOperation, string?>>(comparer);
		var operationAttributes = new Dictionary<string, TAttribute>(comparer);
		var methods = GetType().GetMethods();

		foreach (var method in methods ) {
			var cmds = method.GetCustomAttributes<TAttribute>();
			if(!cmds.Any())
				continue;

			if(method.ReturnType != typeof(string))
				throw new Exception($"Method \"{method.Name}\" of type \"{GetType().Name}\" must return a value of type <string?>.");

			var param = method.GetParameters();
			if(param.Length != 1 || param[0].ParameterType != typeof(TOperation))
				throw new Exception($"Method \"{method.Name}\" of type \"{GetType().Name}\" must have exactly one parameter of type {nameof(TOperation)}.");

			foreach(var attr in cmds)
			{
				if(operations.ContainsKey(attr.Command))
				{
					throw new Exception($"Duplicate command definition for \"{attr.Command}\" in {GetType().Name}.");
				}
				operations.Add(attr.Command, method.CreateDelegate<Func<TOperation, string?>>(this));
				operationAttributes.Add(attr.Command, attr);
			}
		}

		Operations = new(operations);
		OperationAttributes = new(operationAttributes);
	}

	public string? Execute(TOperation operation)
	{
		if(operation.Operation is null)
			throw new CommandException("No operation specified.", operation);
		if(!Operations.TryGetValue(operation.Operation, out var action))
			throw new CommandException($"Unknown operation:", operation);
		
		return action(operation);
	}
}
