using System.Xml.Linq;

namespace xUnit.netTestSamples.外部依赖;

public interface IChampionsLoader
{
    XElement LoadChampions();
}
