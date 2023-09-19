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

public static class Data
{
    public static IDictionary<LabType, string> LabNames = new Dictionary<LabType, string>()
    {
        {LabType.Lab1, "Метод подстановки"},
        {LabType.Lab2, "Метод перестановки"},
        {LabType.Lab3, "Метод "},
        {LabType.Lab4, "Метод "},
        {LabType.Lab5, "Метод "},
        {LabType.Lab6, "Метод "},
        {LabType.Lab7, "Метод "},
        {LabType.Lab8, "Метод "},
        {LabType.Lab9, "Метод "},
    };
}