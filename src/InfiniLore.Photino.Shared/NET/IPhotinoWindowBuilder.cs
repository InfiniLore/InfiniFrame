namespace InfiniLore.Photino.NET;

public interface IPhotinoWindowBuilder : IPhotinoWindowBase
{
    PhotinoNativeParameters StartupParameters { get; }
    
    bool Centered { get; set; }
    new bool Chromeless { get; set; }
    new bool Transparent { get; set; }
    new bool ContextMenuEnabled { get; set; }
    new bool DevToolsEnabled { get; set; }
    bool MediaAutoplayEnabled { get; set; }
    string UserAgent { get; set; }
    new bool FileSystemAccessEnabled { get; set; }
    new bool WebSecurityEnabled { get; set; }
}
