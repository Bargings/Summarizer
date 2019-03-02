using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summarizer
{
    public class StorySentense
    {
        public StorySentense(string text)
        {
            Text = text;
        }
        public string Text { get; set;}
        public int ParagraphID { get; set;}
        public int SentenseID { get; set;}
        public double Weight { get; set; }
    }
}
