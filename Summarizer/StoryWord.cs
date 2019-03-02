using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summarizer
{
    public class StoryWord
    {
        public string Text { get; set; }
        public double Count { get; set; }
        public double DocumentsApeared { get; set; }
        public double Tfidf { get; set; }
        public StoryWord(string text)
        {
            Text = text;
        }
    }
}
