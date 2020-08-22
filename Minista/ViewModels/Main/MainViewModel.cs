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
using Minista.Views.Main;
using InstagramApiSharp.Helpers;

namespace Minista.ViewModels.Main
{
    public class MainViewModel : BaseModel
    {
        public Orientation StoryOrientation { get; set; } = Orientation.Horizontal;
        public ObservableCollection<StoryModel> Stories { get; set; } = new ObservableCollection<StoryModel>();
        public ObservableCollection<StoryWithLiveSupportModel> StoriesX { get; set; } = new ObservableCollection<StoryWithLiveSupportModel>();
        public MainPostsGenerator PostsGenerator { get; set; } = new MainPostsGenerator();

        Visibility _storeisVisibility = Visibility.Collapsed;
        public Visibility StoreisVisibility { get { return _storeisVisibility; } set { _storeisVisibility = value; OnPropertyChanged("StoreisVisibility"); } }
        public async void FirstRun(bool refresh = false)
        {
            //var u = await InstaApi.UserProcessor.GetUserAsync("rmt4006");
            //var s = await InstaApi.StoryProcessor.GetUserStoryAndLivesAsync(u.Value.Pk);

            //var t = "";
            //var live = await InstaApi.LiveProcessor.GetInfoAsync(t);
            //var user = await InstaApi.UserProcessor.GetUserInfoByIdAsync(15154078079);
            //var sss = await InstaApi.CreativeProcessor.GetAssetsAsync();

            ////var inb = await Helper.InstaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
            //var user = await InstaApi.UserProcessor.GetUserAsync("instagram");
            //var sss = await InstaApi.MessagingProcessor.GetThreadByParticipantsAsync(0, user.Value.Pk);
            //var aabc = await InstaApi.AccountProcessor.GetNotificationsSettingsAsync("notifications");
            ////var aabc = await InstaApi.AccountProcessor.GetNotificationsSettingsAsync("email_and_sms");
            //return;
            /*await*/

            var aaabx = await InstaApi.UserProcessor.GetFullUserInfoAsync(30808156545);
            var aaabxxxxxx = await InstaApi.UserProcessor.GetFullUserInfoAsync(InstaApi.GetLoggedUser().LoggedInUser.Pk);
            RefreshStories(refresh);
            //await Task.Delay(3500);
            //await PostsGenerator.RunLoadMoreAsync(refresh);
            PostsGenerator.RunLoadMore(refresh);
            InboxViewModel.ResetInstance();
            if (InboxViewModel.Instance != null)
            {
                try
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        if (InboxViewModel.Instance.SeqId <= 0)
                            await InboxViewModel.Instance.RunLoadMoreAsync(refresh);
                        if (MainPage.Current.RealtimeClient == null)
                            MainPage.Current.RealtimeClient = new InstagramApiSharp.API.RealTime.RealtimeClient(InstaApi);
                        var client = MainPage.Current.RealtimeClient;

                        await client.Start(InboxViewModel.Instance.SeqId, InboxViewModel.Instance.SnapshotAt);
                        client.DirectItemChanged += InboxViewModel.Instance.RealtimeClientDirectItemChanged;
                        client.TypingChanged += InboxViewModel.Instance.RealtimeClientClientTypingChanged;


                    });
                }
                catch { }
            }
            MainView.Current?.MainViewInboxUc?.InboxVM?.RunLoadMore(refresh);
            ActivitiesViewModel.Instance?.RunLoadMore(refresh);
        }
        public async void RefreshStories(bool refresh = false)
        {
            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var stories = await InstaApi.StoryProcessor.GetStoryFeedWithPostMethodAsync(refresh);
                    if (stories.Succeeded)
                    {
                        Stories.Clear();
                        StoriesX.Clear();
                        var listX = new List<StoryWithLiveSupportModel>();
                        if (stories.Value.Broadcasts?.Count > 0)
                        {
                            for (int i = 0; i < stories.Value.Broadcasts.Count; i++)
                            {
                                var item = stories.Value.Broadcasts[i];
                                listX.Add(new StoryWithLiveSupportModel
                                {
                                    Broadcast = item,
                                    Type = StoryType.Broadcast
                                });
                            }
                        }
                        if (stories.Value.Items?.Count > 0)
                        {
                            var list = new List<StoryModel>();
                            string id = null;
                            for (int i = 0; i < stories.Value.Items.Count; i++)
                            {
                                var item = stories.Value.Items[i];
                                var m = item.ToStoryModel();
                                
                                if (string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(id) && id != item.Id)
                                {
                                    list.Add(m);
                                    listX.Add(new StoryWithLiveSupportModel
                                    {
                                        Story = item.ToStoryModel(),
                                        Type = StoryType.Story
                                    });
                                }
                                id = item.Id;
                            }
                            id = null;
                            Stories.AddRange(list);
                            if (stories.Value.Items?.Count > 0)
                            {
                                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                                {
                                    var users = new List<string>();
                                    foreach (var item in stories.Value.Items.Take(5))// 5ta ro migirim!
                                        //if (item.IsHashtag)
                                        //    users.Add(item.Owner.Pk.ToString());
                                        //else
                                            //users.Add(item.User.Pk.ToString());
                                            users.Add(item.Id);

                                    var storiesAfterResult = await InstaApi.StoryProcessor.GetUsersStoriesAsHighlightsAsync(users.ToArray());
                                    if (storiesAfterResult.Succeeded)
                                    {
                                        var storiesAfter = storiesAfterResult.Value.Items;
                                        for (int i = 0; i < Stories.Count; i++)
                                        {
                                            var item = Stories[i];
                                            //var single = storiesAfter.SingleOrDefault(ss => ss.User.Pk.ToString() == item.User.Pk.ToString());
                                            var single = storiesAfter.SingleOrDefault(ss => ss.Id == item.Id);
                                            if (single != null)
                                            {
                                                item.Items.Clear();
                                                item.Items.AddRange(single.Items);
                                            }
                                        }
                                    }
                                    StoreisVisibility = Visibility.Visible;
                                });

                            }
                        }
                        if (stories.Value.PostLives?.Count > 0)
                        {
                            for (int i = 0; i < stories.Value.PostLives.Count; i++)
                            {
                                var item = stories.Value.PostLives[i];
                                listX.Add(new StoryWithLiveSupportModel
                                {
                                    PostLives = item,
                                    Type = StoryType.PostLive
                                });
                            }
                        }

                        StoriesX.AddRange(listX);
                    }
                    else
                    {
                        if (stories.Info.ResponseType == ResponseType.LoginRequired)
                            MainPage.Current.LoggedOut();
                        if (Stories.Count == 0)
                            StoreisVisibility = Visibility.Collapsed;

                        if(stories.Info.ResponseType == ResponseType.ConsentRequired)
                        {
                            ShowNotify("Consent is required!\r\nLet Minista fix it for you ;-)\r\nTrying.... Give me 30 seconds maximum...", 3500);
                            await Task.Delay(TimeSpan.FromSeconds(8));

                            var acceptConsent = await InstaApi.AcceptConsentAsync();
                            await Task.Delay(TimeSpan.FromSeconds(15));
                            ShowNotify("Consent is fixed (I think) let me try to refresh feeds and other stuffs for u.", 2500);
                            MainView.Current?.TryToRefresh(true);
                        }
                    }
                });
            }
            catch(Exception ex) { ex.PrintException("MainViewModel.RefreshStories"); }
            refresh = false;
        }

    }
}
