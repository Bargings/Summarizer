using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.util;
using java.util;

namespace Summarizer
{
    public class StoryParagraph
    {
        public string Text { get; set; }
        public int ParagraphID { get; set; }
        public List<StorySentense> ParagraphSentenses { get; set; }
        public StorySentense s;
        public StoryParagraph(String text, int paragraphID, Annotation annotation)
        {
            ParagraphSentenses = new List<StorySentense>();
            Text = text;
            ParagraphID = paragraphID;
                       
            int sentenseID = 1;
                       
           
            ArrayList sentenses = annotation.get(new CoreAnnotations.SentencesAnnotation().getClass()) as ArrayList;
            foreach (CoreMap sentence in sentenses)
            {
                StorySentense s = new StorySentense(sentence.ToString())
                {
                    SentenseID = sentenseID,
                    ParagraphID = ParagraphID
                };
                sentenseID = sentenseID + 1;
                ParagraphSentenses.Add(s);
            }
        }
        public static string GetAppFolder()
        {
            return System.IO.Directory.GetCurrentDirectory().Replace(@"Summarizer\bin\Debug", string.Empty);
        }
    }
}
