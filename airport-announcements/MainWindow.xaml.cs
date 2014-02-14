using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Announcements.AnnouncementSystem;
using Announcements.DomainEvents;

namespace Announcements
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<FlightInfo> Flights { get; private set; }
        public ObservableCollection<string> Announcements { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            // Some dummy flight info.
            Flights = new ObservableCollection<FlightInfo>
            {
                new FlightInfo("SK123", "Oslo", 13, 30),
                new FlightInfo("LH42", "Munich", 13, 40),
                new FlightInfo("SK45", "Trondheim", 13, 45),
                new FlightInfo("BA141", "London", 13, 50),
                new FlightInfo("UA12", "Kyiv", 14, 00),
            };

            // Wire listbox up to announcements stream.
            Announcements = new ObservableCollection<string>();
            var ann = new Announcer();
            ann.Announcements
                .ObserveOnDispatcher()
                .Subscribe(Announcements.Add);

            // Wire up speech synthesis to announcements stream;
            // use Do to get one at a time.
            ann.Announcements
                .ObserveOn(TaskPoolScheduler.Default)
                .Do(Say)
                .Subscribe();

            DataContext = this;
            Flight.SelectedIndex = 0;
        }

        private void Say(string msg)
        {
            var ss = new SpeechSynthesizer();
            var pb = new PromptBuilder();
            pb.StartVoice("Microsoft Mary");
            pb.AppendText(msg);
            pb.EndVoice();
            ss.Speak(pb);
        }

        // Event handlers that just trigger events directly, but in reality
        // the events would be spat out by an aggregate.

        private void ChangeDepartureTime(object sender, RoutedEventArgs e)
        {
            var picked = (FlightInfo)Flight.SelectedItem;
            var newHour = byte.Parse(NewHour.Text);
            var newMinute = byte.Parse(NewMinute.Text);
            if (picked.Hour == newHour && picked.Minute == newMinute)
                EventStreams.DepartureReturnedToOnTime.OnNext(
                    AddFlightInfo(new DepartureReturnedToOnTime
                    {
                        ScheduledHour = newHour,
                        ScheduledMinute = newMinute
                    }));
            else
                EventStreams.DepartureDelayed.OnNext(
                    AddFlightInfo(new DepartureDelayed
                    {
                        ScheduledHour = picked.Hour,
                        ScheduledMinute = picked.Minute,
                        NewHour = newHour,
                        NewMinute = newMinute
                    }));
        }

        private void CancelDeparture(object sender, RoutedEventArgs e)
        {
            EventStreams.DepartureCanceled.OnNext(AddFlightInfo(new DepartureCanceled()));
        }

        private void AssignGate(object sender, RoutedEventArgs e)
        {
            EventStreams.GateAssigned.OnNext(AddFlightInfo(new GateAssigned
            {
                Gate = AssignedGate.Text
            }));
            AssignedGate.Text = "";
        }

        private void ChangeGate(object sender, RoutedEventArgs e)
        {
            EventStreams.GateChanged.OnNext(AddFlightInfo(new GateChanged
            {
                NewGate = NewGate.Text
            }));
            NewGate.Text = "";
        }

        private void OpenGate(object sender, RoutedEventArgs e)
        {
            EventStreams.GateOpened.OnNext(AddFlightInfo(new GateOpened()));
        }

        private T AddFlightInfo<T>(T e)
        {
            dynamic eventIn = e;
            var picked = (FlightInfo)Flight.SelectedItem;
            eventIn.FlightCode = picked.FlightCode;
            eventIn.Destination = picked.Destination;
            return e;
        }
    }
}
