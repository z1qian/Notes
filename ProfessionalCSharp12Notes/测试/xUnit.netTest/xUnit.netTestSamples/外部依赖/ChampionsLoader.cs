using System.Xml.Linq;

namespace xUnit.netTestSamples.外部依赖;

public class ChampionsLoader : IChampionsLoader
{
    public XElement LoadChampions() => XElement.Load(F1Addresses.RacersUrl);
}
