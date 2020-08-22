using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace Minista
{
    //This class caches and provides information about the supported
    // Windows.Foundation.UniversalApiContract of the runtime
    public class RuntimeSupportedSDKs
    {
        public enum SDKVERSION
        {
            _10586 = 2,   // November Update (1511)
            _14393,       // Anniversary Update (1607)
            _15063,       // Creators Update (1703)
            _16299,       // Fall Creators Update
            _17134,       // Version 1803
            _17763        // Version 1810
        };

        public RuntimeSupportedSDKs()
        {
            AllSupportedSdkVersions = new List<SDKVERSION>();

            // Determine which versions of the SDK are supported on the runtime
            foreach (SDKVERSION v in Enum.GetValues(typeof(SDKVERSION)))
            {
                if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", (ushort)Convert.ToInt32(v)))
                {
                    AllSupportedSdkVersions.Add(v);
                }
            }
        }
        public bool IsSdk16299() => IsSdkVersionRuntimeSupported(SDKVERSION._16299);
        public bool IsSdkVersionRuntimeSupported(SDKVERSION sdkVersion)
        {
            if (AllSupportedSdkVersions.Contains(sdkVersion))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<SDKVERSION> AllSupportedSdkVersions { get; }
    }
}
