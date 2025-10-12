namespace InfiniLore.Photino.Blazor;
public class PhotinoEnvironment : IPhotinoEnvironment
{
    public bool IsDesktop => true;
        
    public string Platform => 
        OperatingSystem.IsWindows() ? "Windows" :
        OperatingSystem.IsMacOS() ? "macOS" :
        OperatingSystem.IsLinux() ? "Linux" :
        "Unknown";
}
