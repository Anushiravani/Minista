using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Enums;
using System.Collections.ObjectModel;
using Minista.ItemsGenerators;
using Windows.UI.Core;
using Minista.UserControls.Direct;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using static Helper;
namespace Minista.ViewModels.Direct
{
    public class DirectRequestsViewModel : BaseModel
    {
        public ObservableCollection<DirectInboxUc> Items { get; set; } = new ObservableCollection<DirectInboxUc>();
        public PaginationParameters Pagination { get; set; } = PaginationParameters.MaxPagesToLoad(1);
        public bool HasMoreItems { get; set; } = true;
        //bool FirstTime = true;
        ScrollViewer Scroll;
        private bool IsLoading = false;
        public void SetLV(/*ListView listView*/ ScrollViewer scroll)
        {
            //Scroll = listView.FindScrollViewer();
            if (Scroll == null)
            {
                Scroll = scroll;

                Scroll.ViewChanging += ScrollViewChanging;
            }
            //HasMoreItems = true;
            //IsLoading = true;
            //Pagination = PaginationParameters.MaxPagesToLoad(1);
        }
        public async void RunLoadMore(bool refresh = false)
        {
            await RunLoadMoreAsync(refresh);
        }
        public async Task RunLoadMoreAsync(bool refresh)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await LoadMoreItemsAsync(refresh);
            });
        }
        async Task LoadMoreItemsAsync(bool refresh = false)
        {
            if (!HasMoreItems && !refresh)
            {
                IsLoading = false;
                return;
            }
            try
            {
                if (refresh)
                {
                    Pagination = PaginationParameters.MaxPagesToLoad(2);

                    Views.Direct.DirectRequestsView.Current?.ShowTopLoading();
                }
                else
                    Views.Direct.DirectRequestsView.Current?.ShowBottomLoading();

                var result = await InstaApi.MessagingProcessor.GetPendingDirectAsync(Pagination);
                if (!result.Succeeded)
                {
                    if (result.Value == null)
                    {
                        IsLoading = false;

                        if (refresh)
                            Views.Direct.DirectRequestsView.Current?.HideTopLoading();
                        else
                            Views.Direct.DirectRequestsView.Current?.HideBottomLoading();
                        return;
                    }
                }
                HasMoreItems = result.Value.Inbox.HasOlder;

                if (result.Value?.Inbox?.Threads?.Count > 0)
                {
                    if (refresh)
                        Items.Clear();

                        for (int i = 0; i < result.Value.Inbox.Threads.Count; i++)
                            Items.Add(GetDirectInboxUc(result.Value.Inbox.Threads[i]));
                    MainPage.Current?.SetDirectMessageCount(result.Value);
                }
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                ex.PrintException("DirectRequestsViewModel.LoadMoreItemsAsync");
            }

            if (refresh)
                Views.Direct.DirectRequestsView.Current?.HideTopLoading();
            else
                Views.Direct.DirectRequestsView.Current?.HideBottomLoading();
            IsLoading = false;

        }





        private void ScrollViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                if (Items == null)
                    return;
                if (!Items.Any())
                    return;
                ScrollViewer view = sender as ScrollViewer;

                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (progress > 0.95 && IsLoading == false)
                {
                    IsLoading = true;
                    RunLoadMore();
                }
            }
            catch (Exception ex) { ex.PrintException("ScrollViewChanging"); }
        }

        readonly Random Rnd = new Random();

        DirectInboxUc GetDirectInboxUc(InstaDirectInboxThread thread)
        {
            return new DirectInboxUc
            {
                Name = (Rnd.Next(11111, 999999) + Rnd.Next(10000, 999999)).ToString(),
                Thread = thread
            };
        }
    }
}
