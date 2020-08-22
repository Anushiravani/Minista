﻿using System;
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

namespace Minista.ViewModels
{
    public class TVViewModel : BaseModel, IGenerator
    {
        public InstaTVSelfChannel MyChannel { get; set; }

        public bool FirstRun = true;
        public ObservableCollection<InstaMedia> Items { get; set; } = new ObservableCollection<InstaMedia>();
        public ObservableCollection<InstaMedia> ItemsX { get; set; } = new ObservableCollection<InstaMedia>();
        public bool HasMoreItems { get; set; } = true;
        public PaginationParameters Pagination { get; private set; } = PaginationParameters.MaxPagesToLoad(1);

        private int PageCount = 1;
        ScrollViewer Scroll;
        bool IsLoading = true;

        public void SetLV(ScrollViewer scrollViewer)
        {
            Scroll = scrollViewer;

            if (Scroll != null)
            {
                Scroll.ViewChanging += ScrollViewChanging;
                HasMoreItems = true;
                IsLoading = true;
            }
        }
        public void ResetCache()
        {
            try
            {
                MyChannel = null;
                FirstRun = true;
                Items.Clear();
                ItemsX.Clear();
                Pagination = PaginationParameters.MaxPagesToLoad(1);
                HasMoreItems = true;
                IsLoading = true;
            }
            catch { }
        }
        public async void RunLoadMore(bool refresh = false)
        {
            await RunLoadMoreAsync(refresh);
        }
        public async Task RunLoadMoreAsync(bool refresh = false)
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
                    PageCount = 1;
                    Pagination = PaginationParameters.MaxPagesToLoad(2);
                    try
                    {
                        Views.TV.TVView.Current?.ShowTopLoading();
                    }
                    catch { }
                    try
                    {
                        //Views.Posts.ScrollableExplorePostView.Current?.ShowTopLoading();
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        Views.TV.TVView.Current?.ShowBottomLoading();
                    }
                    catch { }
                    //try
                    //{
                    //    Views.Posts.ScrollableExplorePostView.Current?.ShowBottomLoading();
                    //}
                    //catch { }
                }

                var result = await InstaApi.TVProcessor.BrowseFeedAsync(Pagination);
                //if (DeviceUtil.IsMobile)
                Pagination.MaximumPagesToLoad = 2;
                PageCount++;
                FirstRun = false;
                if (!result.Succeeded)
                {
                    IsLoading = false;
                    if (result.Value == null || result.Value.BrowseItems?.Count == 0)
                    {
                        Hide(refresh);
                        return;
                    }
                }

                HasMoreItems = result.Value.MoreAvailable;

                Pagination.NextMaxId = result.Value.MaxId;
                if (refresh)
                {
                    Items.Clear();
                    ItemsX.Clear();
                }
                if (result.Value.BrowseItems?.Count > 0)
                {
                    //Items.AddRange(result.Value.Medias);
                    //ItemsX.AddRange(result.Value.Medias);
                    for (int i = 0; i < result.Value.BrowseItems.Count; i++)
                    {
                        var l = result.Value.BrowseItems[i];
                        if (!Items.Any(x => x.InstaIdentifier == l.InstaIdentifier))
                        {
                            Items.Add(l);
                            //ItemsX.Add(l);
                        }
                    }
                }
             
                if (result.Value.MyChannel != null)
                    MyChannel = result.Value.MyChannel;
                await Task.Delay(1000);
                IsLoading = false;
                if (refresh /*&& DeviceUtil.IsMobile*/)
                    RunLoadMore();
            }
            catch (Exception ex)
            {

                ex.PrintException("ExploreClusterGenerator.LoadMoreItemsAsync");
            }
            FirstRun = IsLoading = false;
            Hide(refresh);
        }

        void Hide(bool refresh)
        {
            if (refresh)
            {
                try
                {
                    Views.TV.TVView.Current?.HideTopLoading();
                }
                catch { }
            }
            else
            {
                try
                {
                    Views.TV.TVView.Current?.HideBottomLoading();
                }
                catch { }
              
            }
        }
        public void ScrollViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            try
            {
                if (Items == null)
                    return;
                if (!Items.Any())
                    return;
                ScrollViewer view = sender as ScrollViewer;

                double progress = view.VerticalOffset / view.ScrollableHeight;
                if (progress > 0.95 && IsLoading == false && !FirstRun)
                {
                    IsLoading = true;
                    RunLoadMore();
                }
            }
            catch (Exception ex) { ex.PrintException("Scroll_ViewChanging"); }
        }

    }
}
