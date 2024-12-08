using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class CustomEMA : Indicator
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Periods", DefaultValue = 200)]
        public int Periods { get; set; }

        [Output("Main", LineColor = "Blue", PlotType = PlotType.Line)]
        public IndicatorDataSeries Result { get; set; }

        private double exp;

        protected override void Initialize()
        {
            exp = 2.0 / (Periods + 1);
        }

        public override void Calculate(int index)
        {
            var previousValue = Result[index - 1];

            if (double.IsNaN(previousValue))
                Result[index] = Source[index];
            else
                Result[index] = Source[index] * exp + previousValue * (1 - exp);
        }
    }
}