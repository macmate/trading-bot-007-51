using cAlgo.API;

namespace cAlgo.Tests.Mocks
{
    public class MockSymbol : Symbol
    {
        public override double PipValue { get; set; }
        public override double VolumeInUnitsStep { get; set; }
        public override double VolumeInUnitsMin { get; set; }
        public override double VolumeInUnitsMax { get; set; }
    }
}