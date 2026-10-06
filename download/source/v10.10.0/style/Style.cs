namespace codeconv.ansi;

public static class Style
{

    private const char Esc = '\u001b';

    public static string Red(string text) => $"{Esc}[31m{text}{Esc}[39m";
    public static string Green(string text) => $"{Esc}[32m{text}{Esc}[39m";
    public static string Yellow(string text) => $"{Esc}[33m{text}{Esc}[39m";
    public static string Blue(string text) => $"{Esc}[34m{text}{Esc}[39m";
    public static string Magenta(string text) => $"{Esc}[35m{text}{Esc}[39m";
    public static string Cyan(string text) => $"{Esc}[36m{text}{Esc}[39m";
    
    public static string BrightRed(string text) => $"{Esc}[91m{text}{Esc}[39m";
    public static string BrightGreen(string text) => $"{Esc}[92m{text}{Esc}[39m";
    public static string BrightYellow(string text) => $"{Esc}[93m{text}{Esc}[39m";
    public static string BrightBlue(string text) => $"{Esc}[94m{text}{Esc}[39m";
    public static string BrightMagenta(string text) => $"{Esc}[95m{text}{Esc}[39m";
    public static string BrightCyan(string text) => $"{Esc}[96m{text}{Esc}[39m";
    
    public static string BgRed(string text) => $"{Esc}[41m{text}{Esc}[49m";
    public static string BgGreen(string text) => $"{Esc}[42m{text}{Esc}[49m";
    public static string BgYellow(string text) => $"{Esc}[43m{text}{Esc}[49m";
    public static string BgBlue(string text) => $"{Esc}[44m{text}{Esc}[49m";
    public static string BgMagenta(string text) => $"{Esc}[45m{text}{Esc}[49m";
    public static string BgCyan(string text) => $"{Esc}[46m{text}{Esc}[49m";
    
    public static string BgBrightRed(string text) => $"{Esc}[101m{text}{Esc}[49m";
    public static string BgBrightGreen(string text) => $"{Esc}[102m{text}{Esc}[49m";
    public static string BgBrightYellow(string text) => $"{Esc}[103m{text}{Esc}[49m";
    public static string BgBrightBlue(string text) => $"{Esc}[104m{text}{Esc}[49m";

    public static string RgbFg(string text, int r, int g, int b) => $"{Esc}[38;2;{r};{g};{b}m{text}{Esc}[39m";
    public static string RgbBg(string text, int r, int g, int b) => $"{Esc}[48;2;{r};{g};{b}m{text}{Esc}[49m";

    public static string Bold(string text) => $"{Esc}[1m{text}{Esc}[22m";
    public static string Dim(string text) => $"{Esc}[2m{text}{Esc}[22m";
    public static string Underline(string text) => $"{Esc}[4m{text}{Esc}[24m";
}