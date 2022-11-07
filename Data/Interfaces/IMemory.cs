using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RamMachineInterpreter.Data;

public interface IMemory<T> {

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

	/// <summary>
	/// Get or Set the value of a registry identified by its name.
	/// </summary>
	/// <param name="registryName">The name of the registry to access.</param>
	/// <returns>The value of the registry named <paramref name="registryName"/>, or <see langword="default"/> if
	/// it does not exist.</returns>
	public T? this[string registryName]
	{
		get
		{
			TryGetRegistryValue(registryName, out T? value);
			return value;
		}
		set => TrySetRegistryValue(registryName, value);
	}

	public int InstructionRegister { get; set; }
	public T Accumulator { get; set; }
	public ReadOnlyDictionary<string, AccessorSet<T>> NamedRegistriesAccessors { get; set; }

	/// <summary>
	/// The highest occupied memory index.
	/// </summary>
	public ulong Size { get; }

	/// <summary>
	/// Check if a registry with the name <paramref name="registryName"/> exists.
	/// </summary>
	/// <param name="registryName">The name of the registry to find.</param>
	/// <returns><see langword="true"/> if a registry named <paramref name="registryName"/> exists,
	/// <see langword="false"/> otherwise.</returns>
	public bool HasRegistry(string registryName);

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
	/// Attempt to read the value of a registry identified by its name.
	/// </summary>
	/// <param name="registryName">The name of the registry to read.</param>
	/// <param name="value">The value of the registry, or <see langword="default"/> if it doesn not exist.</param>
	/// <returns><see langword="true"/> if the value of the registry was found, or 
	/// <see langword="false"/> if the registry does not exist.</returns>
	public bool TryGetRegistryValue(string registryName, [NotNullWhen(true)] out T? value);

	/// <summary>
	/// Attempt to overwrite the value of a registry identified by its name.
	/// </summary>
	/// <param name="registryName">The name of the registry to overwrite.</param>
	/// <param name="value">The value to set the registry to.</param>
	/// <returns><see langword="true"/> if the value of the registry has been overwritten, or 
	/// <see langword="false"/> if the registry does not exist.</returns>
	public bool TrySetRegistryValue(string registryName, T? value);
	
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