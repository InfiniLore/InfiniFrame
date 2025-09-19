namespace InfiniLore.Photino.NET;

public interface IPhotinoWindowBase
{
    bool Chromeless { get; }
    bool Transparent { get; }
    bool ContextMenuEnabled { get; }
    bool DevToolsEnabled { get; }
    bool FileSystemAccessEnabled { get; }
    bool WebSecurityEnabled { get; }
}
