using lib.Labs;

namespace Laba1;

public interface ILabsContext
{
    public LabType LabType { get; set; }
    public LabProperties LabProperties { get; }
}

public class LabsContext : ILabsContext
{
    public LabType LabType { get; set; }

    public LabProperties LabProperties
    {
        get
        {
            Data.TryGetLabProperties(LabType, out var value);
            return value;
        }
    }
}
