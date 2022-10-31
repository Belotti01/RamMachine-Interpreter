namespace RamMachineInterpreter.Data;

public class Memory {
	public int this[uint registry] {
		get => ReadMemory(registry);
		set => SetMemory(registry, value);
	}

	public int Size => _memory.Count;

	public int Accumulator = 0;
	protected readonly List<int> _memory = new();

	public int ReadMemory(uint registry)
	{
		if(registry >= _memory.Count)
		{
			// Registry has no assigned value - return default
			return 0;
		}
		return _memory[(int)registry];
	}

	public void SetMemory(uint registry, int value)
	{
		if(registry >= _memory.Count)
		{
			Pan(registry);
		}

		_memory[(int)registry] = value;
	}

	public void Reset()
	{
		Accumulator = 0;
		_memory.Clear();
	}

	protected void Pan(uint toIndex)
	{
		for(int i = _memory.Count; i <= toIndex; i++)
		{
			_memory.Add(0);
		}
	}
}
