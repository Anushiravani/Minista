using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minista.ItemsGenerators;
namespace Minista.ViewModels
{
    public class TVSearchViewModel : BaseModel
    {
        public TVSearchItemsGenerator Suggested { get; set; } = new TVSearchItemsGenerator();
        public TVSearchItemsGenerator SearchItems { get; set; } = new TVSearchItemsGenerator();

        public async void FirstLoad()
        {
            await Suggested.RunLoadMoreAsync();
        }
    }
}
