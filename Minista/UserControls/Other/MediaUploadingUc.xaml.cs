using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Minista.UserControls.Other
{
    public sealed partial class MediaUploadingUc : UserControl
    {
        private readonly DispatcherTimer Timer = new DispatcherTimer();
        const string TEXT = "Media is uploading";
        int Index = 0;
        readonly string[] Dots = new string[] { ".", "..", "...", "....", "....", };
        public MediaUploadingUc()
        {
            this.InitializeComponent();
            Timer.Interval = TimeSpan.FromMilliseconds(1000);
            Timer.Tick += TimerTick;
        }
        public void Start()
        {
            try
            {
                Timer.Start();
            }
            catch { }
        }
        public void Stop()
        {
            try
            {
                Timer.Stop();
            }
            catch { }
        }
        private void TimerTick(object sender, object e)
        {
            try
            {
                if (Index > Dots.Length)
                    Index = 0;
                MediaTest.Text = TEXT + Dots[Index];
                Index++;
            }
            catch { Index = 0; }
        }
    }
}
