using InstagramApiSharp.Classes.Models;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Minista.UserControls.Main
{
    public sealed partial class MediaTagUc : UserControl
    {
        public InstaUserTag UserTag;
        public MediaTagUc()
        {
            this.InitializeComponent();
        }
        public void SetUserTag(InstaUserTag tag)
        {
            UserTag = tag;
            txtUsername.Text = "@" + tag.User.UserName.ToLower();
            //Width = txtUsername.ActualWidth + 4;
            //Height = txtUsername.ActualHeight + 4;
        }
        public MediaTagUc TrashItem = null;
    }
}
