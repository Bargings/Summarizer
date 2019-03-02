using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.ling;
using edu.stanford.nlp;
using edu.stanford.nlp.util;
using java.io;
using Console = System.Console;
using java.util;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Summarizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string story = "";
        private StoryDocument Doc;
        public MainWindow()
        {
            InitializeComponent();
        }

        // get the base folder for the project
      
        private void btnSummarize_Click(object sender, RoutedEventArgs e)
        {
            List<string> words = new List<string>();
           
            words = Doc.SummaryTools.GetWords(Doc.Story, Doc.Anno);
            Doc.StoryWords = Doc.SummaryTools.CalculateWordFrequency(words, Doc.Paragraphs);
            CloudRtxBox.Document.Blocks.Clear();           
            CloudRtxBox.FontSize = CloudRtxBox.FontSize + 1;
            foreach (StoryWord w in Doc.StoryWords)
            {
                if (w.Count > 0)
                {
                    TextRange rangeOfText = new TextRange(CloudRtxBox.Document.ContentEnd, CloudRtxBox.Document.ContentEnd);
                    rangeOfText.Text = w.Text + " ";
                    double fontsize = 12.0 * w.Tfidf * 500.0; if (fontsize < 1.0) { fontsize = 1.0; }
                    rangeOfText.ApplyPropertyValue(TextElement.FontSizeProperty, fontsize);
                }
            }
            Doc.SummaryTools.CalculateSentenceWeight(Doc.Paragraphs, Doc.StoryWords,Doc.Title, Doc.Anno);
            rtxtStory.Document.Blocks.Clear();
            double maxweight = 0;
            foreach(StoryParagraph p in Doc.Paragraphs)
            {
                foreach(StorySentense s in p.ParagraphSentenses)
                {                    
                    if (s.Weight > maxweight ){ maxweight = s.Weight; }
                }
            }
            sliderThreshold.Maximum = maxweight;
            sliderThreshold.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.Both;
            sliderThreshold.Value = maxweight / 2;
            string textcontents = new TextRange(rtxtStory.Document.ContentStart, rtxtStory.Document.ContentEnd).Text;
            while (textcontents.Split(' ').Count() < Doc.SummaryTools.StoryWordCount/4)
            {
                sliderThreshold.Value = sliderThreshold.Value - 0.1;
                textcontents = new TextRange(rtxtStory.Document.ContentStart, rtxtStory.Document.ContentEnd).Text;
            }
            sliderThreshold.IsSnapToTickEnabled = true;
            sliderThreshold.TickFrequency = maxweight / 100;
            lblThreshold.Content = "Sentense threshold" + " " + Math.Round(sliderThreshold.Value ,5).ToString();

        }
     
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.ShowDialog();
                try
                {
                    story = System.IO.File.ReadAllText(openFileDialog.FileName);
                }
                catch (ArgumentNullException ex)
                {
                    MessageBox.Show(ex.Message);
                    Environment.Exit(0);
                }
              
                Doc = new StoryDocument(story);
                if (txtTitle.Text != "Title")
                {
                    Doc.Title = txtTitle.Text.ToLower();
                }
                else
                {
                    Doc.Title = openFileDialog.SafeFileName.Replace(".txt", "");
                    txtTitle.Text = Doc.Title;
                }

                rtxtStory.Document.Blocks.Clear();
                foreach (StoryParagraph p in Doc.Paragraphs)
                {
                    rtxtStory.AppendText(p.Text + '\r' + '\n');
                }

            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        

        private void SliderThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblThreshold.Content = "Sentense threshold" + " " + Math.Round(sliderThreshold.Value, 5).ToString();
            
            rtxtStory.Document.Blocks.Clear();
            foreach (StoryParagraph p in Doc.Paragraphs)
            {
                foreach (StorySentense s in p.ParagraphSentenses)
                {
                    if (s.Weight > sliderThreshold.Value)
                    {
                        TextRange rangeOfText = new TextRange(rtxtStory.Document.ContentEnd, rtxtStory.Document.ContentEnd);
                        rangeOfText.Text = s.Text;
                        rangeOfText.ApplyPropertyValue(TextElement.FontSizeProperty, 12.0);// * s.Weight * 100);
                    }
                }
                rtxtStory.AppendText("" + '\r' + '\n');
            }
        }

        
    }
  
}
