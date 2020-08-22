using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minista.Models;
using Minista.Models.Main;
using System.Diagnostics;
using System.Threading;
using static Helper;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using InstagramApiSharp;
using Minista.ItemsGenerators;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Minista.ViewModels.Direct;
using InstagramApiSharp.Enums;
#pragma warning disable IDE0044
#pragma warning disable IDE0051
#pragma warning disable IDE0052
#pragma warning disable IDE0060
#pragma warning disable CS0618
namespace Minista.ViewModels.Main
{
    public class ExploreViewModel : BaseModel
    {
        public ExploreClusterGenerator ExploreGenerator { get; set; } = new ExploreClusterGenerator();
    }
}
