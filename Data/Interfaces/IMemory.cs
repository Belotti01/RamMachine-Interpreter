namespace RamMachineInterpreter.Data;

public interface IMemory<T> {
	/// <summary>
	/// Get or Set the value of a registry.
	/// </summary>
	/// <param name="registry">The index of the registry to access.</param>
	/// <returns>The value of the registry at the specified index, or <see langword="default"/> if
	/// no value was assigned to it.</returns>
	public T this[ulong registry] {
		get => GetRegistryValue(registry);
		set => SetRegistryValue(registry, value);
	}
	
	public int InstructionRegister { get; set;  }
	public T Accumulator { get; set; }
	/// <summary>
	/// The highest occupied memory index.
	/// </summary>
	public ulong Size { get; }

	/// <summary>
	/// Get the value of a registry.
	/// </summary>
	/// <inheritdoc cref="this[ulong]"/>
	public T GetRegistryValue(ulong registry);
	/// <summary>
	/// Set the value of a registry.
	/// </summary>
	/// <param name="value">The value to assign to the registry.</param>
	/// <returns></returns>
	/// <inheritdoc cref="this[ulong]"/>
	public void SetRegistryValue(ulong registry, T value);
	/// <summary>
	/// Reset the memory to its initial state.
	/// </summary>
	public void Clear();
	/// <summary>
	/// Check if a registry has a value assigned to it.
	/// </summary>
	/// <param name="registry">The index of the registry to check.</param>
	/// <returns><see langword="true"/> if the registry has been assigned a value (even if 
	/// it's <see langword="default"/>), <see langword="false"/> otherwise.</returns>
	public bool HasValue(ulong registry);
}
