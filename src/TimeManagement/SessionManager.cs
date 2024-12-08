using System;

namespace cAlgo.TimeManagement
{
    public class SessionManager
    {
        private readonly int startHourEst;
        private readonly int endHourEst;
        private readonly int timeZoneOffset; // Difference between server time and EST

        public SessionManager(int startHourEst, int endHourEst, int timeZoneOffset)
        {
            this.startHourEst = startHourEst;
            this.endHourEst = endHourEst;
            this.timeZoneOffset = timeZoneOffset;
        }

        public bool IsWithinSession(DateTime serverTime)
        {
            var estHour = (serverTime.Hour + timeZoneOffset) % 24;
            return estHour >= startHourEst && estHour < endHourEst;
        }

        public DateTime GetNextSessionStart(DateTime currentServerTime)
        {
            var currentEstHour = (currentServerTime.Hour + timeZoneOffset) % 24;
            var hoursToAdd = currentEstHour >= endHourEst 
                ? (24 - currentEstHour + startHourEst) 
                : (startHourEst - currentEstHour);
                
            return currentServerTime.AddHours(hoursToAdd);
        }
    }
}