using System;
using NUnit.Framework;
using cAlgo.API;
using cAlgo.RiskManagement;
using cAlgo.Tests.Mocks;
using cAlgo.Tests.TestHelpers;

namespace cAlgo.Tests
{
    [TestFixture]
    public class PositionSizingTests
    {
        private PositionSizing positionSizing;
        private Symbol mockSymbol;

        [SetUp]
        public void Setup()
        {
            mockSymbol = new MockSymbol
            {
                PipValue = TestConstants.DefaultPipValue,
                VolumeInUnitsStep = TestConstants.DefaultVolumeStep,
                VolumeInUnitsMin = TestConstants.MinVolume,
                VolumeInUnitsMax = TestConstants.MaxVolume
            };

            positionSizing = new PositionSizing(mockSymbol, TestConstants.DefaultRiskAmount);
        }

        [Test]
        public void CalculatePositionSize_StandardScenario_CalculatesCorrectly()
        {
            var stopLossInPips = 10;
            var expectedSize = 10000;

            var result = positionSizing.CalculatePositionSize(stopLossInPips);

            Assert.AreEqual(expectedSize, result);
        }

        [Test]
        public void CalculatePositionSize_SmallStopLoss_RoundsToStepSize()
        {
            var stopLossInPips = 5;
            var expectedSize = 20000;

            var result = positionSizing.CalculatePositionSize(stopLossInPips);

            Assert.AreEqual(expectedSize, result);
            Assert.IsTrue(result % mockSymbol.VolumeInUnitsStep == 0);
        }

        [Test]
        public void CalculatePositionSize_LargeStopLoss_RespectsMinimumSize()
        {
            var stopLossInPips = 100;
            var result = positionSizing.CalculatePositionSize(stopLossInPips);

            Assert.GreaterOrEqual(result, mockSymbol.VolumeInUnitsMin);
        }

        [Test]
        public void CalculatePositionSize_SmallStopLoss_RespectsMaximumSize()
        {
            var stopLossInPips = 0.1;
            var result = positionSizing.CalculatePositionSize(stopLossInPips);

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
            var standardRisk = new PositionSizing(mockSymbol, TestConstants.DefaultRiskAmount);
            var doubleRisk = new PositionSizing(mockSymbol, TestConstants.DefaultRiskAmount * 2);
            var stopLossInPips = 10;

            var standardSize = standardRisk.CalculatePositionSize(stopLossInPips);
            var doubleSize = doubleRisk.CalculatePositionSize(stopLossInPips);

            Assert.AreEqual(standardSize * 2, doubleSize);
        }

        [Test]
        public void Constructor_InvalidRiskAmount_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new PositionSizing(mockSymbol, 0));
            Assert.Throws<ArgumentException>(() => new PositionSizing(mockSymbol, -1000));
        }
    }
}