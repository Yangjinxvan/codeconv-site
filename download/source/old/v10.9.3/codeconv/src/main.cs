// Logo
/*  ____           _          ____
 * / ___\ ___   __| | ___    / ___\ ___  ____ __    __
 *| |    / _ \/  _  |/ _ \  | |    / _ \|  _ \\ \  / /
 *| |___| (_) | (_| |  __/  | |___| (_) | | | |\ \/ /
 * \____/\___/\___,_|\___/   \____/\___/|_| |_| \__/
 */
using codeconv.contract;
using Serilog;
using Spectre.Console;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using static codeconv.ansi.Style;
namespace codeconv.main;
/// <summary>
/// 全局工具配置类，存储版本、编码映射、内置命令、控制字符枚举
/// </summary>
class ToolConfig
{
    public const string VERSION = "10.9.3";
    public string ConverterMode = "Windows1252";
    public (int Start, int End) CodeRange => ConverterMode == "ASCII" ? (0, 127) : (0, 255);
    public int CODE_MIN => CodeRange.Start;
    public int CODE_MAX => CodeRange.End;
    public readonly string SCRIPT_ROOT = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
    // ProgramData 共享插件根目录
    public readonly string PROGRAM_DATA_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "codeconv");
    public Dictionary<string, IContract> PluginCommands = new Dictionary<string, IContract>();
    public Dictionary<string, string> BuiltinCommands = new Dictionary<string, string>()
    {
        { "编码", "切换到编码模式（字符→编码） 🔤→🔢" },
        { "解码", "切换到解码模式（编码→字符） 🔢→🔤" },
        { "退出", "退出程序 🚪" }
    };
    /// <summary>
    /// ASCII 0~31、127 控制字符枚举
    /// </summary>
    public enum ControlCharNames
    {
        NUL = 0,
        SOH = 1,
        STX = 2,
        ETX = 3,
        EOT = 4,
        ENQ = 5,
        ACK = 6,
        BEL = 7,
        BS = 8,
        HT = 9,
        LF = 10,
        VT = 11,
        FF = 12,
        CR = 13,
        SO = 14,
        SI = 15,
        DLE = 16,
        DC1 = 17,
        DC2 = 18,
        DC3 = 19,
        DC4 = 20,
        NAK = 21,
        SYN = 22,
        ETB = 23,
        CAN = 24,
        EM = 25,
        SUB = 26,
        ESC = 27,
        FS = 28,
        GS = 29,
        RS = 30,
        US = 31,
        DEL = 127
    }
    /// <summary>
    /// Windows1252 字节码 -> Unicode字符映射表
    /// </summary>
    public static readonly Dictionary<byte, string> Windows1252CharMap = new Dictionary<byte, string>
    {
        {128, "\u20AC"}, {129, "[未分配]"}, {130, "\u201A"}, {131, "\u0192"}, {132, "\u201E"}, {133, "\u2026"},
        {134, "\u2020"}, {135, "\u2021"}, {136, "\u02C6"}, {137, "\u2030"}, {138, "\u0160"}, {139, "\u2039"},
        {140, "\u0152"}, {141, "[未分配]"}, {142, "\u017D"}, {143, "[未分配]"}, {144, "[未分配]"}, {145, "\u2018"},
        {146, "\u2019"}, {147, "\u201C"}, {148, "\u201D"}, {149, "\u2022"}, {150, "\u2013"}, {151, "\u2014"},
        {152, "\u02DC"}, {153, "\u2122"}, {154, "\u0161"}, {155, "\u203A"}, {156, "\u0153"}, {157, "[未分配]"},
        {158, "\u017E"}, {159, "\u0178"}, {160, "\u00A0"}, {161, "\u00A1"}, {162, "\u00A2"}, {163, "\u00A3"},
        {164, "\u00A4"}, {165, "\u00A5"}, {166, "\u00A6"}, {167, "\u00A7"}, {168, "\u00A8"}, {169, "\u00A9"},
        {170, "\u00AA"}, {171, "\u00AB"}, {172, "\u00AC"}, {173, "\u00AD"}, {174, "\u00AE"}, {175, "\u00AF"},
        {176, "\u00B0"}, {177, "\u00B1"}, {178, "\u00B2"}, {179, "\u00B3"}, {180, "\u00B4"}, {181, "\u00B5"},
        {182, "\u00B6"}, {183, "\u00B7"}, {184, "\u00B8"}, {185, "\u00B9"}, {186, "\u00BA"}, {187, "\u00BB"},
        {188, "\u00BC"}, {189, "\u00BD"}, {190, "\u00BE"}, {191, "\u00BF"}, {192, "\u00C0"}, {193, "\u00C1"},
        {194, "\u00C2"}, {195, "\u00C3"}, {196, "\u00C4"}, {197, "\u00C5"}, {198, "\u00C6"}, {199, "\u00C7"},
        {200, "\u00C8"}, {201, "\u00C9"}, {202, "\u00CA"}, {203, "\u00CB"}, {204, "\u00CC"}, {205, "\u00CD"},
        {206, "\u00CE"}, {207, "\u00CF"}, {208, "\u00D0"}, {209, "\u00D1"}, {210, "\u00D2"}, {211, "\u00D3"},
        {212, "\u00D4"}, {213, "\u00D5"}, {214, "\u00D6"}, {215, "\u00D7"}, {216, "\u00D8"}, {217, "\u00D9"},
        {218, "\u00DA"}, {219, "\u00DB"}, {220, "\u00DC"}, {221, "\u00DD"}, {222, "\u00DE"}, {223, "\u00DF"},
        {224, "\u00E0"}, {225, "\u00E1"}, {226, "\u00E2"}, {227, "\u00E3"}, {228, "\u00E4"}, {229, "\u00E5"},
        {230, "\u00E6"}, {231, "\u00E7"}, {232, "\u00E8"}, {233, "\u00E9"}, {234, "\u00EA"}, {235, "\u00EB"},
        {236, "\u00EC"}, {237, "\u00ED"}, {238, "\u00EE"}, {239, "\u00EF"}, {240, "\u00F0"}, {241, "\u00F1"},
        {242, "\u00F2"}, {243, "\u00F3"}, {244, "\u00F4"}, {245, "\u00F5"}, {246, "\u00F6"}, {247, "\u00F7"},
        {248, "\u00F8"}, {249, "\u00F9"}, {250, "\u00FA"}, {251, "\u00FB"}, {252, "\u00FC"}, {253, "\u00FD"},
        {254, "\u00FE"}, {255, "\u00FF"}
    };
    private string? _cacheMode;
    private Encoding? _startEncoding;
    private Dictionary<string, int>? _charToCodeCache;
    private string?[]? _controlNames;
    private string[]? _decodeCache;
    private Dictionary<char, string>? _encodeCache;
    /// <summary>
    /// 获取当前模式对应的编码名称，用于Encoding.GetEncoding
    /// </summary>
    public string StartFileEncoding()
    {
        return ConverterMode == "Windows1252" ? "cp1252" : "ascii";
    }
    /// <summary>
    /// 按当前模式一次性构建全部缓存：编码实例、控制字符名表、byte->字符串解码表、char->编码映射、char->编码结果缓存。
    /// 模式一旦切换即整体重建，避免脏缓存。
    /// </summary>
    private void EnsureCache()
    {
        if (_cacheMode == ConverterMode && _decodeCache != null)
            return;
        _cacheMode = ConverterMode;
        _startEncoding = Encoding.GetEncoding(
            StartFileEncoding(),
            new EncoderExceptionFallback(),
            new DecoderExceptionFallback());
        var ctrl = new string?[256];
        foreach (var v in Enum.GetValues<ControlCharNames>())
        {
            ctrl[(int)v] = v.ToString();
        }
        _controlNames = ctrl;
        var cc = new Dictionary<string, int>();
        var dc = new string[256];
        for (int code = 0; code < 256; code++)
        {
            if (code < CODE_MIN || code > CODE_MAX)
            {
                dc[code] = $"[无效:{code}] ❌";
            }
            else if (ctrl[code] != null)
            {
                dc[code] = $"[{ctrl[code]}]";
            }
            else if (ConverterMode == "Windows1252")
            {
                string ch = Windows1252CharMap.TryGetValue((byte)code, out var ms)
                    ? ms
                    : ((char)code).ToString();
                dc[code] = ch;
                if (ch != "[未分配]")
                {
                    cc[ch] = code;
                }
            }
            else
            {
                string ch = ((char)code).ToString();
                dc[code] = ch;
                cc[ch] = code;
            }
        }
        _decodeCache = dc;
        _charToCodeCache = cc;
        _encodeCache = new Dictionary<char, string>();
    }
    /// <summary>
    /// 生成【字符 -> 字节码】反向映射字典，带缓存
    /// </summary>
    public Dictionary<string, int> CharToCode()
    {
        EnsureCache();
        return _charToCodeCache!;
    }
    /// <summary>
    /// 获取当前模式缓存的Encoding实例
    /// </summary>
    public Encoding StartEncoding()
    {
        EnsureCache();
        return _startEncoding!;
    }
    /// <summary>
    /// 查控件字符名，非控件字符返回null；O(1)查表
    /// </summary>
    public string? ControlName(byte code)
    {
        EnsureCache();
        return _controlNames![code];
    }
    /// <summary>
    /// 字节解码为结果字符串（含控制字符标记与无效标记），O(1)查表
    /// </summary>
    public string DecodeByte(byte code)
    {
        EnsureCache();
        return _decodeCache![code];
    }
    /// <summary>
    /// 单字符编码为字节码字符串，带 char->string 缓存
    /// </summary>
    public string EncodeChar(char c)
    {
        EnsureCache();
        if (_encodeCache!.TryGetValue(c, out var r))
            return r;
        string result = _charToCodeCache!.TryGetValue(c.ToString(), out int code)
            ? code.ToString()
            : _startEncoding!.GetBytes(c.ToString())[0].ToString();
        _encodeCache[c] = result;
        return result;
    }
    public HashSet<ControlCharNames> CONTROL_VALUE_SET = new HashSet<ControlCharNames>(Enum.GetValues<ControlCharNames>());
    public HashSet<string> CONTROL_NAME_SET = new HashSet<string>(Enum.GetNames<ControlCharNames>());
}
public static class CrashDumper
{
    [Flags]
    public enum MINIDUMP_TYPE : uint
    {
        MiniDumpNormal = 0x00000000,
        MiniDumpWithDataSegs = 0x00000001,
        MiniDumpWithFullMemory = 0x00000002,
        MiniDumpWithHandleData = 0x00000004,
        MiniDumpWithUnloadedModules = 0x00000020,
        MiniDumpWithProcessThreadData = 0x00000100,
        MiniDumpWithPrivateReadWriteMemory = 0x00000200,
        MiniDumpWithFullMemoryInfo = 0x00000800,
        MiniDumpWithThreadInfo = 0x00001000,
        MiniDumpWithPrivateWriteCopyMemory = 0x00010000
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MINIDUMP_EXCEPTION_INFORMATION
    {
        public uint ThreadId;
        public IntPtr ExceptionPointers;
        [MarshalAs(UnmanagedType.Bool)]
        public bool ClientPointers;
    }
    [DllImport("DbgHelp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool MiniDumpWriteDump(
        IntPtr hProcess,
        int ProcessId,
        IntPtr hFile,
        MINIDUMP_TYPE DumpType,
        IntPtr ExceptionParam,
        IntPtr UserStreamParam,
        IntPtr CallbackParam);
    public static void InitDumpFile()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            try
            {
                string logPath = $"crash_{DateTime.Now:yyyyMMdd_HHmmss}.log";
                string dumpPath = $"crash_{DateTime.Now:yyyyMMdd_HHmmss}.dmp";
                File.WriteAllText(logPath, $"Crash Time:{DateTime.Now}\r\n{e.ExceptionObject}");
                const MINIDUMP_TYPE HeapDumpFlags =
                    MINIDUMP_TYPE.MiniDumpWithDataSegs |
                    MINIDUMP_TYPE.MiniDumpWithProcessThreadData |
                    MINIDUMP_TYPE.MiniDumpWithHandleData |
                    MINIDUMP_TYPE.MiniDumpWithPrivateReadWriteMemory |
                    MINIDUMP_TYPE.MiniDumpWithUnloadedModules |
                    MINIDUMP_TYPE.MiniDumpWithFullMemoryInfo |
                    MINIDUMP_TYPE.MiniDumpWithThreadInfo |
                    MINIDUMP_TYPE.MiniDumpWithPrivateWriteCopyMemory;
                using var fs = File.Create(dumpPath);
                var proc = Process.GetCurrentProcess();
                MiniDumpWriteDump(
                    proc.Handle,
                    proc.Id,
                    fs.SafeFileHandle.DangerousGetHandle(),
                    HeapDumpFlags,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
            catch
            {
                // 静默失败，避免写 dump 过程中二次崩溃
            }
        };
    }
}
/// <summary>
/// 程序入口主类，包含初始化、插件加载、日志初始化、转换逻辑、主循环
/// </summary>
class Main
{
    static ToolConfig Config = new ToolConfig();
    /// <summary>
    /// 统一初始化入口：加载插件 + 初始化Serilog日志
    /// </summary>
    static void Init()
    {
        InitPlugins();
        InitLog();
        CrashDumper.InitDumpFile();
    }
    /// <summary>
    /// 扫描plugins目录，加载所有实现IContract的插件DLL，处理命令冲突
    /// </summary>
    static void InitPlugins()
    {
        Log.Information("加载插件中……");
        List<IContract> pluginList = new List<IContract>();
        // ========== 改动在这里：插件目录切到 ProgramData\codeconv\plugins ==========
        string pluginDir = Path.Combine(Config.PROGRAM_DATA_ROOT, "plugins");
        if (!Directory.Exists(pluginDir))
            Directory.CreateDirectory(pluginDir);
        foreach (string file in Directory.GetFiles(pluginDir, "*.dll"))
        {
            try
            {
                Assembly asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                Type[] allTypes = asm.GetTypes();
                foreach (Type t in allTypes)
                {
                    bool isPlugin = typeof(IContract).IsAssignableFrom(t);
                    if (isPlugin && t.IsClass && !t.IsAbstract && !t.IsInterface)
                    {
                        if (Activator.CreateInstance(t) is not IContract plugin)
                            continue;
                        pluginList.Add(plugin);
                        Dictionary<string, string> cmdDict = plugin.Commands;
                        foreach (var kv in cmdDict)
                        {
                            string cmd = kv.Key;
                            string desc = kv.Value;
                            if (!Config.PluginCommands.ContainsKey(cmd))
                            {
                                Config.PluginCommands[cmd] = plugin;
                            }
                            else
                            {
                                Log.Warning(
                                    "旧插件 {OldPluginName} 与新插件 {NewPluginName} 命令冲突：{CommandName}",
                                    Config.PluginCommands[cmd].Name,
                                    plugin.Name,
                                    cmd);
                                Console.WriteLine("\n" + Yellow($"⚠命令冲突：[{cmd}]"));
                                Console.WriteLine(Cyan($"已注册插件：{Config.PluginCommands[cmd].Name}"));
                                Console.WriteLine(Cyan($"新来插件：{plugin.Name}  描述：{desc}"));
                                Console.WriteLine(Magenta("请选择：1=保留旧插件  2=使用新插件"));
                                string? select = Console.ReadLine();
                                if (select == "2")
                                {
                                    Log.Information("用户选择了使用新插件：{PluginName}", plugin.Name);
                                    Config.PluginCommands[cmd] = plugin;
                                    Console.WriteLine(Green($"✅已切换为：{plugin.Name}"));
                                }
                                else
                                {
                                    Log.Information("用户选择了保留旧插件：{PluginName}", Config.PluginCommands[cmd].Name);
                                    Console.WriteLine(Green($"✅保留原有：{Config.PluginCommands[cmd].Name}"));
                                }
                            }
                        }
                        Log.Information("成功加载插件：{PluginName}", plugin.Name);
                        Console.WriteLine(Green($"✅加载插件 {plugin.Name}"));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "加载插件失败，插件名：{PluginFileName}", Path.GetFileName(file));
                Console.WriteLine(Red($"❌加载失败 {Path.GetFileName(file)}：{ex.Message}"));
            }
        }
    }
    /// <summary>
    /// 初始化Serilog，按天滚动日志文件，限制单文件10MB
    /// </summary>
    static void InitLog()
    {
        // 日志建议也放到ProgramData，和插件统一
        string logFilePath = Path.Combine(Config.PROGRAM_DATA_ROOT, "log", "log_.log");
        string logDir = Path.GetDirectoryName(logFilePath)!;
        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy‑MM‑dd dddd HH:mm:ss.ffffff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10 * 1024 * 1024,
                buffered: false,
                shared: true,
                encoding: new System.Text.UTF8Encoding(true)
            )
            .CreateLogger();
    }
    /// <summary>
    /// 核心转换函数
    /// InputString：输入文本/空格分隔编码串
    /// ToEncode=true：字符→编码；false：编码→字符
    /// </summary>
    static string StringConvert(string InputString, bool ToEncode = false)
    {
        List<string> ResultList = new List<string>();
        if (ToEncode)
        {
            AnsiConsole.Progress()
                .Columns(
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn { Width = 80 },
                    new PercentageColumn()
                )
                .Start(ctx =>
                {
                    var task = ctx.AddTask("编码字符 🔤→🔢");
                    Rune[] runes = InputString.EnumerateRunes().ToArray();
                    task.MaxValue = runes.Length;
                    foreach (Rune rune in runes)
                    {
                        try
                        {
                            string s = rune.ToString();
                            if (Config.CharToCode().TryGetValue(s, out int code))
                            {
                                ResultList.Add(code.ToString());
                            }
                            else
                            {
                                ResultList.Add(Config.StartEncoding().GetBytes(s)[0].ToString());
                            }
                        }
                        catch (EncoderFallbackException)
                        {
                            ResultList.Add($"[不支持:{rune}] ❌");
                        }
                        task.Increment(1);
                    }
                });
            return string.Join(" ", ResultList);
        }
        else
        {
            List<string> CodeStringList = InputString.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            AnsiConsole.Progress()
                .Columns(
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn { Width = 80 },
                    new PercentageColumn()
                )
                .Start(ctx =>
                {
                    var task = ctx.AddTask("解码编码值 🔢→🔤");
                    task.MaxValue = CodeStringList.Count;
                    foreach (var CodeString in CodeStringList)
                    {
                        try
                        {
                            int code = int.Parse(CodeString);
                            if (code < Config.CODE_MIN || code > Config.CODE_MAX)
                            {
                                ResultList.Add($"[无效:{CodeString}] ❌");
                            }
                            else
                            {
                                ResultList.Add(Config.DecodeByte((byte)code));
                            }
                        }
                        catch (FormatException)
                        {
                            ResultList.Add($"[非数字:{CodeString}] ❌");
                        }
                        task.Increment(1);
                    }
                });
            return string.Join(" ", ResultList);
        }
    }
    public static void PressAnyKeyToContinue()
    {
        Console.Write("请按任意键退出...");
        Console.ReadKey(true);
    }
    /// <summary>
    /// 主交互循环，控制台UI、命令解析、输入分发入口
    /// </summary>
    public static void DualModeConverter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        string CurrentConvertMode = "解码";
        Console.Clear();
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine(@" ____           _           ___                     ");
        Console.WriteLine(@"/ ___\ ___   __| | ___     / ___\ ___  ____ __    __");
        Console.WriteLine(@"| |   / _ \/  _  |/ _ \   | |    / _ \|  _ \\ \  / /");
        Console.WriteLine(@"| |__| (_) | (_| |  __/   | |___| (_) | | | |\ \/ / ");
        Console.WriteLine(@"\____/\___/\___,_|\___/    \____/\___/|_| |_| \__/  ");
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine(BrightCyan(Bold($"Windows-1252 / ASCII 转换器 v{ToolConfig.VERSION} 🎉")));
        Console.WriteLine(new string('=', 60));
        Console.WriteLine(Blue($"当前编码模式：{Config.ConverterMode}（有效编码范围：{Config.CODE_MIN}-{Config.CODE_MAX}）🌐"));
        Console.WriteLine(Blue($"使用说明：输入编码值/字符或中文命令（输入 '帮助' 查看所有可用命令）❓"));
        Console.WriteLine(new string('=', 60) + "\n");
        Init();
        Log.Information("程序初始化成功，版本 {Version}", ToolConfig.VERSION);
        try
        {
            while (true)
            {
                string Prompt = Magenta($"{Config.ConverterMode} - {CurrentConvertMode} 请输入内容/命令 📥：");
                Console.Write(Prompt);
                string? UserInput = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(UserInput))
                {
                    Console.WriteLine(Red("错误：输入不能为空 ❌"));
                    Log.Warning("用户输入了空值");
                    continue;
                }
                Log.Information("用户输入内容：{UserInput}，当前转换模式：{ConvertMode}", UserInput, CurrentConvertMode);
                if (Config.BuiltinCommands.ContainsKey(UserInput))
                {
                    switch (UserInput)
                    {
                        case "编码":
                            CurrentConvertMode = "编码";
                            Console.WriteLine(Green("已切换到编码模式（字符→编码）🔤→🔢"));
                            Log.Information("用户将转换模式切换为编码模式");
                            break;
                        case "解码":
                            CurrentConvertMode = "解码";
                            Console.WriteLine(Green("已切换到解码模式（编码→字符）🔢→🔤"));
                            Log.Information("用户将转换模式切换为解码模式");
                            break;
                        case "退出":
                            Console.WriteLine(BrightGreen("感谢使用本转换器，再见！👋"));
                            Log.Information("用户触发退出命令，准备关闭程序");
                            PressAnyKeyToContinue();
                            Log.CloseAndFlush();
                            Environment.Exit(0);
                            break;
                    }
                }
                else if (Config.PluginCommands.ContainsKey(UserInput))
                {
                    var targetPlugin = Config.PluginCommands[UserInput];
                    targetPlugin.Execute(UserInput);
                    Log.Information(
                        "用户执行插件命令，插件名称：{PluginName}，命令名称：{PluginCommandName}",
                        targetPlugin.Name,
                        UserInput);
                }
                else
                {
                    if (CurrentConvertMode == "解码")
                    {
                        if (UserInput.Contains(' '))
                        {
                            string Result = StringConvert(UserInput, ToEncode: false);
                            Console.WriteLine(Blue($"\n解码结果：{UserInput} → {BrightGreen(Result)} 🎉\n"));
                            Log.Information("批量解码完成，输入：{Input}，输出：{Output}", UserInput, Result);
                        }
                        else
                        {
                            if (!int.TryParse(UserInput, out int WinCodeRaw))
                            {
                                Console.WriteLine(Red("错误：不是有效数字 ❌"));
                                Log.Warning("解码输入不是数字，输入内容：{Input}", UserInput);
                                continue;
                            }
                            if (!(Config.CODE_MIN <= WinCodeRaw && WinCodeRaw <= Config.CODE_MAX))
                            {
                                Console.WriteLine(Yellow($"警告：编码值超出范围（{Config.CODE_MIN}-{Config.CODE_MAX}）⚠️"));
                                Log.Warning("解码数字超出编码范围，输入值：{Code}", WinCodeRaw);
                                continue;
                            }
                            byte WinCode = (byte)WinCodeRaw;
                            string? ctrlName = Config.ControlName(WinCode);
                            if (ctrlName != null)
                            {
                                string Result = $"[控制字符:{ctrlName}] 🎛️";
                                Console.WriteLine(Blue($"\n解码结果：{UserInput} → {BrightGreen(Result)} 🎉\n"));
                                Log.Information("单值解码（控制字符），输入：{Input}，结果：{Result}", UserInput, Result);
                            }
                            else
                            {
                                string Result = Config.ConverterMode == "Windows1252"
                                    ? ToolConfig.Windows1252CharMap.GetValueOrDefault(WinCode, ((char)WinCode).ToString())
                                    : ((char)WinCode).ToString();
                                Console.WriteLine(Blue($"\n解码结果：{UserInput} → {BrightGreen(Result)} 🎉\n"));
                                Log.Information("单值解码普通字符，输入：{Input}，结果：{Result}", UserInput, Result);
                            }
                        }
                    }
                    else
                    {
                        string Result = StringConvert(UserInput, ToEncode: true);
                        Console.WriteLine(Blue($"\n编码结果：{UserInput} → {BrightGreen(Result)} 🎉\n"));
                        Log.Information("字符编码完成，输入：{Input}，输出：{Output}", UserInput, Result);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(Red($"错误：程序异常 ❌ - {e.Message}"));
            Console.WriteLine(e);
            Log.Error(e, "程序顶层捕获未处理异常");
            PressAnyKeyToContinue();
        }
    }
}
