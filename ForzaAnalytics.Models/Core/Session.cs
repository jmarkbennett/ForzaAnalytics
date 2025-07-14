using ForzaAnalytics.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForzaAnalytics.Models.Core
{
    public class Session : INotifyPropertyChanged
    {
        private Guid sessionId;
        private int carId;
        private int trackId;
        private string carClass;
        private int carPi;
        private DateTime sessionStart;
        private DateTime sessionEnd;
        private string sessionEndType;
        private ObservableCollection<LapTime> lapTimes;
        private int currentLapNumber;

        public Session()
        {
            sessionId = Guid.NewGuid();
            lapTimes = [];
            carId = -1;
            carClass = "";
            carPi = 0;
            trackId = -1;
            LastSessionRaceTime = 0;
            DistanceTravelled = -100000;
            sessionEndType = "";
        }

        public Guid SessionId
        {
            get { return sessionId; }
            set
            {
                sessionId = value;
                OnPropertyChanged(nameof(SessionId));
            }
        }
        public int CarId
        {
            get { return carId; }
            set
            {
                carId = value;
                OnPropertyChanged(nameof(CarId));
            }
        }
        public string CarClass
        {
            get { return carClass; }
            set
            {
                carClass = value;
                OnPropertyChanged(nameof(CarClass));
            }
        }

        public int CarPi
        {
            get { return carPi; }
            set
            {
                carPi = value;
                OnPropertyChanged(nameof(CarPi));
            }
        }

        public int TrackId
        {
            get { return trackId; }
            set
            {
                trackId = value;
                OnPropertyChanged(nameof(TrackId));
            }
        }

        public int CurrentLapNumber
        {
            get { return currentLapNumber; }
            set
            {
                currentLapNumber = value;
                OnPropertyChanged(nameof(CurrentLapNumber));
            }
        }

        public DateTime SessionStart
        {
            get { return sessionStart; }
            set
            {
                sessionStart = value;
                OnPropertyChanged(nameof(SessionStart));
            }
        }
        public DateTime SessionEnd
        {
            get { return sessionEnd; }
            set
            {
                sessionEnd = value;
                OnPropertyChanged(nameof(SessionEnd));
            }
        }

        public String SessionEndType
        {
            get { return sessionEndType; }
            set
            {
                sessionEndType = value;
                OnPropertyChanged(nameof(SessionEndType));
            }
        }

        

        public ObservableCollection<LapTime> LapTimes
        {
            get { return lapTimes; }
            set
            {
                lapTimes = value;
                OnPropertyChanged(nameof(LapTimes));
            }
        }

        #region Tracking Events
        // we can use scenarios where Distance or Time is less to indicate a change in session.
        // both values increment so should never be less.
        public float LastSessionRaceTime { get; set; }
        public float DistanceTravelled { get; set; }
        #endregion
        public void InitSession(int carId, int trackId, float sessionTime, int carPi, string carClass)
        {
            SessionStart = DateTime.Now;
            CarId = carId;
            TrackId = trackId;
            LastSessionRaceTime = sessionTime;
            CarPi = carPi;
            CarClass = carClass;
        }

        public void FinalizeSession()
        {
            SessionEnd = DateTime.Now;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}