using Windows.ApplicationModel.Resources.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;


namespace Minista
{
    public static class DeviceUtil
    {
        public static bool IsDesktop => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop";

        public static bool IsMobile => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";

        public static bool IsIoT => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.IoT";

        public static bool IsXbox => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox";

        private static string[] GetDeviceOsVersion()
        {
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            return new string[] { v1.ToString(), v2.ToString(), v3.ToString(), v4.ToString() };
        }

        public static string OSVersion
        {
            get
            {
                string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                ulong v = ulong.Parse(sv);
                ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                ulong v4 = (v & 0x000000000000FFFFL);
                return $"{v1}.{v2}.{v3}.{v4}";
            }
        }
        public static bool OverRS2OS
        {
            get
            {
                var versions = GetDeviceOsVersion();
                int.TryParse(versions[2], out int versionCode);

                return versionCode > 15063; // >=
            }
        }
    }
}
