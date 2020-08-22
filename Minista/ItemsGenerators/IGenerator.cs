using InstagramApiSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minista.ItemsGenerators
{
    public interface IGenerator
    {
        //ObservableCollection<object> Items { get; set; }
        bool HasMoreItems { get; set; }
        PaginationParameters Pagination { get; }
        Task RunLoadMoreAsync(bool refresh);
    }
    public interface IIsMine
    { 
        bool IsMine { get; set; }
    }
}
