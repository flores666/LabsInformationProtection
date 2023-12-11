using lib.Labs;

namespace Labs;

public interface ILabsContext
{
    public LabType LabType { get; set; }
    public LabProperties LabProperties { get; }
}

public class LabsContext : ILabsContext
{
    public LabType LabType { get; set; } = LabType.Lab1;

    public LabProperties LabProperties
    {
        get
        {
            Data.TryGetLabProperties(LabType, out var value);
            return value;
        }
    }
}
