// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Tools.Pack.Services;

namespace InfiniFrame.Tools.Pack;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal static class Program {
    public static async Task<int> Main(string[] args) {
        try {
            ParseResult parse = CommandLine.Parse(args);
            if (!parse.ShowUsage) return await PublishService.PublishAsync(parse.Options!);

            CommandLine.PrintUsage();
            return parse.ExitCode;

        }
        catch (Exception ex) {
            await Console.Error.WriteLineAsync($"[InfiniFrame.Pack] ERROR: {ex.Message}");
            return 1;
        }
    }
}
