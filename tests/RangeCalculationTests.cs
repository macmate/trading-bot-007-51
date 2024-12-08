using System;
using NUnit.Framework;
using cAlgo.API;
using cAlgo.Tests.Mocks;
using cAlgo.Tests.TestHelpers;

namespace cAlgo.Tests
{
    [TestFixture]
    public class RangeCalculationTests
    {
        private MockMarket mockMarket;
        private readonly DateTime sessionStart = new DateTime(2024, 12, 8, 16, 0, 0); // 4 PM EST

        [SetUp]
        public void Setup()
        {
            mockMarket = new MockMarket();
            SetupTestData();
        }

        [Test]
        public void CalculateRange_GoldenEyeSession_IdentifiesCorrectRange()
        {
            // Arrange
            var expectedHigh = 2320.5;
            var expectedLow = 2315.5;

            // Act
            var range = mockMarket.CalculateSessionRange(sessionStart, sessionStart.AddHours(3));

            // Assert
            Assert.AreEqual(expectedHigh, range.High);
            Assert.AreEqual(expectedLow, range.Low);
        }

        [Test]
        public void CalculateRange_NoDataInSession_ReturnsNull()
        {
            // Arrange
            var emptySession = sessionStart.AddDays(1);

            // Act
            var range = mockMarket.CalculateSessionRange(emptySession, emptySession.AddHours(3));

            // Assert
            Assert.IsNull(range);
        }

        [Test]
        public void CalculateRange_SingleBar_CalculatesCorrectly()
        {
            // Arrange
            mockMarket.AddBar(sessionStart, 2317.5, 2318.0, 2317.0, 2317.8);

            // Act
            var range = mockMarket.CalculateSessionRange(sessionStart, sessionStart.AddMinutes(30));

            // Assert
            Assert.AreEqual(2318.0, range.High);
            Assert.AreEqual(2317.0, range.Low);
        }

        [Test]
        public void CalculateRange_OutsideSession_ReturnsNull()
        {
            // Act
            var range = mockMarket.CalculateSessionRange(
                sessionStart.AddHours(4),  // Outside Golden Eye session
                sessionStart.AddHours(7));

            // Assert
            Assert.IsNull(range);
        }

        [Test]
        public void CalculateRange_PartialSession_CalculatesCorrectly()
        {
            // Act
            var range = mockMarket.CalculateSessionRange(
                sessionStart.AddMinutes(30),  // Start 30 minutes into session
                sessionStart.AddHours(2));     // End 1 hour early

            // Assert
            Assert.IsNotNull(range);
            Assert.Greater(range.High, range.Low);
        }

        [Test]
        public void ValidateRange_MinimumRange_ReturnsValid()
        {
            // Arrange
            const double minimumRange = 0.5; // Minimum range in points
            mockMarket.ClearBars();
            mockMarket.AddBar(sessionStart, 2317.0, 2317.6, 2317.0, 2317.5); // Range = 0.6

            // Act
            var range = mockMarket.CalculateSessionRange(sessionStart, sessionStart.AddMinutes(30));
            var isValid = mockMarket.IsValidRange(range, minimumRange);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void ValidateRange_BelowMinimum_ReturnsInvalid()
        {
            // Arrange
            const double minimumRange = 1.0;
            mockMarket.ClearBars();
            mockMarket.AddBar(sessionStart, 2317.0, 2317.6, 2317.0, 2317.5); // Range = 0.6

            // Act
            var range = mockMarket.CalculateSessionRange(sessionStart, sessionStart.AddMinutes(30));
            var isValid = mockMarket.IsValidRange(range, minimumRange);

            // Assert
            Assert.IsFalse(isValid);
        }

        private void SetupTestData()
        {
            // Add sample bars for a typical Golden Eye session
            mockMarket.AddBar(sessionStart, 2317.5, 2318.0, 2315.5, 2317.6);
            mockMarket.AddBar(sessionStart.AddMinutes(30), 2317.6, 2319.5, 2317.2, 2318.6);
            mockMarket.AddBar(sessionStart.AddMinutes(60), 2318.6, 2319.7, 2318.2, 2319.1);
            mockMarket.AddBar(sessionStart.AddMinutes(90), 2319.0, 2320.5, 2318.8, 2319.0);
            mockMarket.AddBar(sessionStart.AddMinutes(120), 2319.1, 2320.0, 2318.5, 2319.4);
            mockMarket.AddBar(sessionStart.AddMinutes(150), 2319.3, 2319.8, 2318.9, 2319.7);
        }
    }
}