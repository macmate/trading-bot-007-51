using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.Tests.Mocks
{
    public class MockMarket
    {
        private List<Bar> bars;

        public MockMarket()
        {
            bars = new List<Bar>();
        }

        public void AddBar(DateTime time, double open, double high, double low, double close)
        {
            bars.Add(new Bar { Time = time, Open = open, High = high, Low = low, Close = close });
        }

        public void ClearBars()
        {
            bars.Clear();
        }

        public Range CalculateSessionRange(DateTime start, DateTime end)
        {
            var sessionBars = bars.Where(b => b.Time >= start && b.Time < end).ToList();

            if (!sessionBars.Any())
                return null;

            return new Range
            {
                High = sessionBars.Max(b => b.High),
                Low = sessionBars.Min(b => b.Low)
            };
        }

        public bool IsValidRange(Range range, double minimumRange)
        {
            if (range == null)
                return false;

            return (range.High - range.Low) >= minimumRange;
        }
    }

    public class Bar
    {
        public DateTime Time { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
    }

    public class Range
    {
        public double High { get; set; }
        public double Low { get; set; }
    }
}