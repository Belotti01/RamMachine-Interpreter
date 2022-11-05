namespace RamMachineInterpreter.Data;

public class Memory<T> : IMemory<T>
	where T : struct {

	/// <summary>
	/// Get or Set the value of a registry.
	/// </summary>
	/// <param name="registry">The index of the registry to access.</param>
	/// <returns>The value of the registry at the specified index, or <see langword="default"/> if
	/// no value was assigned to it.</returns>
	public T this[ulong registry]
	{
		get => GetRegistryValue(registry);
		set => SetRegistryValue(registry, value);
	}

	protected readonly Dictionary<ulong, T> _memory = new();

	public T Accumulator { get; set; }
	public ulong Size { get; protected set; } = 0;  // Set manually - more efficient than checking the _memory.Keys everytime

	public int InstructionRegister { get; set; }

	public virtual void Clear()
	{
		Accumulator = default;
		_memory.Clear();
		InstructionRegister = 0;
		Size = 0;
	}

	public virtual T GetRegistryValue(ulong registry)
	{
		if(registry < Size && _memory.TryGetValue(registry, out T value))
		{
			return value;
		}
		// Registry has no assigned value - return default
		return default;
	}

	public virtual bool HasValue(ulong registry)
	{
		return registry < Size
			&& _memory.ContainsKey(registry);
	}

	public virtual void SetRegistryValue(ulong registry, T value)
	{
		if(_memory.TryAdd(registry, value))
		{
			Size = Math.Max(Size, registry + 1);
		} else
		{
			// Registry already exists in _memory
			_memory[registry] = value;
		}
	}
}