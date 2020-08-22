using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Minista.UserControls.Story
{
    public sealed partial class StoryQuizUc : UserControl
    {
        public InstaStoryItem StoryItem { get; private set; }
        public InstaStoryQuizItem QuizItem { get; private set; }
        public StoryQuizUc()
        {
            InitializeComponent();

        }

        public void SetQuiz(InstaStoryQuizItem quizItem, InstaStoryItem storyItem)
        {
            if (quizItem == null) return;
            if (storyItem == null) return;
            QuizItem = quizItem;
            StoryItem = storyItem;
            try
            {
                var backgroundGradient = new LinearGradientBrush();
                var startBG = new GradientStop { Color = ("#ff" + QuizItem.QuizSticker.StartBackgroundColor.Replace("#", "")).GetColorFromHex() };
                var endBG = new GradientStop { Color = ("#ff" + QuizItem.QuizSticker.EndBackgroundColor.Replace("#", "")).GetColorFromHex() , Offset = 1};
                backgroundGradient.GradientStops.Add(startBG);
                backgroundGradient.GradientStops.Add(endBG);

                //HeaderGrid.Background = backgroundGradient;
                Answer1Ellipse.Stroke = Answer2Ellipse.Stroke = Answer3Ellipse.Stroke = Answer4Ellipse.Stroke = backgroundGradient;
                Answer1OuterText.Foreground = Answer2OuterText.Foreground = Answer3OuterText.Foreground = Answer4OuterText.Foreground = backgroundGradient;
                txtQuiz.Foreground = ("#ff" + QuizItem.QuizSticker.TextColor.Replace("#", "")).GetColorBrush();
                txtQuiz.Text = QuizItem.QuizSticker.Question;

                if (QuizItem.QuizSticker.Tallies.Count > 1)
                {
                    Answer1Text.Text = QuizItem.QuizSticker.Tallies[0].Text;
                    Answer2Text.Text = QuizItem.QuizSticker.Tallies[1].Text;
                }

                if (QuizItem.QuizSticker.Tallies.Count > 2)
                    Answer3Text.Text = QuizItem.QuizSticker.Tallies[2].Text;

                if (QuizItem.QuizSticker.Tallies.Count > 3)
                    Answer4Text.Text = QuizItem.QuizSticker.Tallies[3].Text;

                if (QuizItem.QuizSticker.Tallies.Count > 2)
                    Answer3Grid.Visibility = Answer4Grid.Visibility = Visibility.Visible;

                if (QuizItem.QuizSticker.Tallies.Count > 3)
                    Answer4Grid.Visibility = Visibility.Visible;

                if (QuizItem.QuizSticker.Tallies.Count == 2)
                    Answer3Grid.Visibility = Answer4Grid.Visibility = Visibility.Collapsed;

                //backgroundGradient.GradientStops.Add
                //<LinearGradientBrush>
                //    <GradientStop Color="Black" />
                //    <GradientStop Color="#FFEC7171"
                //                  Offset="1" />
                //</LinearGradientBrush>



                //Answer1Grid
                //Answer1Ellipse
                //Answer1OuterText
                //Answer1Text
                ShowCorrectAnswerIfExists();



            }
            catch { }
        }

        void ShowCorrectAnswerIfExists()
        {
            try
            {
                //if (!QuizItem.QuizSticker.ViewerCanAnswer || !QuizItem.QuizSticker.Finished || QuizItem.QuizSticker.ViewerAnswer != -1)
                {
                    
                    var whiteColor = new SolidColorBrush(Colors.White);
                    var blackColor = new SolidColorBrush(Colors.Black);
                    var redColor = "#FFFF4C4C".GetColorBrush();
                    var greenColor = "#FF4CFF85".GetColorBrush();
                    var greenLowColor = "#FFA7FFC3".GetColorBrush();
                    var materialFont = Application.Current.Resources["MaterialSymbolFont"] as FontFamily;
                    if (QuizItem.QuizSticker.ViewerAnswer != -1)
                    {
                        Answer1Button.IsEnabled = Answer2Button.IsEnabled = Answer3Button.IsEnabled = Answer4Button.IsEnabled = false;
                        if (QuizItem.QuizSticker.ViewerAnswer == QuizItem.QuizSticker.CorrectAnswer)
                        {
                            // #FF4CFF85 sabze por rang
                            switch(QuizItem.QuizSticker.CorrectAnswer)
                            {
                                case 0:
                                    Answer1Grid.BorderBrush = greenColor;
                                    Answer1Grid.Background = greenColor;
                                    Answer1Ellipse.Stroke = whiteColor;
                                    Answer1OuterText.Foreground = whiteColor;
                                    Answer1OuterText.FontFamily = materialFont;
                                    Answer1OuterText.Text = Helper.CheckMaterialIcon;
                                    Answer1Text.Foreground = whiteColor;


                                    Answer2Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer2Ellipse.Stroke = redColor;
                                    Answer2OuterText.Foreground = redColor;
                                    Answer2OuterText.FontFamily = materialFont;
                                    Answer2OuterText.Text = Helper.XMaterialIcon;
                                    Answer2Text.Foreground = blackColor;


                                    Answer3Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer3Ellipse.Stroke = redColor;
                                    Answer3OuterText.Foreground = redColor;
                                    Answer3OuterText.FontFamily = materialFont;
                                    Answer3OuterText.Text = Helper.XMaterialIcon;
                                    Answer3Text.Foreground = blackColor;


                                    Answer4Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer4Ellipse.Stroke = redColor;
                                    Answer4OuterText.Foreground = redColor;
                                    Answer4OuterText.FontFamily = materialFont;
                                    Answer4OuterText.Text = Helper.XMaterialIcon;
                                    Answer4Text.Foreground = blackColor;
                                    break;

                                case 1:
                                    Answer2Grid.BorderBrush = greenColor;
                                    Answer2Grid.Background = greenColor;
                                    Answer2Ellipse.Stroke = whiteColor;
                                    Answer2OuterText.Foreground = whiteColor;
                                    Answer2OuterText.FontFamily = materialFont;
                                    Answer2OuterText.Text = Helper.CheckMaterialIcon;
                                    Answer2Text.Foreground = whiteColor;


                                    Answer1Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer1Ellipse.Stroke = redColor;
                                    Answer1OuterText.Foreground = redColor;
                                    Answer1OuterText.FontFamily = materialFont;
                                    Answer1OuterText.Text = Helper.XMaterialIcon;
                                    Answer1Text.Foreground = blackColor;


                                    Answer3Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer3Ellipse.Stroke = redColor;
                                    Answer3OuterText.Foreground = redColor;
                                    Answer3OuterText.FontFamily = materialFont;
                                    Answer3OuterText.Text = Helper.XMaterialIcon;
                                    Answer3Text.Foreground = blackColor;


                                    Answer4Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer4Ellipse.Stroke = redColor;
                                    Answer4OuterText.Foreground = redColor;
                                    Answer4OuterText.FontFamily = materialFont;
                                    Answer4OuterText.Text = Helper.XMaterialIcon;
                                    Answer4Text.Foreground = blackColor;
                                    break;

                                case 2:
                                    Answer3Grid.BorderBrush = greenColor;
                                    Answer3Grid.Background = greenColor;
                                    Answer3Ellipse.Stroke = whiteColor;
                                    Answer3OuterText.Foreground = whiteColor;
                                    Answer3OuterText.FontFamily = materialFont;
                                    Answer3OuterText.Text = Helper.CheckMaterialIcon;
                                    Answer3Text.Foreground = whiteColor;


                                    Answer1Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer1Ellipse.Stroke = redColor;
                                    Answer1OuterText.Foreground = redColor;
                                    Answer1OuterText.FontFamily = materialFont;
                                    Answer1OuterText.Text = Helper.XMaterialIcon;
                                    Answer1Text.Foreground = blackColor;


                                    Answer2Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer2Ellipse.Stroke = redColor;
                                    Answer2OuterText.Foreground = redColor;
                                    Answer2OuterText.FontFamily = materialFont;
                                    Answer2OuterText.Text = Helper.XMaterialIcon;
                                    Answer2Text.Foreground = blackColor;


                                    Answer4Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer4Ellipse.Stroke = redColor;
                                    Answer4OuterText.Foreground = redColor;
                                    Answer4OuterText.FontFamily = materialFont;
                                    Answer4OuterText.Text = Helper.XMaterialIcon;
                                    Answer4Text.Foreground = blackColor;
                                    break;

                                case 3:
                                    Answer4Grid.BorderBrush = greenColor;
                                    Answer4Grid.Background = greenColor;
                                    Answer4Ellipse.Stroke = whiteColor;
                                    Answer4OuterText.Foreground = whiteColor;
                                    Answer4OuterText.FontFamily = materialFont;
                                    Answer4OuterText.Text = Helper.CheckMaterialIcon;
                                    Answer4Text.Foreground = whiteColor;


                                    Answer1Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer1Ellipse.Stroke = redColor;
                                    Answer1OuterText.Foreground = redColor;
                                    Answer1OuterText.FontFamily = materialFont;
                                    Answer1OuterText.Text = Helper.XMaterialIcon;
                                    Answer1Text.Foreground = blackColor;


                                    Answer2Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer2Ellipse.Stroke = redColor;
                                    Answer2OuterText.Foreground = redColor;
                                    Answer2OuterText.FontFamily = materialFont;
                                    Answer2OuterText.Text = Helper.XMaterialIcon;
                                    Answer2Text.Foreground = blackColor;


                                    Answer3Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer3Ellipse.Stroke = redColor;
                                    Answer3OuterText.Foreground = redColor;
                                    Answer3OuterText.FontFamily = materialFont;
                                    Answer3OuterText.Text = Helper.XMaterialIcon;
                                    Answer3Text.Foreground = blackColor;
                                    break;
                            }
                        }
                        else
                        {
                            switch (QuizItem.QuizSticker.ViewerAnswer)
                            {
                                case 0:
                                    Answer1Grid.BorderBrush = redColor;
                                    Answer1Grid.Background = redColor;
                                    Answer1Ellipse.Stroke = whiteColor;
                                    Answer1OuterText.Foreground = whiteColor;
                                    Answer1OuterText.FontFamily = materialFont;
                                    Answer1OuterText.Text = Helper.XMaterialIcon;
                                    Answer1Text.Foreground = whiteColor;

                                    switch(QuizItem.QuizSticker.CorrectAnswer)
                                    {
                                        case 0:
                                            Answer1Grid.BorderBrush = greenLowColor;
                                            Answer1Grid.Background = greenLowColor;
                                            Answer1Ellipse.Stroke = whiteColor;
                                            Answer1OuterText.Foreground = whiteColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer1Text.Foreground = whiteColor;



                                            Answer2Grid.Background = whiteColor;
                                            Answer2Ellipse.Stroke = redColor;
                                            Answer2OuterText.Foreground = redColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.XMaterialIcon;
                                            Answer2Text.Foreground = blackColor;



                                            Answer3Grid.Background = whiteColor;
                                            Answer3Ellipse.Stroke = redColor;
                                            Answer3OuterText.Foreground = redColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.XMaterialIcon;
                                            Answer3Text.Foreground = blackColor;


                                            Answer4Grid.Background = whiteColor;
                                            Answer4Ellipse.Stroke = redColor;
                                            Answer4OuterText.Foreground = redColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.XMaterialIcon;
                                            Answer4Text.Foreground = blackColor;
                                            break;
                                        case 1:
                                            Answer2Grid.BorderBrush = greenLowColor;
                                            Answer2Grid.Background = greenLowColor;
                                            Answer2Ellipse.Stroke = whiteColor;
                                            Answer2OuterText.Foreground = whiteColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer2Text.Foreground = whiteColor;



                                            //Answer1Grid.Background = whiteColor;
                                            //Answer1Ellipse.Stroke = redColor;
                                            //Answer1OuterText.Foreground = redColor;
                                            //Answer1OuterText.FontFamily = materialFont;
                                            //Answer1OuterText.Text = Helper.XMaterialIcon;
                                            //Answer1Text.Foreground = blackColor;



                                            Answer3Grid.Background = whiteColor;
                                            Answer3Ellipse.Stroke = redColor;
                                            Answer3OuterText.Foreground = redColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.XMaterialIcon;
                                            Answer3Text.Foreground = blackColor;


                                            Answer4Grid.Background = whiteColor;
                                            Answer4Ellipse.Stroke = redColor;
                                            Answer4OuterText.Foreground = redColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.XMaterialIcon;
                                            Answer4Text.Foreground = blackColor;
                                            break;
                                        case 2:
                                            Answer3Grid.BorderBrush = greenLowColor;
                                            Answer3Grid.Background = greenLowColor;
                                            Answer3Ellipse.Stroke = whiteColor;
                                            Answer3OuterText.Foreground = whiteColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer3Text.Foreground = whiteColor;



                                            //Answer1Grid.Background = whiteColor;
                                            //Answer1Ellipse.Stroke = redColor;
                                            //Answer1OuterText.Foreground = redColor;
                                            //Answer1OuterText.FontFamily = materialFont;
                                            //Answer1OuterText.Text = Helper.XMaterialIcon;
                                            //Answer1Text.Foreground = blackColor;



                                            Answer2Grid.Background = whiteColor;
                                            Answer2Ellipse.Stroke = redColor;
                                            Answer2OuterText.Foreground = redColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.XMaterialIcon;
                                            Answer2Text.Foreground = blackColor;


                                            Answer4Grid.Background = whiteColor;
                                            Answer4Ellipse.Stroke = redColor;
                                            Answer4OuterText.Foreground = redColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.XMaterialIcon;
                                            Answer4Text.Foreground = blackColor;
                                            break;
                                        case 3:
                                            Answer4Grid.BorderBrush = greenLowColor;
                                            Answer4Grid.Background = greenLowColor;
                                            Answer4Ellipse.Stroke = whiteColor;
                                            Answer4OuterText.Foreground = whiteColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer4Text.Foreground = whiteColor;



                                            //Answer1Grid.Background = whiteColor;
                                            //Answer1Ellipse.Stroke = redColor;
                                            //Answer1OuterText.Foreground = redColor;
                                            //Answer1OuterText.FontFamily = materialFont;
                                            //Answer1OuterText.Text = Helper.XMaterialIcon;
                                            //Answer1Text.Foreground = blackColor;



                                            Answer2Grid.Background = whiteColor;
                                            Answer2Ellipse.Stroke = redColor;
                                            Answer2OuterText.Foreground = redColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.XMaterialIcon;
                                            Answer2Text.Foreground = blackColor;


                                            Answer3Grid.Background = whiteColor;
                                            Answer3Ellipse.Stroke = redColor;
                                            Answer3OuterText.Foreground = redColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.XMaterialIcon;
                                            Answer3Text.Foreground = blackColor;
                                            break;
                                    }

                                    break;










                                case 1:
                                    Answer2Grid.BorderBrush = redColor;
                                    Answer2Grid.Background = redColor;
                                    Answer2Ellipse.Stroke = whiteColor;
                                    Answer2OuterText.Foreground = whiteColor;
                                    Answer2OuterText.FontFamily = materialFont;
                                    Answer2OuterText.Text = Helper.XMaterialIcon;
                                    Answer2Text.Foreground = whiteColor;

                                    switch (QuizItem.QuizSticker.CorrectAnswer)
                                    {
                                        case 0:
                                            Answer1Grid.BorderBrush = greenLowColor;
                                            Answer1Grid.Background = greenLowColor;
                                            Answer1Ellipse.Stroke = whiteColor;
                                            Answer1OuterText.Foreground = whiteColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer1Text.Foreground = whiteColor;



                                            //Answer2Grid.Background = whiteColor;
                                            //Answer2Ellipse.Stroke = redColor;
                                            //Answer2OuterText.Foreground = redColor;
                                            //Answer2OuterText.FontFamily = materialFont;
                                            //Answer2OuterText.Text = Helper.XMaterialIcon;
                                            //Answer2Text.Foreground = blackColor;



                                            Answer3Grid.Background = whiteColor;
                                            Answer3Ellipse.Stroke = redColor;
                                            Answer3OuterText.Foreground = redColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.XMaterialIcon;
                                            Answer3Text.Foreground = blackColor;


                                            Answer4Grid.Background = whiteColor;
                                            Answer4Ellipse.Stroke = redColor;
                                            Answer4OuterText.Foreground = redColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.XMaterialIcon;
                                            Answer4Text.Foreground = blackColor;
                                            break;
                                        case 1:
                                            //Answer2Grid.Background = greenLowColor;
                                            //Answer2Ellipse.Stroke = whiteColor;
                                            //Answer2OuterText.Foreground = whiteColor;
                                            //Answer2OuterText.FontFamily = materialFont;
                                            //Answer2OuterText.Text = Helper.CheckMaterialIcon;
                                            //Answer2Text.Foreground = whiteColor;



                                            Answer1Grid.Background = whiteColor;
                                            Answer1Ellipse.Stroke = redColor;
                                            Answer1OuterText.Foreground = redColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.XMaterialIcon;
                                            Answer1Text.Foreground = blackColor;



                                            Answer3Grid.Background = whiteColor;
                                            Answer3Ellipse.Stroke = redColor;
                                            Answer3OuterText.Foreground = redColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.XMaterialIcon;
                                            Answer3Text.Foreground = blackColor;


                                            Answer4Grid.Background = whiteColor;
                                            Answer4Ellipse.Stroke = redColor;
                                            Answer4OuterText.Foreground = redColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.XMaterialIcon;
                                            Answer4Text.Foreground = blackColor;
                                            break;
                                        case 2:
                                            Answer3Grid.BorderBrush = greenLowColor;
                                            Answer3Grid.Background = greenLowColor;
                                            Answer3Ellipse.Stroke = whiteColor;
                                            Answer3OuterText.Foreground = whiteColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer3Text.Foreground = whiteColor;



                                            Answer1Grid.Background = whiteColor;
                                            Answer1Ellipse.Stroke = redColor;
                                            Answer1OuterText.Foreground = redColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.XMaterialIcon;
                                            Answer1Text.Foreground = blackColor;



                                            //Answer2Grid.Background = whiteColor;
                                            //Answer2Ellipse.Stroke = redColor;
                                            //Answer2OuterText.Foreground = redColor;
                                            //Answer2OuterText.FontFamily = materialFont;
                                            //Answer2OuterText.Text = Helper.XMaterialIcon;
                                            //Answer2Text.Foreground = blackColor;


                                            Answer4Grid.Background = whiteColor;
                                            Answer4Ellipse.Stroke = redColor;
                                            Answer4OuterText.Foreground = redColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.XMaterialIcon;
                                            Answer4Text.Foreground = blackColor;
                                            break;
                                        case 3:
                                            Answer4Grid.BorderBrush = greenLowColor;
                                            Answer4Grid.Background = greenLowColor;
                                            Answer4Ellipse.Stroke = whiteColor;
                                            Answer4OuterText.Foreground = whiteColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer4Text.Foreground = whiteColor;



                                            Answer1Grid.Background = whiteColor;
                                            Answer1Ellipse.Stroke = redColor;
                                            Answer1OuterText.Foreground = redColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.XMaterialIcon;
                                            Answer1Text.Foreground = blackColor;



                                            //Answer2Grid.Background = whiteColor;
                                            //Answer2Ellipse.Stroke = redColor;
                                            //Answer2OuterText.Foreground = redColor;
                                            //Answer2OuterText.FontFamily = materialFont;
                                            //Answer2OuterText.Text = Helper.XMaterialIcon;
                                            //Answer2Text.Foreground = blackColor;


                                            Answer3Grid.Background = whiteColor;
                                            Answer3Ellipse.Stroke = redColor;
                                            Answer3OuterText.Foreground = redColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.XMaterialIcon;
                                            Answer3Text.Foreground = blackColor;
                                            break;
                                    }

                                    break;











                                case 2:
                                    Answer3Grid.BorderBrush = redColor;
                                    Answer3Grid.Background = redColor;
                                    Answer3Ellipse.Stroke = whiteColor;
                                    Answer3OuterText.Foreground = whiteColor;
                                    Answer3OuterText.FontFamily = materialFont;
                                    Answer3OuterText.Text = Helper.XMaterialIcon;
                                    Answer3Text.Foreground = whiteColor;

                                    switch (QuizItem.QuizSticker.CorrectAnswer)
                                    {
                                        case 0:
                                            Answer1Grid.BorderBrush = greenLowColor;
                                            Answer1Grid.Background = greenLowColor;
                                            Answer1Ellipse.Stroke = whiteColor;
                                            Answer1OuterText.Foreground = whiteColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer1Text.Foreground = whiteColor;



                                            Answer2Grid.Background = whiteColor;
                                            Answer2Ellipse.Stroke = redColor;
                                            Answer2OuterText.Foreground = redColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.XMaterialIcon;
                                            Answer2Text.Foreground = blackColor;



                                            //Answer3Grid.Background = whiteColor;
                                            //Answer3Ellipse.Stroke = redColor;
                                            //Answer3OuterText.Foreground = redColor;
                                            //Answer3OuterText.FontFamily = materialFont;
                                            //Answer3OuterText.Text = Helper.XMaterialIcon;
                                            //Answer3Text.Foreground = blackColor;


                                            Answer4Grid.Background = whiteColor;
                                            Answer4Ellipse.Stroke = redColor;
                                            Answer4OuterText.Foreground = redColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.XMaterialIcon;
                                            Answer4Text.Foreground = blackColor;
                                            break;
                                        case 1:
                                            Answer2Grid.BorderBrush = greenLowColor;
                                            Answer2Grid.Background = greenLowColor;
                                            Answer2Ellipse.Stroke = whiteColor;
                                            Answer2OuterText.Foreground = whiteColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer2Text.Foreground = whiteColor;



                                            Answer1Grid.Background = whiteColor;
                                            Answer1Ellipse.Stroke = redColor;
                                            Answer1OuterText.Foreground = redColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.XMaterialIcon;
                                            Answer1Text.Foreground = blackColor;



                                            //Answer3Grid.Background = whiteColor;
                                            //Answer3Ellipse.Stroke = redColor;
                                            //Answer3OuterText.Foreground = redColor;
                                            //Answer3OuterText.FontFamily = materialFont;
                                            //Answer3OuterText.Text = Helper.XMaterialIcon;
                                            //Answer3Text.Foreground = blackColor;


                                            Answer4Grid.Background = whiteColor;
                                            Answer4Ellipse.Stroke = redColor;
                                            Answer4OuterText.Foreground = redColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.XMaterialIcon;
                                            Answer4Text.Foreground = blackColor;
                                            break;
                                        case 2:
                                            //Answer3Grid.Background = greenLowColor;
                                            //Answer3Ellipse.Stroke = whiteColor;
                                            //Answer3OuterText.Foreground = whiteColor;
                                            //Answer3OuterText.FontFamily = materialFont;
                                            //Answer3OuterText.Text = Helper.CheckMaterialIcon;
                                            //Answer3Text.Foreground = whiteColor;



                                            Answer1Grid.Background = whiteColor;
                                            Answer1Ellipse.Stroke = redColor;
                                            Answer1OuterText.Foreground = redColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.XMaterialIcon;
                                            Answer1Text.Foreground = blackColor;



                                            Answer2Grid.Background = whiteColor;
                                            Answer2Ellipse.Stroke = redColor;
                                            Answer2OuterText.Foreground = redColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.XMaterialIcon;
                                            Answer2Text.Foreground = blackColor;


                                            Answer4Grid.Background = whiteColor;
                                            Answer4Ellipse.Stroke = redColor;
                                            Answer4OuterText.Foreground = redColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.XMaterialIcon;
                                            Answer4Text.Foreground = blackColor;
                                            break;
                                        case 3:
                                            Answer4Grid.BorderBrush = greenLowColor;
                                            Answer4Grid.Background = greenLowColor;
                                            Answer4Ellipse.Stroke = whiteColor;
                                            Answer4OuterText.Foreground = whiteColor;
                                            Answer4OuterText.FontFamily = materialFont;
                                            Answer4OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer4Text.Foreground = whiteColor;



                                            Answer1Grid.Background = whiteColor;
                                            Answer1Ellipse.Stroke = redColor;
                                            Answer1OuterText.Foreground = redColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.XMaterialIcon;
                                            Answer1Text.Foreground = blackColor;



                                            Answer2Grid.Background = whiteColor;
                                            Answer2Ellipse.Stroke = redColor;
                                            Answer2OuterText.Foreground = redColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.XMaterialIcon;
                                            Answer2Text.Foreground = blackColor;


                                            //Answer3Grid.Background = whiteColor;
                                            //Answer3Ellipse.Stroke = redColor;
                                            //Answer3OuterText.Foreground = redColor;
                                            //Answer3OuterText.FontFamily = materialFont;
                                            //Answer3OuterText.Text = Helper.XMaterialIcon;
                                            //Answer3Text.Foreground = blackColor;
                                            break;
                                    }

                                    break;










                                case 3:
                                    Answer4Grid.BorderBrush = redColor;
                                    Answer4Grid.Background = redColor;
                                    Answer4Ellipse.Stroke = whiteColor;
                                    Answer4OuterText.Foreground = whiteColor;
                                    Answer4OuterText.FontFamily = materialFont;
                                    Answer4OuterText.Text = Helper.XMaterialIcon;
                                    Answer4Text.Foreground = whiteColor;

                                    switch (QuizItem.QuizSticker.CorrectAnswer)
                                    {
                                        case 0:
                                            Answer1Grid.BorderBrush = greenLowColor;
                                            Answer1Grid.Background = greenLowColor;
                                            Answer1Ellipse.Stroke = whiteColor;
                                            Answer1OuterText.Foreground = whiteColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer1Text.Foreground = whiteColor;



                                            Answer2Grid.Background = whiteColor;
                                            Answer2Ellipse.Stroke = redColor;
                                            Answer2OuterText.Foreground = redColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.XMaterialIcon;
                                            Answer2Text.Foreground = blackColor;



                                            Answer3Grid.Background = whiteColor;
                                            Answer3Ellipse.Stroke = redColor;
                                            Answer3OuterText.Foreground = redColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.XMaterialIcon;
                                            Answer3Text.Foreground = blackColor;


                                            //Answer4Grid.Background = whiteColor;
                                            //Answer4Ellipse.Stroke = redColor;
                                            //Answer4OuterText.Foreground = redColor;
                                            //Answer4OuterText.FontFamily = materialFont;
                                            //Answer4OuterText.Text = Helper.XMaterialIcon;
                                            //Answer4Text.Foreground = blackColor;
                                            break;
                                        case 1:
                                            Answer2Grid.BorderBrush = greenLowColor;
                                            Answer2Grid.Background = greenLowColor;
                                            Answer2Ellipse.Stroke = whiteColor;
                                            Answer2OuterText.Foreground = whiteColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer2Text.Foreground = whiteColor;



                                            Answer1Grid.Background = whiteColor;
                                            Answer1Ellipse.Stroke = redColor;
                                            Answer1OuterText.Foreground = redColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.XMaterialIcon;
                                            Answer1Text.Foreground = blackColor;



                                            Answer3Grid.Background = whiteColor;
                                            Answer3Ellipse.Stroke = redColor;
                                            Answer3OuterText.Foreground = redColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.XMaterialIcon;
                                            Answer3Text.Foreground = blackColor;


                                            //Answer4Grid.Background = whiteColor;
                                            //Answer4Ellipse.Stroke = redColor;
                                            //Answer4OuterText.Foreground = redColor;
                                            //Answer4OuterText.FontFamily = materialFont;
                                            //Answer4OuterText.Text = Helper.XMaterialIcon;
                                            //Answer4Text.Foreground = blackColor;
                                            break;
                                        case 2:
                                            Answer3Grid.BorderBrush = greenLowColor;
                                            Answer3Grid.Background = greenLowColor;
                                            Answer3Ellipse.Stroke = whiteColor;
                                            Answer3OuterText.Foreground = whiteColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.CheckMaterialIcon;
                                            Answer3Text.Foreground = whiteColor;



                                            Answer1Grid.Background = whiteColor;
                                            Answer1Ellipse.Stroke = redColor;
                                            Answer1OuterText.Foreground = redColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.XMaterialIcon;
                                            Answer1Text.Foreground = blackColor;



                                            Answer2Grid.Background = whiteColor;
                                            Answer2Ellipse.Stroke = redColor;
                                            Answer2OuterText.Foreground = redColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.XMaterialIcon;
                                            Answer2Text.Foreground = blackColor;


                                            //Answer4Grid.Background = whiteColor;
                                            //Answer4Ellipse.Stroke = redColor;
                                            //Answer4OuterText.Foreground = redColor;
                                            //Answer4OuterText.FontFamily = materialFont;
                                            //Answer4OuterText.Text = Helper.XMaterialIcon;
                                            //Answer4Text.Foreground = blackColor;
                                            break;
                                        case 3:
                                            //Answer4Grid.Background = greenLowColor;
                                            //Answer4Ellipse.Stroke = whiteColor;
                                            //Answer4OuterText.Foreground = whiteColor;
                                            //Answer4OuterText.FontFamily = materialFont;
                                            //Answer4OuterText.Text = Helper.CheckMaterialIcon;
                                            //Answer4Text.Foreground = whiteColor;



                                            Answer1Grid.Background = whiteColor;
                                            Answer1Ellipse.Stroke = redColor;
                                            Answer1OuterText.Foreground = redColor;
                                            Answer1OuterText.FontFamily = materialFont;
                                            Answer1OuterText.Text = Helper.XMaterialIcon;
                                            Answer1Text.Foreground = blackColor;



                                            Answer2Grid.Background = whiteColor;
                                            Answer2Ellipse.Stroke = redColor;
                                            Answer2OuterText.Foreground = redColor;
                                            Answer2OuterText.FontFamily = materialFont;
                                            Answer2OuterText.Text = Helper.XMaterialIcon;
                                            Answer2Text.Foreground = blackColor;


                                            Answer3Grid.Background = whiteColor;
                                            Answer3Ellipse.Stroke = redColor;
                                            Answer3OuterText.Foreground = redColor;
                                            Answer3OuterText.FontFamily = materialFont;
                                            Answer3OuterText.Text = Helper.XMaterialIcon;
                                            Answer3Text.Foreground = blackColor;
                                            break;
                                    }

                                    break;
                            }
                            //#FFFF4C4C ghermez
                            //#FFA7FFC3 sabze kamrang
                        }
                    }
                    else
                    {
                        if (/*!QuizItem.QuizSticker.ViewerCanAnswer || *//*!*/QuizItem.QuizSticker.Finished)
                        {
                            switch (QuizItem.QuizSticker.CorrectAnswer)
                            {
                                case 0:
                                    Answer1Grid.BorderBrush = greenColor;
                                    Answer1Grid.Background = greenColor;
                                    Answer1Ellipse.Stroke = whiteColor;
                                    Answer1OuterText.Foreground = whiteColor;
                                    Answer1OuterText.FontFamily = materialFont;
                                    Answer1OuterText.Text = Helper.CheckMaterialIcon;
                                    Answer1Text.Foreground = whiteColor;


                                    Answer2Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer2Ellipse.Stroke = redColor;
                                    Answer2OuterText.Foreground = redColor;
                                    Answer2OuterText.FontFamily = materialFont;
                                    Answer2OuterText.Text = Helper.XMaterialIcon;
                                    Answer2Text.Foreground = blackColor;


                                    Answer3Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer3Ellipse.Stroke = redColor;
                                    Answer3OuterText.Foreground = redColor;
                                    Answer3OuterText.FontFamily = materialFont;
                                    Answer3OuterText.Text = Helper.XMaterialIcon;
                                    Answer3Text.Foreground = blackColor;


                                    Answer4Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer4Ellipse.Stroke = redColor;
                                    Answer4OuterText.Foreground = redColor;
                                    Answer4OuterText.FontFamily = materialFont;
                                    Answer4OuterText.Text = Helper.XMaterialIcon;
                                    Answer4Text.Foreground = blackColor;
                                    break;

                                case 1:
                                    Answer2Grid.BorderBrush = greenColor;
                                    Answer2Grid.Background = greenColor;
                                    Answer2Ellipse.Stroke = whiteColor;
                                    Answer2OuterText.Foreground = whiteColor;
                                    Answer2OuterText.FontFamily = materialFont;
                                    Answer2OuterText.Text = Helper.CheckMaterialIcon;
                                    Answer2Text.Foreground = whiteColor;


                                    Answer1Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer1Ellipse.Stroke = redColor;
                                    Answer1OuterText.Foreground = redColor;
                                    Answer1OuterText.FontFamily = materialFont;
                                    Answer1OuterText.Text = Helper.XMaterialIcon;
                                    Answer1Text.Foreground = blackColor;


                                    Answer3Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer3Ellipse.Stroke = redColor;
                                    Answer3OuterText.Foreground = redColor;
                                    Answer3OuterText.FontFamily = materialFont;
                                    Answer3OuterText.Text = Helper.XMaterialIcon;
                                    Answer3Text.Foreground = blackColor;


                                    Answer4Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer4Ellipse.Stroke = redColor;
                                    Answer4OuterText.Foreground = redColor;
                                    Answer4OuterText.FontFamily = materialFont;
                                    Answer4OuterText.Text = Helper.XMaterialIcon;
                                    Answer4Text.Foreground = blackColor;
                                    break;

                                case 2:
                                    Answer3Grid.BorderBrush = greenColor;
                                    Answer3Grid.Background = greenColor;
                                    Answer3Ellipse.Stroke = whiteColor;
                                    Answer3OuterText.Foreground = whiteColor;
                                    Answer3OuterText.FontFamily = materialFont;
                                    Answer3OuterText.Text = Helper.CheckMaterialIcon;
                                    Answer3Text.Foreground = whiteColor;


                                    Answer1Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer1Ellipse.Stroke = redColor;
                                    Answer1OuterText.Foreground = redColor;
                                    Answer1OuterText.FontFamily = materialFont;
                                    Answer1OuterText.Text = Helper.XMaterialIcon;
                                    Answer1Text.Foreground = blackColor;


                                    Answer2Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer2Ellipse.Stroke = redColor;
                                    Answer2OuterText.Foreground = redColor;
                                    Answer2OuterText.FontFamily = materialFont;
                                    Answer2OuterText.Text = Helper.XMaterialIcon;
                                    Answer2Text.Foreground = blackColor;


                                    Answer4Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer4Ellipse.Stroke = redColor;
                                    Answer4OuterText.Foreground = redColor;
                                    Answer4OuterText.FontFamily = materialFont;
                                    Answer4OuterText.Text = Helper.XMaterialIcon;
                                    Answer4Text.Foreground = blackColor;
                                    break;

                                case 3:
                                    Answer4Grid.BorderBrush = greenColor;
                                    Answer4Grid.Background = greenColor;
                                    Answer4Ellipse.Stroke = whiteColor;
                                    Answer4OuterText.Foreground = whiteColor;
                                    Answer4OuterText.FontFamily = materialFont;
                                    Answer4OuterText.Text = Helper.CheckMaterialIcon;
                                    Answer4Text.Foreground = whiteColor;


                                    Answer1Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer1Ellipse.Stroke = redColor;
                                    Answer1OuterText.Foreground = redColor;
                                    Answer1OuterText.FontFamily = materialFont;
                                    Answer1OuterText.Text = Helper.XMaterialIcon;
                                    Answer1Text.Foreground = blackColor;


                                    Answer2Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer2Ellipse.Stroke = redColor;
                                    Answer2OuterText.Foreground = redColor;
                                    Answer2OuterText.FontFamily = materialFont;
                                    Answer2OuterText.Text = Helper.XMaterialIcon;
                                    Answer2Text.Foreground = blackColor;


                                    Answer3Grid.Background = new SolidColorBrush(Colors.White);
                                    Answer3Ellipse.Stroke = redColor;
                                    Answer3OuterText.Foreground = redColor;
                                    Answer3OuterText.FontFamily = materialFont;
                                    Answer3OuterText.Text = Helper.XMaterialIcon;
                                    Answer3Text.Foreground = blackColor;
                                    break;
                            }
                            Answer1Button.IsEnabled = Answer2Button.IsEnabled = Answer3Button.IsEnabled = Answer4Button.IsEnabled = false;
                        }
                        else
                        {
                            Answer1Button.IsEnabled = Answer2Button.IsEnabled = Answer3Button.IsEnabled = Answer4Button.IsEnabled = true;
                        }
                    }

                }
            }
            catch { }
        }

        private async void Answer1ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    SetHolding(true);
                    QuizItem.QuizSticker.ViewerAnswer = 0;
                    QuizItem.QuizSticker.Tallies[0].Count++;
                    ShowCorrectAnswerIfExists();
                    var result = await Helper.InstaApi.StoryProcessor
                    .AnswerToStoryQuizAsync(StoryItem.Pk, QuizItem.QuizSticker.QuizId, 0);

                    SetHolding(false);
                });
            }
            catch { SetHolding(false); }
        }

        private async void Answer2ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    SetHolding(true);
                    QuizItem.QuizSticker.ViewerAnswer = 1;
                    QuizItem.QuizSticker.Tallies[1].Count++;
                    ShowCorrectAnswerIfExists();
                    var result = await Helper.InstaApi.StoryProcessor
                    .AnswerToStoryQuizAsync(StoryItem.Pk, QuizItem.QuizSticker.QuizId, 1);

                    SetHolding(false);
                });
            }
            catch { SetHolding(false); }
        }

        private async void Answer3ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    SetHolding(true);
                    QuizItem.QuizSticker.ViewerAnswer = 2;
                    QuizItem.QuizSticker.Tallies[2].Count++;
                    ShowCorrectAnswerIfExists();
                    var result = await Helper.InstaApi.StoryProcessor
                    .AnswerToStoryQuizAsync(StoryItem.Pk, QuizItem.QuizSticker.QuizId, 2);

                    SetHolding(false);
                });
            }
            catch { SetHolding(false); }
        }

        private async void Answer4ButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    SetHolding(true);
                    QuizItem.QuizSticker.ViewerAnswer = 3;
                    QuizItem.QuizSticker.Tallies[3].Count++;
                    ShowCorrectAnswerIfExists();
                    var result = await Helper.InstaApi.StoryProcessor
                    .AnswerToStoryQuizAsync(StoryItem.Pk, QuizItem.QuizSticker.QuizId, 3);

                    SetHolding(false);
                });
            }
            catch { SetHolding(false); }
        }
        void SetHolding(bool flag)
        {
            try
            {
                if (Helpers.NavigationService.Frame.Content is Views.Main.StoryView story && story != null)
                    story.IsHolding = flag;
            }
            catch { }
        }
    }
}
