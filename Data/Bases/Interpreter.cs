using System.Diagnostics.CodeAnalysis;

namespace RamMachineInterpreter.Data;

public abstract class Interpreter<TMemory, TInstructionSet, TOperation, TAttribute, T> 
	: IInterpreter<TMemory, TInstructionSet, TOperation, TAttribute, T>
	where TOperation : IOperation<T>
	where T : struct
	where TMemory : IMemory<T>
	where TAttribute : OperationAttribute
	where TInstructionSet : IInstructionSet<TOperation, TAttribute, T> 
{
	public abstract TInstructionSet InstructionSet { get; protected set; }
	public abstract TMemory Memory { get; protected set; }
	public int Delay { get; set; } = 10;
	public List<TOperation> Instructions { get; } = new();
	protected bool _interrupted = false;
	public int InstructionsCount => Instructions.Count;
	public bool IsExecuting { get; protected set; }
	public string[]? Inputs { get; protected set; }
	public int NextInputIndex { get; set; }

	public async Task<string[]> ExecuteAsync(IEnumerable<string> inputs)
	{
		Inputs = inputs.ToArray();
		List<string> output = new();
		string? lastOutput;
		IsExecuting = true;
		try
		{
			while(Memory.InstructionRegister >= 0 && Memory.InstructionRegister < InstructionsCount)
			{
				lastOutput = ExecuteNext();
				if(lastOutput is not null)
				{
					output.Add(lastOutput);
				}

				if(Delay > 0)
				{
					await Task.Delay(Delay);
				}

				if(_interrupted)
				{
					_interrupted = false;
					output.Add("Execution has been interrupted.");
					break;
				}
			}
		}catch
		{
			IsExecuting = false;
			throw;
		}

		IsExecuting = false;
		return output.ToArray();
	}

	public string? ExecuteNext()
	{
		var code = Instructions[Memory.InstructionRegister];
		var output = InstructionSet.Execute(code);
		Memory.InstructionRegister++;
		return output;
	}

	public void Reset()
	{
		_interrupted = false;
		Memory.Clear();
		Instructions.Clear();
		NextInputIndex = 0;
		Inputs = null;
	}

	public async Task StopExecutionAsync()
	{
		if(!IsExecuting)
			return;
		
		_interrupted = true;
		// Wait for the interruption to be applied
		do
		{
			await Task.Delay(10);
		} while(IsExecuting);
	}

	public string[] LoadCode(IEnumerable<string> codeLines)
	{
		int lineNumber = -1;
		TOperation operation;
		List<string> errors = new();
		
		foreach(string codeLine in codeLines)
		{
			lineNumber++;
			if(string.IsNullOrWhiteSpace(codeLine))
				continue;

			try
			{
				operation = ParseCodeLine(codeLine.Trim(), lineNumber);
				Instructions.Add(operation);
			} catch(CommandException ex)
			{
				errors.Add(ex.Message);
			}
		}

		return errors.ToArray();
	}

	public abstract TOperation ParseCodeLine(string codeLine, int lineNumber);
}
