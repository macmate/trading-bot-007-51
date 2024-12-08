using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.RiskManagement;
using cAlgo.TimeManagement;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TradingBot007_51 : Robot
    {
        #region Parameters

        [Parameter("Strategy Mode", DefaultValue = "Both")]
        public string StrategyMode { get; set; }

        [Parameter("Use EMA Filter", DefaultValue = true)]
        public bool UseEmaFilter { get; set; }

        [Parameter("Move to Break Even", DefaultValue = true)]
        public bool MoveToBreakEven { get; set; }

        [Parameter("Risk Amount ($)", DefaultValue = 1000)]
        public double RiskAmount { get; set; }

        [Parameter("EMA Period", DefaultValue = 200)]
        public int EmaPeriod { get; set; }

        [Parameter("Time Zone Offset", DefaultValue = -4)]
        public int TimeZoneOffset { get; set; }

        // 007 Golden Eye Parameters
        [Parameter("007 Enabled", Group = "Golden Eye", DefaultValue = true)]
        public bool GoldenEyeEnabled { get; set; }

        [Parameter("007 Start Hour (EST)", Group = "Golden Eye", DefaultValue = 16)]
        public int GoldenEyeStartHour { get; set; }

        [Parameter("007 End Hour (EST)", Group = "Golden Eye", DefaultValue = 19)]
        public int GoldenEyeEndHour { get; set; }

        // Area 51 Parameters
        [Parameter("Area 51 Enabled", Group = "Area 51", DefaultValue = true)]
        public bool Area51Enabled { get; set; }

        [Parameter("Area 51 Start Hour (EST)", Group = "Area 51", DefaultValue = 0)]
        public int Area51StartHour { get; set; }

        [Parameter("Area 51 End Hour (EST)", Group = "Area 51", DefaultValue = 2)]
        public int Area51EndHour { get; set; }

        #endregion

        #region Instance Variables
        private MovingAverage ema;
        private SessionManager goldenEyeSession;
        private SessionManager area51Session;
        private PositionSizing positionSizing;
        private double goldenEyeRangeHigh;
        private double goldenEyeRangeLow;
        private double area51RangeHigh;
        private double area51RangeLow;
        private bool goldenEyeRangeSet;
        private bool area51RangeSet;
        private const string GoldenEyeLabel = "GoldenEye";
        private const string Area51Label = "Area51";
        #endregion

        protected override void OnStart()
        {
            // Initialize components
            if (UseEmaFilter)
            {
                ema = Indicators.MovingAverage(Bars.ClosePrices, EmaPeriod, MovingAverageType.Exponential);
            }

            goldenEyeSession = new SessionManager(GoldenEyeStartHour, GoldenEyeEndHour, TimeZoneOffset);
            area51Session = new SessionManager(Area51StartHour, Area51EndHour, TimeZoneOffset);
            positionSizing = new PositionSizing(Symbol, RiskAmount);

            // Reset variables
            ResetRangeVariables();
        }

        protected override void OnTick()
        {
            // Handle Golden Eye strategy
            if (GoldenEyeEnabled)
            {
                HandleGoldenEyeStrategy();
            }

            // Handle Area 51 strategy
            if (Area51Enabled)
            {
                HandleArea51Strategy();
            }

            // Manage existing positions
            ManagePositions();
        }

        private void HandleGoldenEyeStrategy()
        {
            if (goldenEyeSession.IsWithinSession(Server.Time))
            {
                if (!goldenEyeRangeSet)
                {
                    CalculateGoldenEyeRange();
                }
            }
            else if (goldenEyeRangeSet)
            {
                CheckForBreakout(GoldenEyeLabel, goldenEyeRangeHigh, goldenEyeRangeLow);
            }
        }

        private void HandleArea51Strategy()
        {
            if (area51Session.IsWithinSession(Server.Time))
            {
                if (!area51RangeSet)
                {
                    CalculateArea51Range();
                }
            }
            else if (area51RangeSet)
            {
                CheckForBreakout(Area51Label, area51RangeHigh, area51RangeLow);
            }
        }

        private void CalculateGoldenEyeRange()
        {
            var lastCompletedBar = Bars.Count - 2;
            var sessionBars = 0;
            var high = double.MinValue;
            var low = double.MaxValue;

            // Look back through bars to find session range
            for (int i = lastCompletedBar; i >= 0; i--)
            {
                var barTime = Bars.OpenTimes[i];
                if (!goldenEyeSession.IsWithinSession(barTime))
                    break;

                high = Math.Max(high, Bars.HighPrices[i]);
                low = Math.Min(low, Bars.LowPrices[i]);
                sessionBars++;
            }

            if (sessionBars > 0)
            {
                goldenEyeRangeHigh = high;
                goldenEyeRangeLow = low;
                goldenEyeRangeSet = true;
            }
        }

        private void CalculateArea51Range()
        {
            var lastCompletedBar = Bars.Count - 2;
            var sessionBars = 0;
            var high = double.MinValue;
            var low = double.MaxValue;

            // Look back through bars to find session range
            for (int i = lastCompletedBar; i >= 0; i--)
            {
                var barTime = Bars.OpenTimes[i];
                if (!area51Session.IsWithinSession(barTime))
                    break;

                high = Math.Max(high, Bars.HighPrices[i]);
                low = Math.Min(low, Bars.LowPrices[i]);
                sessionBars++;
            }

            if (sessionBars > 0)
            {
                area51RangeHigh = high;
                area51RangeLow = low;
                area51RangeSet = true;
            }
        }

        private void CheckForBreakout(string label, double high, double low)
        {
            if (!UseEmaFilter || ValidateEmaDirection())
            {
                var range = high - low;
                var stopLoss = range * 0.5;
                var takeProfit = range * 3;

                // Check for long setup
                if (Symbol.Ask > high && !HasPosition(label))
                {
                    ExecuteMarketOrder(TradeType.Buy, Symbol.Name, CalculatePositionSize(stopLoss), label, stopLoss, takeProfit);
                }
                // Check for short setup
                else if (Symbol.Bid < low && !HasPosition(label))
                {
                    ExecuteMarketOrder(TradeType.Sell, Symbol.Name, CalculatePositionSize(stopLoss), label, stopLoss, takeProfit);
                }
            }
        }

        private bool ValidateEmaDirection()
        {
            var currentClose = Bars.ClosePrices.Last(1);
            var currentEma = ema.Result.Last(1);
            
            return currentClose > currentEma; // Validates trend direction
        }

        private double CalculatePositionSize(double stopLossPips)
        {
            return positionSizing.CalculatePositionSize(stopLossPips);
        }

        private bool HasPosition(string label)
        {
            return Positions.Find(label, Symbol.Name) != null;
        }

        private void ManagePositions()
        {
            if (!MoveToBreakEven) return;

            foreach (var position in Positions)
            {
                if (position.Pips >= position.StopLoss * 2) // Move to break even at 1R
                {
                    ModifyPosition(position, position.EntryPrice, position.TakeProfit);
                }
            }
        }

        private void ResetRangeVariables()
        {
            goldenEyeRangeHigh = 0;
            goldenEyeRangeLow = 0;
            area51RangeHigh = 0;
            area51RangeLow = 0;
            goldenEyeRangeSet = false;
            area51RangeSet = false;
        }

        protected override void OnStop()
        {
            // Cleanup code if needed
        }
    }
}