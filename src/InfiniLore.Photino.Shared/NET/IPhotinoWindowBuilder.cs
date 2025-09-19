namespace InfiniLore.Photino.NET;

public interface IPhotinoWindowBuilder : IPhotinoWindowBase
{
    bool Centered { get; set; }
    new bool Chromeless { get; set; }
    new bool Transparent { get; set; }
}
