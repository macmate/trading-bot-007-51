using System;
using cAlgo.API;

namespace cAlgo.RiskManagement
{
    public class PositionSizing
    {
        private readonly Symbol symbol;
        private readonly double riskAmount;

        public PositionSizing(Symbol symbol, double riskAmount)
        {
            this.symbol = symbol;
            this.riskAmount = riskAmount;
        }

        public double CalculatePositionSize(double stopLossInPips)
        {
            if (stopLossInPips <= 0)
                throw new ArgumentException("Stop loss must be greater than zero");

            var pipValue = symbol.PipValue;
            var positionSize = (riskAmount / (stopLossInPips * pipValue));
            
            // Round to symbol volume step
            positionSize = Math.Floor(positionSize / symbol.VolumeInUnitsStep) * symbol.VolumeInUnitsStep;
            
            return positionSize;
        }
    }
}