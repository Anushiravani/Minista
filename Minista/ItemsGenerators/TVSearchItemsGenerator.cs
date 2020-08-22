using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minista.ItemsGenerators
{
    public class TVSearchItemsGenerator : BaseModel
    {
        public ObservableCollection<InstaTVSearchResult> Items { get; set; }
        string _searchWord;
        public string SearchWord
        {
            get { return _searchWord; }
            set { _searchWord = value; OnPropertyChanged("SearchWord"); }
        }
        public TVSearchItemsGenerator()
        {
            Items = new ObservableCollection<InstaTVSearchResult>();
        }
        public async Task RunLoadMoreAsync()
        {
            await Helper.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await LoadMoreItemsAsync();
            });
        }
        async Task LoadMoreItemsAsync()
        {
            try
            {
                IResult<InstaTVSearch> result;
                if (string.IsNullOrEmpty(SearchWord))
                    result = await Helper.InstaApi.TVProcessor.GetSuggestedSearchesAsync();
                else
                    result = await Helper.InstaApi.TVProcessor.SearchAsync(SearchWord);

                Items.Clear();
                if (!result.Succeeded)
                    return;
                if (result.Value.Results != null && result.Value.Results.Any())
                    Items.AddRange(result.Value.Results);


            }
            catch (Exception ex)
            {
                ex.PrintException("GenerateSearchItems.LoadMoreItemsAsync");
            }
        }
    }
}
