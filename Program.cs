using tests;

public class Program
{
    public static void Main(string[] args)
    {
        // Test cases for SerializeInt
        var testInt = new TestInt();

        testInt.SerializeInt(0, "0");
        testInt.SerializeInt(42, "42");
        testInt.SerializeInt(-25, "-25");
        testInt.SerializeInt(int.MaxValue, "2147483647");
        testInt.SerializeInt(int.MinValue, "-2147483648");

    }
}