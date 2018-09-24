using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
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

        private double progress;
        public double Progress {
            get { return progress; }
            set
            {
                if (value != progress)
                    progress = value;
                OnPropertyChanged("Progress");
            }
        }

        private bool _paused;
        public bool Paused { get { return _paused; } private set { _paused = value; OnPropertyChanged("Paused"); } }

        public static TabataViewModel Instance { get; private set; }

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
            Instance = this;

            //First run to initialize service
            PlaySound();
        }

        private void StopTimer()
        {
            if (Paused)
            {
                Stopped = true;
                ResetTimer();
            }
            else
            {
                Paused = true;
            }                          
        }

        private void StartTimer()
        {
            if (Progress >= 1.0)
                ResetTimer();

            Paused = false;
            Stopped = false;
            Device.StartTimer(TimeSpan.FromSeconds(1), () => TimerCallback());
            DependencyService.Get<INotification>().Create("Tabata is running!");
        }

        private void ResetTimer()
        {
            Time = TimeSpan.FromSeconds(0);
        }

        private bool TimerCallback()
        {
            if (Paused || Stopped)
                return false;
            if(Reps * (BreakTime + ExcerciseTime) == 0)
                return false;

            Time += TimeSpan.FromSeconds(1);
            Progress = Time.TotalSeconds / (Reps * ExcerciseTime + (Reps - 1) * BreakTime);

            for(int x = Reps; x >= 1; x--)
            {
                //Played at excersise complete
                if (Time.TotalSeconds == (x*ExcerciseTime+(x-1)*BreakTime))
                {
                    PlaySound();
                    if (Time.TotalSeconds == (Reps * ExcerciseTime + (Reps - 1) * BreakTime))
                    {
                        Stopped = true;
                        Paused = true;
                        return false;
                    }
                    return true;
                }
                //Played at excersise start
                else if (Time.TotalSeconds == (x * ExcerciseTime + x * BreakTime))
                {
                    PlaySound();
                }
            }
            
            return true;
        }

        private void PlaySound()
        {
            Task.Run(async () => { await DependencyService.Get<IAudioManager>().PlaySound("Ring.mp3"); }).ConfigureAwait(false);            
        }

        internal void CloseApp()
        {
            Paused = true;
            DependencyService.Get<INotification>().Hide();
        }
    }
}
