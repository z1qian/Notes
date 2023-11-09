namespace xUnit.netTestSamples;

public class StringSample
{
    public StringSample(string init)
    {
        // 即使启用了可空引用类型，检查方法参数是否为 null 仍然是一种好做法
        if (init is null)
            throw new ArgumentNullException(nameof(init));
        _init = init;
    }

    private string _init;
    public string GetStringDemo(string first, string second)
    {
        ArgumentNullException.ThrowIfNull(first);
        if (string.IsNullOrEmpty(first)) throw new ArgumentException("empty string is not allowed", first);
        ArgumentNullException.ThrowIfNull(second);
        if (second.Length > first.Length) throw new ArgumentOutOfRangeException(nameof(second),
              "must be shorter than second");

        int startIndex = first.IndexOf(second);
        if (startIndex < 0)
        {
            return $"{second} not found in {first}";
        }
        else if (startIndex < 5)
        {
            string result = first.Remove(startIndex, second.Length);
            return $"removed {second} from {first}: {result}";
        }
        else
        {
            return _init.ToUpperInvariant();
        }
    }
}
