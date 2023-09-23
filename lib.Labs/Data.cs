namespace lib.Labs;

public enum LabType
{
    Lab1,
    Lab2,
    Lab3,
    Lab4,
    Lab5,
    Lab6,
    Lab7,
    Lab8,
    Lab9
}

public class LabProperties
{
    public string Name { get; }
    public string Alphabet { get; }
    public int TextBlockLength { get; }
    public int KeyLength { get; }
    public string KeyPattern { get; set; }
    public int FuncValuesCount { get; }

    public LabProperties(string name, string alphabet, int textBlockLength, string keyPattern)
    {
        Name = name;
        Alphabet = alphabet;
        KeyLength = TextBlockLength = textBlockLength;
        FuncValuesCount = (int)Math.Pow(Alphabet.Length, KeyLength);
        KeyPattern = keyPattern;
    }
}

public static class Data
{
    public static int LabsCount => _labProps.Count;

    private static IDictionary<LabType, Func<LabProperties>> _labProps = new Dictionary<LabType, Func<LabProperties>>()
    {
        [LabType.Lab1] = () => new("Метод подстановки", "012345", 2, "[0-5]"),
        [LabType.Lab2] = () => new("Метод перестановки", "0123456789ABCDEF", 16, "[0-9,]"),
        [LabType.Lab3] = () => new("?", "?", 0, ""),
    };

    public static bool TryGetLabProperties(LabType lab, out LabProperties props)
    {
        if (_labProps.TryGetValue(lab, out var value))
        {
            props = value.Invoke();
            return true;
        }

        props = null;
        return false;
    }
}