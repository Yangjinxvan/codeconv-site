using codeconv.contract;

namespace codeconv.plugin.sample;

public class SamplePlugin : IContract
{
    public string Name => "示例插件";
    public string Version => "1.0.0";
    public string Description => "最小占位示例插件";

    public Dictionary<string, string> Commands => new Dictionary<string, string>
    {
        { "示例", "打印示例插件说明" }
    };

    public void Execute(string UserInput)
    {
        Console.WriteLine("这是一个最小占位示例插件，命令：示例。");
    }
}
