namespace InfiniLore.Photino.Blazor;
public interface IPhotinoEnvironment
{
    bool IsDesktop { get; }
    string Platform { get; }
}