using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Tabata.ViewModel
{
    class TabataViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        private int reps;
        public int Reps {
            get { return reps; }
            set
            {
                if (value != reps)
                {
                    reps = value;
                    OnPropertyChanged("Reps");
                }
            } }
        private int exerciseTime;
        public int ExcerciseTime
        {
            get { return exerciseTime; }
            set
            {
                if (value != exerciseTime)
                {
                    exerciseTime = value;
                    OnPropertyChanged("ExcerciseTime");
                }
            }
        }
        private int breakTime;
        public int BreakTime
        {
            get { return breakTime; }
            set
            {
                if (value != breakTime)
                {
                    breakTime = value;
                    OnPropertyChanged("BreakTime");
                }
            }
        }
        private bool enableControls;
        public bool EnableControls {
            get { return enableControls; }
            set { enableControls = value; OnPropertyChanged("EnableControls"); } }
        private TimeSpan time;
        public TimeSpan Time
        {
            get { return time; }
            protected set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged("Time");
                }
            }
        }

        private bool _paused;
        public bool Paused { get { return _paused; } private set { _paused = value; OnPropertyChanged("Paused"); } }
        private bool _stopped;
        public bool Stopped { get { return _stopped; } private set { _stopped = value; OnPropertyChanged("Stopped"); } }


        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }

        public TabataViewModel()
        {
            Start = new Command(() => { StartTimer(); });
            Stop = new Command(() => { StopTimer(); });
            Stopped = true;
            Paused = true;
        }

        private void StopTimer()
        {
            if (Paused)
            {
                Stopped = true;
                Time = TimeSpan.FromSeconds(0);
            }
            else
            {
                Paused = true;
            }                          
        }

        private void StartTimer()
        {
            Paused = false;
            Stopped = false;
            Device.StartTimer(TimeSpan.FromSeconds(1), () => TimerCallback());
        }

        private bool TimerCallback()
        {
            if (Paused)
                return false;

            Time += TimeSpan.FromSeconds(1);
            if (Time.Seconds >= Reps*breakTime*ExcerciseTime)
            {
                StopTimer();
                System.Diagnostics.Debug.WriteLine("stop");
            }
            else if (Time.Seconds % (breakTime * ExcerciseTime) == 0)
            {
                System.Diagnostics.Debug.WriteLine("break");
            }

            return true;
        }
    }
}
