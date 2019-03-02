using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using edu.stanford.nlp.pipeline;
using System.Windows;

namespace Summarizer
{
    public class StoryDocument
    {
        public string Story { get; }
        public string Title { get; set; }
        private int noOfWords;
        public List<StoryParagraph> Paragraphs { get; set; }
        public List<StoryWord> StoryWords { get; set; }
        public StoryWord w;
        public StoryParagraph Paragraph;
        public Summarize SummaryTools;
        public Annotation Anno;
        public StoryDocument(String story)
        {
            Story = story;
            noOfWords = Story.Split(new[] { ' ' }).Length;
            var storyLines = Story.Split(new[] { '\r', '\n' });
            int lineCounter = 0;
            foreach (string line in storyLines)
            {
                string ln = line.Trim();
                if (ln.Length > 0)
                {
                    lineCounter = lineCounter + 1;
                }
            }
            int avgLineLength = noOfWords / lineCounter;
            SummaryTools = new Summarize();
           
            Anno = SummaryTools.Anno(Story, "tokenize, ssplit, pos, lemma, ner, depparse, coref.mention, coref");
           
            Paragraphs = SeparateParagraphs(storyLines, avgLineLength);
            
        }
        private List<StoryParagraph> SeparateParagraphs(string[] storyLines, int avgWordsInLine)
        {
            List<StoryParagraph> paragraphs = new List<StoryParagraph>();
            String ptext = "";
            int LineNoWords = 0;
            int paragraphCounter = 1;
            Boolean ForceParagraph = false;
            Annotation AnnoSSplit = new Annotation();
            foreach (string line in storyLines)
            {
                string ln = line.Trim();
                LineNoWords = ln.Split(new[] { ' ' }).Length;
                if (ln.Length > 0)
                {
                    if ((LineNoWords <= avgWordsInLine) && ((ln.Substring(ln.Length - 1) == ".") ||
                                                            (ln.Substring(ln.Length - 1) == "’") ||
                                                            (ln.Substring(ln.Length - 1) == "\"")) ||
                                                            (ForceParagraph = true))
                    {
                        ptext = ptext + ln + " ";
                        AnnoSSplit = SummaryTools.Anno(ptext, "tokenize, ssplit");
                        Paragraph = new StoryParagraph(ptext, paragraphCounter, AnnoSSplit);
                        paragraphs.Add(Paragraph);
                        ptext = "";
                        ForceParagraph = false;
                    }
                    else
                    {
                        ptext = ptext + ln + " ";
                    }
                    paragraphCounter = paragraphCounter + 1;
                }
                else
                {
                    ForceParagraph = true;
                }
            }
            AnnoSSplit = SummaryTools.Anno(ptext, "tokenize, ssplit");
            Paragraph = new StoryParagraph(ptext, paragraphCounter, AnnoSSplit);
            paragraphs.Add(Paragraph);
            return paragraphs;
        }
        
    }
}
