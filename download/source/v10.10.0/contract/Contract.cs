namespace codeconv.contract;

public interface IContract
{
    string Name { get; }
    string Version { get; }
    string Description { get; }

    Dictionary<string, string> Commands { get; }


    void Execute(string UserInput);
}
