using System;
using NUnit.Framework;
using cAlgo.TimeManagement;

namespace cAlgo.Tests
{
    [TestFixture]
    public class SessionManagerTests
    {
        private SessionManager goldenEyeSession;
        private SessionManager area51Session;
        private readonly int timeZoneOffset = -4; // EST offset

        [SetUp]
        public void Setup()
        {
            goldenEyeSession = new SessionManager(16, 19, timeZoneOffset); // 4 PM - 7 PM EST
            area51Session = new SessionManager(0, 2, timeZoneOffset);      // 12 AM - 2 AM EST
        }

        [Test]
        public void GoldenEye_IsWithinSession_ValidatesCorrectly()
        {
            // Valid time - 4:30 PM EST
            var validTime = new DateTime(2024, 12, 8, 16, 30, 0);
            Assert.IsTrue(goldenEyeSession.IsWithinSession(validTime));

            // Invalid time - 8:30 PM EST
            var invalidTime = new DateTime(2024, 12, 8, 20, 30, 0);
            Assert.IsFalse(goldenEyeSession.IsWithinSession(invalidTime));
        }

        [Test]
        public void Area51_IsWithinSession_ValidatesCorrectly()
        {
            // Valid time - 1:30 AM EST
            var validTime = new DateTime(2024, 12, 8, 1, 30, 0);
            Assert.IsTrue(area51Session.IsWithinSession(validTime));

            // Invalid time - 3:30 AM EST
            var invalidTime = new DateTime(2024, 12, 8, 3, 30, 0);
            Assert.IsFalse(area51Session.IsWithinSession(invalidTime));
        }

        [Test]
        public void GetNextSessionStart_CalculatesCorrectly()
        {
            // Current time: 3 PM EST
            var currentTime = new DateTime(2024, 12, 8, 15, 0, 0);
            var expectedStart = new DateTime(2024, 12, 8, 16, 0, 0);
            
            var nextStart = goldenEyeSession.GetNextSessionStart(currentTime);
            Assert.AreEqual(expectedStart, nextStart);
        }

        [Test]
        public void SessionTransition_HandlesCorrectly()
        {
            // Before session
            var beforeSession = new DateTime(2024, 12, 8, 15, 59, 0);
            Assert.IsFalse(goldenEyeSession.IsWithinSession(beforeSession));

            // Start of session
            var startSession = new DateTime(2024, 12, 8, 16, 0, 0);
            Assert.IsTrue(goldenEyeSession.IsWithinSession(startSession));

            // End of session
            var endSession = new DateTime(2024, 12, 8, 19, 0, 0);
            Assert.IsFalse(goldenEyeSession.IsWithinSession(endSession));
        }

        [Test]
        public void DaylightSavings_HandlesCorrectly()
        {
            // TODO: Add tests for daylight savings transitions
            // This will require additional implementation in SessionManager
        }
    }
}