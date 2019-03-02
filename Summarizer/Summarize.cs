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
using System.Text.RegularExpressions;
using edu.stanford.nlp.coref.data;
using edu.stanford.nlp.coref;




namespace Summarizer
{
    public class Summarize
    {
        public ArrayList sentences;
        
        public double TFidfSignificntThreshold;
        public int StoryWordCount = 0;
        public Summarize()
        { }

        public static string GetAppFolder()
        {
            return System.IO.Directory.GetCurrentDirectory().Replace(@"Summarizer\bin\Debug", string.Empty);
        }
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show("MyHandler caught : " + e.Message);
        }
        public Annotation Anno(string text, string annotators)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            var jarRoot = System.IO.Path.Combine(GetAppFolder(), @"packages\Stanford.NLP.CoreNLP.3.9.1.0");
            // Annotation pipeline configuration
            var props = new java.util.Properties();
            props.setProperty("annotators", annotators);
            props.setProperty("ner.useSUTime", "0");
            // We should change current directory, so StanfordCoreNLP could find all the model files automatically
            var curDir = Environment.CurrentDirectory;
            try
            {
                Directory.SetCurrentDirectory(jarRoot);
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message);
            }
            var pipeline = new StanfordCoreNLP(props);
            Directory.SetCurrentDirectory(curDir);
            CoreDocument doc = new CoreDocument(text);
            // Annotation
            Annotation annotation = new Annotation(text);
            pipeline.annotate(annotation);
           
            var FilePath = System.IO.Path.Combine(GetAppFolder(), @"packages\Stanford.NLP.CoreNLP.3.9.1.0\patterns\stopwords.txt");

            return annotation;
        }
        public List<string> GetWords(string text, Annotation annotation)
        {
            var FilePath = System.IO.Path.Combine(GetAppFolder(), @"packages\Stanford.NLP.CoreNLP.3.9.1.0\patterns\stopwords.txt");
            string line = "";
            List<string> stopwords = new List<string>();
            List<string> words = new List<string>();
            StoryWordCount = 0;
            using (StreamReader stream = new StreamReader(FilePath))
            {
                while ((line = stream.ReadLine()) != null)
                {
                    stopwords.Add(line.ToLower());
                }
            }
                        
            sentences = annotation.get(new CoreAnnotations.SentencesAnnotation().getClass()) as ArrayList;
            
            
            foreach (CoreMap sentence in sentences)
            {
                var tokens = sentence.get(new CoreAnnotations.TokensAnnotation().getClass()) as ArrayList;
                foreach (CoreLabel token in tokens)
                {
                    var word = token.get(new CoreAnnotations.TextAnnotation().getClass());
                    var ner = token.get(new CoreAnnotations.NamedEntityTagAnnotation().getClass()) as string;
                    var normalizedner = token.get(new CoreAnnotations.NormalizedNamedEntityTagAnnotation().getClass());

                    if (stopwords.Contains(word.ToString().ToLower()))
                    {
                        //token.remove(new CoreAnnotations.TextAnnotation().getClass());
                    }
                    else
                    {
                        if (ner != "NUMBER")
                        {
                            words.Add(word.ToString().ToLower());
                        }
                        
                    }
                    StoryWordCount += 1;
                }
            }
            return words;
        }

        public List<StoryWord> CalculateWordFrequency(List<string> Words, List<StoryParagraph> Paragraphs)
        {
            List<string> Visited = new List<string>();
            List<StoryWord> WordsInDocument = new List<StoryWord>();
            double TotalFidf = 0;
            int AvgCount = 0;
            foreach (string w in Words)
            {
                if (Visited.Contains(w.ToLower()) is false)
                {
                    Visited.Add(w.ToLower());
                    double count = Words.Where(temp => temp.Equals(w.ToLower())).Select(temp => temp).Count();
                    double ApearedInPara = 0;
                    
                    foreach (StoryParagraph p in Paragraphs)
                    {
                        if (Regex.IsMatch(p.Text.ToLower(), @"\b"+ w +@"\b",RegexOptions.IgnoreCase))
                        {
                            ApearedInPara = ApearedInPara + 1;
                        }
                    }
                    if (ApearedInPara > 0)
                    {
                        StoryWord word = new StoryWord(w.ToLower())
                        {
                            Count = count,
                            DocumentsApeared = ApearedInPara,
                            Tfidf = (count / Words.Count) * Math.Log(Paragraphs.Count / ApearedInPara)
                                                       
                        };
                        if (word.Tfidf == 0) { word.Tfidf = 0.00000000001; }
                        if (word.Count > 1)
                        {
                            TotalFidf = TotalFidf + word.Tfidf;
                            AvgCount += 1;
                        }
                        
                        //if (MaxTFidf < word.Tfidf) { MaxTFidf = word.Tfidf; }
                        WordsInDocument.Add(word);
                    }
                    count = 0;
                }
            }
            TFidfSignificntThreshold = TotalFidf / AvgCount;
            return WordsInDocument;
        }

        public void CalculateSentenceWeight(List<StoryParagraph> Paragraphs, List<StoryWord> CountedWords, string Title, Annotation annotation)
        {
            var map = annotation.get(new CorefCoreAnnotations.CorefChainAnnotation().getClass()) as Map;
            String newwords = "";
            if ((sentences is null) || (sentences.isEmpty()))
            {
                sentences = annotation.get(new CoreAnnotations.SentencesAnnotation().getClass()) as ArrayList;
            }
            foreach (StoryParagraph p in Paragraphs)
            {
                foreach (StorySentense s in p.ParagraphSentenses)
                {
                    foreach (CoreMap sentense in sentences)
                    {
                        if(sentense.ToString().ToLower() == s.Text.ToLower())
                        {
                            var tokens = sentense.get(new CoreAnnotations.TokensAnnotation().getClass()) as ArrayList;
                            foreach (CoreLabel token in tokens)
                            {
                                var word = token.get(new CoreAnnotations.TextAnnotation().getClass());
                                word = word.ToString().ToLower();
                                var corefClustId = token.get(new CorefCoreAnnotations.CorefClusterIdAnnotation().getClass());
                                var chain = map.get(corefClustId) as CorefChain;
                                if (chain == null || chain.getMentionsInTextualOrder().size() == 1)
                                {
                                    if (word != null)
                                    {
                                        foreach (StoryWord w in CountedWords)
                                        {
                                            if (word.ToString() == w.Text)
                                            {
                                                s.Weight = s.Weight + w.Tfidf;
                                                if (w.Tfidf >= TFidfSignificntThreshold)
                                                {
                                                    s.Weight = s.Weight + 5;
                                                }
                                            }
                                        }
                                        if (Regex.IsMatch(Title.ToLower(), @"\b" + word.ToString().ToLower() + @"\b", RegexOptions.IgnoreCase))
                                        {
                                            s.Weight = s.Weight + 0.01;
                                        }
                                    }
                                }
                                else
                                {
                                    int sentINdx = chain.getRepresentativeMention().sentNum;
                                    var corefSentence = sentences.get(sentINdx-1) as CoreMap;
                                    var corefSentenceTokens = corefSentence.get(new CoreAnnotations.TokensAnnotation().getClass()) as ArrayList;
                                    
                                    CorefChain.CorefMention reprMent = chain.getRepresentativeMention();
                                    if (token.index() < reprMent.startIndex || token.index() > reprMent.endIndex)
                                    {
                                        for (int i = reprMent.startIndex; i < reprMent.endIndex; i++)
                                        {
                                            try
                                            {
                                                var matchedLabel = corefSentenceTokens.get(i - 1) as CoreLabel;
                                                newwords += matchedLabel.word() + " ";
                                            }
                                            catch
                                            {}
                                            
                                        }
                                        foreach (StoryWord w in CountedWords)
                                        {
                                            if (Regex.IsMatch(newwords.ToLower(), @"\b" + w.Text + @"\b", RegexOptions.IgnoreCase))
                                            {
                                                s.Weight = s.Weight + w.Tfidf;
                                                if (w.Tfidf >= TFidfSignificntThreshold)
                                                {
                                                    s.Weight = s.Weight + 0.1;
                                                }
                                            }
                                        }
                                        if (Regex.IsMatch(Title.ToLower(), @"\b" + newwords.ToLower() + @"\b", RegexOptions.IgnoreCase))
                                        {
                                            s.Weight = s.Weight + 1;
                                        }
                                    }
                                    else
                                    {
                                        if (word != null)
                                        {
                                            foreach (StoryWord w in CountedWords)
                                            {
                                                if (word.ToString() == w.Text)
                                                {
                                                    s.Weight = s.Weight + w.Tfidf;
                                                    if (w.Tfidf >= TFidfSignificntThreshold) { s.Weight = s.Weight + 5; }
                                                }
                                            }
                                            if (Regex.IsMatch(Title.ToLower(), @"\b" + word.ToString().ToLower() + @"\b", RegexOptions.IgnoreCase))
                                            {
                                                s.Weight = s.Weight + 1;
                                            }
                                        }
                                    }
                                }
                                newwords = "";
                            }
                        }
                    }
                    s.Weight = s.Weight + 5/ s.SentenseID + 0.1/ s.ParagraphID;
                    if (s.SentenseID == 1)
                    {
                        s.Weight = s.Weight + 5;
                    }
                    if (s.SentenseID >= p.ParagraphSentenses.Count()-2)
                    {
                        s.Weight = s.Weight + 5;
                    }
                    if (s.ParagraphID >= Paragraphs.Count - 2)
                    {
                       s.Weight = s.Weight + 5/s.SentenseID;
                    }
                    
                }
            }
        }
    }
}
