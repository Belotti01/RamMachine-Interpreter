namespace RamMachineInterpreter.Data;

public interface IInterpreter
{
    public int Delay { get; set; }
    public int NextInstruction { get; }
    public int InstructionsCount { get; }


    public void ParseCode(IEnumerable<string> codeLines);
    public string[] Execute();
    public string? ExecuteNext();
    public Task<string[]> ExecuteAsync();
}
