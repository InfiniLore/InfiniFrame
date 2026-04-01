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

            // ReSharper disable once InvertIf
            if (parse.ShowUsage) {
                CommandLine.PrintUsage();
                return parse.ExitCode;
            }

            return await PublishService.PublishAsync(parse.Options);

        }
        catch (Exception ex) {
            await Console.Error.WriteLineAsync($"[InfiniFrame.Pack] ERROR: {ex.Message}");
            return 1;
        }
    }
}
