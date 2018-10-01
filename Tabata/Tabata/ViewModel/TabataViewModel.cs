using System;
using System.ComponentModel;
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

        #region BindableProperties
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

        private Color backgroundColor;
        public Color BackgroundColor { get { return backgroundColor; } set { if (value != backgroundColor) backgroundColor = value; OnPropertyChanged("BackgroundColor"); } }

        private int excerciseTime;
        public int ExcerciseTime
        {
            get { return excerciseTime; }
            set
            {
                if (value != excerciseTime)
                {
                    if(value<15 && value < excerciseTime)
                    {
                        ExcerciseTimeIncrement = 1;
                    }
                    else if(value > 9 && value > excerciseTime)
                    {
                        ExcerciseTimeIncrement = 5;
                    }
                                       
                    excerciseTime = value;
                    OnPropertyChanged("ExcerciseTime");
                }
            }
        }

        private int exerciseTimeIncrement;
        public int ExcerciseTimeIncrement
        {
            get { return exerciseTimeIncrement; }
            set
            {
                if (value != exerciseTimeIncrement)
                {
                    exerciseTimeIncrement = value;
                    OnPropertyChanged("ExcerciseTimeIncrement");
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
        private int warmupTime;
        public int WarmupTime
        {
            get { return warmupTime; }
            set
            {
                if (value != warmupTime)
                {
                    warmupTime = value;
                    OnPropertyChanged("WarmupTime");
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
        public bool Paused
        {
            get
            { return _paused; }
            private set
            {
                _paused = value;
                OnPropertyChanged("Paused");
            }
        }

        public static TabataViewModel Instance { get; private set; }

        private bool _stopped;
        public bool Stopped { get { return _stopped; } private set { _stopped = value; OnPropertyChanged("Stopped"); } }

        #endregion

        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }

        public TabataViewModel()
        {
            Start = new Command(() => { StartTimer(); });
            Stop = new Command(() => { StopTimer(); });
            Stopped = true;
            Paused = true;
            Instance = this;

            //Set color
            BackgroundColor = Color.Default;
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
            BackgroundColor = Color.Default;
        }

        private void StartTimer()
        {
            if (Progress >= 1.0 || Progress == 0)
                ResetTimer();

            Paused = false;
            Stopped = false;
            Device.StartTimer(TimeSpan.FromSeconds(1), () => TimerCallback());
            ShowNotification();
         }

        private void ResetTimer()
        {
            Time = TimeSpan.FromSeconds(WarmupTime*(-1));
            Progress = 0;
        }

        private bool TimerCallback()
        {
            if (Paused || Stopped)
                return false;
            if(Reps * (BreakTime + ExcerciseTime) == 0)
                return false;

            Time += TimeSpan.FromSeconds(1);
            Progress = Time.TotalSeconds / (Reps * ExcerciseTime + (Reps - 1) * BreakTime);
            ShowNotification(String.Format("Time passed: {0}", Time.TotalSeconds));

            for(int x = Reps; x >= 1; x--)
            {
                //Played at excersise complete
                if (Time.TotalSeconds == (x*ExcerciseTime+(x-1)*BreakTime))
                {
                    PlaySound(AudioEnum.Stop);
                    BackgroundColor = Color.Default;
                    if (Time.TotalSeconds == (Reps * ExcerciseTime + (Reps - 1) * BreakTime))
                    {
                        Stopped = true;
                        Paused = true;
                        ShowNotification("Training completed!");
                        return false;
                    }
                    return true;
                }
                //Played at excersise start
                else if (Time.TotalSeconds == (x * ExcerciseTime + x * BreakTime))
                {
                    PlaySound(AudioEnum.Start);
                    BackgroundColor = Color.LimeGreen;
                }
                //Played at warmup
                else if (Time.TotalSeconds <= 0)
                {
                    PlaySound(AudioEnum.Pre);
                }

                //Change color at start of excersise
                if(Time.TotalSeconds == 0)
                {
                    BackgroundColor = Color.LimeGreen;
                    PlaySound(AudioEnum.Start);
                }
                    
            }
            
            return true;
        }

        private void PlaySound(AudioEnum audio)
        {
            string audioFileName;

            switch (audio)
            {
                case AudioEnum.Pre:
                    audioFileName = "pre.mp3";
                    break;
                case AudioEnum.Start:
                    audioFileName = "start.mp3";
                    break;
                case AudioEnum.Stop:
                    audioFileName = "finish.mp3";
                    break;
                default:
                    audioFileName = "pre.mp3";
                    break;
            }

            Task.Run(async () => { await DependencyService.Get<IAudioManager>().PlaySound(audioFileName); }); 
        }

        internal void CloseApp()
        {
            Paused = true;
            HideNotification();
        }

        internal void ShowNotification()
        {
            DependencyService.Get<INotification>().Create("Tabata is running!");
        }

        internal void ShowNotification(string message)
        {
            DependencyService.Get<INotification>().Create(message);
        }

        internal void HideNotification()
        {
            DependencyService.Get<INotification>().Hide();
        }

        internal void LoadSettings()
        {
            var settings = Serializer.Deserialize<Settings>();
            if (settings == null)
                return;
            Reps = settings.Reps;
            ExcerciseTime = settings.ExcersiseTime;
            BreakTime = settings.BreakTime;
            WarmupTime = settings.WarmupTime;
        }

        internal void SaveSetting()
        {
            var settings = new Settings() { Reps = this.Reps, BreakTime = this.BreakTime, ExcersiseTime = this.ExcerciseTime, WarmupTime = this.WarmupTime };
            var result = Serializer.Serialize<Settings>(settings);
        }
    }
}
