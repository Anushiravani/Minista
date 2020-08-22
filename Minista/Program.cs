﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.System.Profile;

namespace Minista
{
    //public static class Program
    //{
    //    public static int InstanceNumber { get; set; }
    //    private static IList<AppInstance> instances;

    //    // For demo purposes, we're tracking live instances with a simple integer count.
    //    private static void UpdateSharedInstanceNumber()
    //    {
    //        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    //        object data = localSettings.Values["InstanceCount"];
    //        if (data == null)
    //        {
    //            localSettings.Values["InstanceCount"] = 0;
    //        }
    //        if (instances.Count == 0)
    //        {
    //            // Write data to settings, using this app instance's ID.
    //            // If there are no other instances, we reset the InstanceCount.
    //            localSettings.Values["InstanceCount"] = 1;
    //            InstanceNumber = 1;
    //        }
    //        else
    //        {
    //            // Read the settings data, and increment it.
    //            InstanceNumber = (int)localSettings.Values["InstanceCount"] + 1;
    //            localSettings.Values["InstanceCount"] = InstanceNumber;
    //        }
    //    }

    //    // Default code-gen unless we include DISABLE_XAML_GENERATED_MAIN in the build properties.
    //    //static void Main(string[] args)
    //    //{
    //    //    global::Windows.UI.Xaml.Application.Start((p) => new App());
    //    //}

    //    #region Main
    //    static void Main(string[] args)
    //    {
    //        if (IsRS1OS)
    //        {
    //            global::Windows.UI.Xaml.Application.Start((p) => new App());

    //            return;
    //        }
    //        else if (IsRS2OS)
    //        {
    //            global::Windows.UI.Xaml.Application.Start((p) => new App());
    //            return;
    //        }
    //        // First, get a list of all running instances of this app.
    //        instances = AppInstance.GetInstances();

    //        // Next, we'll get our rich activation event args.
    //        IActivatedEventArgs activatedArgs = AppInstance.GetActivatedEventArgs();

    //        // An app might want to set itself up for possible redirection in 
    //        // the case where it opens files - for example, to prevent multiple
    //        // instances from working on the same file.
    //        if (activatedArgs is FileActivatedEventArgs fileArgs)
    //        {
    //            // For simplicity, we'll only look at the first file.
    //            IStorageItem file = fileArgs.Files.FirstOrDefault();
    //            if (file != null)
    //            {
    //                // Let's try to register this instance for this file.
    //                var instance = AppInstance.FindOrRegisterInstanceForKey(file.Name);
    //                if (instance.IsCurrentInstance)
    //                {
    //                    // If we successfully registered this instance, we can now just
    //                    // go ahead and do normal XAML initialization.
    //                    UpdateSharedInstanceNumber();
    //                    global::Windows.UI.Xaml.Application.Start((p) => new App());
    //                }
    //                else
    //                {
    //                    // Some other instance registered for this file, so we'll 
    //                    // redirect this activation to that instance instead.
    //                    instance.RedirectActivationTo();
    //                }
    //            }
    //        }
    //        else
    //        {
    //            // The platform might provide a recommended instance.
    //            if (AppInstance.RecommendedInstance != null)
    //            {
    //                AppInstance.RecommendedInstance.RedirectActivationTo();
    //            }
    //            else
    //            {
    //                // If the platform hasn't expressed a preference, we need to examine all
    //                // other instances to see if any are suitable for redirecting this request.
    //                // In the simple case, any instance will do.
    //                //AppInstance instance = instances.FirstOrDefault();

    //                // If the app re-registers re-usable instances, we can filter for these instead.
    //                AppInstance instance = instances.Where((i) => i.Key.StartsWith("REUSABLE")).FirstOrDefault();
    //                if (instance != null)
    //                {
    //                    Debug.WriteLine($"instance = {instance.Key}");
    //                    instance.RedirectActivationTo();
    //                }
    //                else
    //                {
    //                    AppInstance.FindOrRegisterInstanceForKey("REUSABLE" + App.Id.ToString());
    //                    UpdateSharedInstanceNumber();
    //                    global::Windows.UI.Xaml.Application.Start((p) => new App());
    //                }
    //            }
    //        }
    //    }

    //    #endregion

    //    private static string[] GetDeviceOsVersion()
    //    {
    //        string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
    //        ulong v = ulong.Parse(sv);
    //        ulong v1 = (v & 0xFFFF000000000000L) >> 48;
    //        ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
    //        ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
    //        ulong v4 = (v & 0x000000000000FFFFL);
    //        return new string[] { v1.ToString(), v2.ToString(), v3.ToString(), v4.ToString() };
    //    }
    //    public static bool IsRS1OS
    //    {
    //        get
    //        {
    //            var versions = GetDeviceOsVersion();
    //            return versions[2] == "14393";
    //        }
    //    }

    //    public static bool IsRS2OS
    //    {
    //        get
    //        {
    //            var versions = GetDeviceOsVersion();
    //            return versions[2] == "15063";
    //        }
    //    }
    //}

}
