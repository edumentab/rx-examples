using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentUI
{
    public class SentimentAnalysis
    {
        private List<string> positive = new List<string>
                { ":)", ":d", "yay", "good", "great", "<3", "yes", "yeah",
                  "pretty", "cute", "beautiful", "love", "happy", "awesome",
                  "joy", "best", "better", "win", "gold", "medal" };
        private List<string> negative = new List<string>
                { ":(", ":/", "argh", "darn", "dammit", "idiot",
                  "stupid", "moron", "ruin", "destory", "pain", "shit",
                  "crap", "scandal", "death", "theft", "worse", "worst",
                  "lose", "fail", "awful", "miss", "kill" };

        public int Score(Tweet t)
        {
            var lc = t.Text.ToLower();
            var score = 0;
            score += positive.Count(word => lc.Contains(word));
            score -= negative.Count(word => lc.Contains(word));
            return score;
        }
    }
}
