using System;
using NUnit.Framework;
using cAlgo.API;
using cAlgo.RiskManagement;

namespace cAlgo.Tests
{
    [TestFixture]
    public class PositionSizingTests
    {
        private PositionSizing positionSizing;
        private double riskAmount = 1000; // $1000 risk per trade
        private Symbol mockSymbol;

        [SetUp]
        public void Setup()
        {
            // Create mock symbol with known values
            mockSymbol = new MockSymbol
            {
                PipValue = 10, // $10 per pip
                VolumeInUnitsStep = 1000, // Standard lot size step
                VolumeInUnitsMin = 1000,  // Minimum lot size
                VolumeInUnitsMax = 100000 // Maximum lot size
            };

            positionSizing = new PositionSizing(mockSymbol, riskAmount);
        }

        [Test]
        public void CalculatePositionSize_StandardScenario_CalculatesCorrectly()
        {
            // Arrange
            var stopLossInPips = 10; // 10 pips stop loss
            var expectedSize = 10000; // Expected position size for $1000 risk

            // Act
            var result = positionSizing.CalculatePositionSize(stopLossInPips);

            // Assert
            Assert.AreEqual(expectedSize, result);
        }

        [Test]
        public void CalculatePositionSize_SmallStopLoss_RoundsToStepSize()
        {
            // Arrange
            var stopLossInPips = 5; // 5 pips stop loss
            var expectedSize = 20000; // Expected position size for $1000 risk

            // Act
            var result = positionSizing.CalculatePositionSize(stopLossInPips);

            // Assert
            Assert.AreEqual(expectedSize, result);
            Assert.IsTrue(result % mockSymbol.VolumeInUnitsStep == 0);
        }

        [Test]
        public void CalculatePositionSize_LargeStopLoss_RespectsMinimumSize()
        {
            // Arrange
            var stopLossInPips = 100; // 100 pips stop loss
            var result = positionSizing.CalculatePositionSize(stopLossInPips);

            // Assert
            Assert.GreaterOrEqual(result, mockSymbol.VolumeInUnitsMin);
        }

        [Test]
        public void CalculatePositionSize_SmallStopLoss_RespectsMaximumSize()
        {
            // Arrange
            var stopLossInPips = 0.1; // Very small stop loss
            var result = positionSizing.CalculatePositionSize(stopLossInPips);

            // Assert
            Assert.LessOrEqual(result, mockSymbol.VolumeInUnitsMax);
        }

        [Test]
        public void CalculatePositionSize_InvalidStopLoss_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => positionSizing.CalculatePositionSize(0));
            Assert.Throws<ArgumentException>(() => positionSizing.CalculatePositionSize(-1));
        }

        [Test]
        public void CalculatePositionSize_DifferentRiskAmounts_ScalesCorrectly()
        {
            // Arrange
            var standardRisk = new PositionSizing(mockSymbol, 1000);
            var doubleRisk = new PositionSizing(mockSymbol, 2000);
            var stopLossInPips = 10;

            // Act
            var standardSize = standardRisk.CalculatePositionSize(stopLossInPips);
            var doubleSize = doubleRisk.CalculatePositionSize(stopLossInPips);

            // Assert
            Assert.AreEqual(standardSize * 2, doubleSize);
        }

        [Test]
        public void Constructor_InvalidRiskAmount_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new PositionSizing(mockSymbol, 0));
            Assert.Throws<ArgumentException>(() => new PositionSizing(mockSymbol, -1000));
        }

        private class MockSymbol : Symbol
        {
            public override double PipValue { get; set; }
            public override double VolumeInUnitsStep { get; set; }
            public override double VolumeInUnitsMin { get; set; }
            public override double VolumeInUnitsMax { get; set; }
        }
    }
}