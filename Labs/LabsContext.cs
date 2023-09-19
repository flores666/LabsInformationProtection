using lib.Labs;

namespace Laba1;

public interface ILabsContext
{
    public LabType CurrentLabType { get; set; }
}

public class LabsContext : ILabsContext
{
    public LabType CurrentLabType { get; set; }
}
