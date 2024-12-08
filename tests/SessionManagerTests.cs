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
            // Spring forward
            var springForward = new DateTime(2024, 3, 10, 16, 30, 0);
            Assert.IsTrue(goldenEyeSession.IsWithinSession(springForward));

            // Fall back
            var fallBack = new DateTime(2024, 11, 3, 16, 30, 0);
            Assert.IsTrue(goldenEyeSession.IsWithinSession(fallBack));
        }

        [Test]
        public void CrossDaySession_HandlesCorrectly()
        {
            // Area 51 session spans across midnight
            var beforeMidnight = new DateTime(2024, 12, 8, 23, 30, 0);
            Assert.IsFalse(area51Session.IsWithinSession(beforeMidnight));

            var afterMidnight = new DateTime(2024, 12, 9, 0, 30, 0);
            Assert.IsTrue(area51Session.IsWithinSession(afterMidnight));
        }

        [Test]
        public void WeekendTransition_HandlesCorrectly()
        {
            // Friday session end
            var fridayEnd = new DateTime(2024, 12, 6, 19, 0, 0);
            Assert.IsFalse(goldenEyeSession.IsWithinSession(fridayEnd));

            // Monday session start
            var mondayStart = new DateTime(2024, 12, 9, 16, 0, 0);
            Assert.IsTrue(goldenEyeSession.IsWithinSession(mondayStart));
        }

        [Test]
        public void InvalidTimeZoneOffset_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new SessionManager(16, 19, -13));
            Assert.Throws<ArgumentException>(() => new SessionManager(16, 19, 13));
        }

        [Test]
        public void InvalidSessionHours_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new SessionManager(-1, 19, timeZoneOffset));
            Assert.Throws<ArgumentException>(() => new SessionManager(16, 24, timeZoneOffset));
        }

        [Test]
        public void GetSessionDuration_CalculatesCorrectly()
        {
            var expectedDuration = TimeSpan.FromHours(3); // Golden Eye: 4 PM - 7 PM
            Assert.AreEqual(expectedDuration, goldenEyeSession.GetSessionDuration());
        }
    }
}