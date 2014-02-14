using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;
using Newtonsoft.Json.Linq;

namespace SentimentUI
{
    public class TwitterObservable
    {
        private static IAuthorizer auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
                    AccessToken = ConfigurationManager.AppSettings["accessToken"],
                    AccessTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"]
                }
            };

        public static IObservable<Tweet> AllTweetsAbout(string topic)
        {
            return Observable.Create<Tweet>(obs =>
                {
                    var disposed = false;
                    Task.Run(() =>
                    {
                        var ctx = new TwitterContext(auth);
                        var query = from s in ctx.Streaming
                                    where s.Type == StreamingType.Filter &&
                                            s.Track == topic
                                    select s;
                        query.StartAsync(s =>
                            {
                                if (disposed)
                                    s.CloseStream();
                                else
                                    obs.OnNext(Parse(s.Content));
                                return Task.FromResult(true);
                            });
                    });
                    return Disposable.Create(() => {
                        disposed = true;
                    });
                }); 
        }

        private static Tweet Parse(string json)
        {
            dynamic parsed = JObject.Parse(json);
            return new Tweet
            {
                Text = parsed.text,
                User = parsed.screen_name
            };
        }
    }
}
