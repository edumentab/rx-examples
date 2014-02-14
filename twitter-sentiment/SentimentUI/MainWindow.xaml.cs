using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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

namespace SentimentUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> LatestPositive { get; set; }
        public ObservableCollection<string> LatestNegative { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var sa = new SentimentAnalysis();
            var currentTopic = Observable
                .Switch(Topics()
                    .Select(
                        topic => TwitterObservable.AllTweetsAbout(topic)));

            var scored = currentTopic
                .Select(tweet => new ScoredTweet
                {
                    Tweet = tweet,
                    Score = sa.Score(tweet)
                })
                .Publish();
            scored.Connect();

            LatestPositive = new ObservableCollection<string>();
            SetupOutput(LatestPositive, scored.Where(st => st.Score > 0));
            LatestNegative = new ObservableCollection<string>();
            SetupOutput(LatestNegative, scored.Where(st => st.Score < 0));

            DataContext = this;
        }

        private void SetupOutput(ObservableCollection<string> target, IObservable<ScoredTweet> tweets)
        {
            tweets
                .Sample(TimeSpan.FromSeconds(1))
                .Select(st => st.Tweet.Text)
                .ObserveOnDispatcher()
                .Subscribe(tweet => StoreLatest(target, tweet));
        }

        private IObservable<string> Topics()
        {
            return Observable
                .FromEventPattern<TextChangedEventArgs>(Topic, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Throttle(TimeSpan.FromSeconds(1));
        }

        private void StoreLatest(ObservableCollection<string> target, string tweet)
        {
            if (target.Count == 4)
                target.RemoveAt(0);
            target.Add(tweet);
        }
    }
}
