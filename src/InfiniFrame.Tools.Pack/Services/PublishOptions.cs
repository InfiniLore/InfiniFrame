// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Tools.Pack.Services;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class PublishOptions {
    public required string ProjectPath { get; set; }
    public required string Rid { get; set; }
    public required string Configuration { get; set; }
    public string? Framework { get; set; }
    public required bool SelfContained { get; set; }
    public string? Output { get; set; }
    public bool NoRestore { get; set; }
    public bool Verbose { get; set; }
}
