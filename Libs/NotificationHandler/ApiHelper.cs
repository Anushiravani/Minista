using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;

namespace NotificationHandler
{
    public static class ApiHelper
    {
        public static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        public static List<IInstaApi> InstaApiList { get; set; } = new List<IInstaApi>();
        internal static DebugLogger DebugLogger;
        public static IInstaApi BuildApi(string username = null, string password = null)
        {
            UserSessionData sessionData;
            if (string.IsNullOrEmpty(username))
                sessionData = UserSessionData.ForUsername("FAKEUSER").WithPassword("FAKEPASS");
            else
                sessionData = new UserSessionData { UserName = username, Password = password };

            DebugLogger = new DebugLogger(LogLevel.All);
            var api = InstaApiBuilder.CreateBuilder()
                      .SetUser(sessionData)

#if DEBUG
                  .UseLogger(DebugLogger)
#endif

                      .Build();
            api.SetTimeout(TimeSpan.FromMinutes(2));

            //InstaApi = api;
            return api;
        }

        public static async Task Load()
        {
            try
            {
                var files = await LocalFolder.GetFilesAsync();
                if (files?.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var item = files[i];
                        if (item.Path.ToLower().EndsWith(".mises"))
                        {
                            try
                            {
                                var json = await FileIO.ReadTextAsync(item);
                                if (!string.IsNullOrEmpty(json))
                                {
                                    var content = Decrypt(json);
                                    var api = BuildApi();
                                    await api.LoadStateDataFromStringAsync(content);
                                    InstaApiList.Add(api);
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }
        const string Abc = "x*kKG$js=VmnfxQ@VB=su+_j9G6_TsY=E_Bjx3";
        static string Decrypt(string encryptedString, string key = null)
        {
            try
            {
                if (string.IsNullOrEmpty(key)) key = Abc;
                var hashKey = GetMD5Hash(key);
                IBuffer decryptBuffer = CryptographicBuffer.DecodeFromBase64String(encryptedString);
                var AES = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
                var symmetricKey = AES.CreateSymmetricKey(hashKey);
                var decryptedBuffer = CryptographicEngine.Decrypt(symmetricKey, decryptBuffer, null);
                string decryptedString = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, decryptedBuffer);
                return decryptedString;
            }
            catch (Exception)
            {
                //ex.PrintException("DecryptException");
                return "";
            }
        }
        private static IBuffer GetMD5Hash(string key)
        {
            IBuffer bufferUTF8Msg = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer hashBuffer = hashAlgorithmProvider.HashData(bufferUTF8Msg);
            if (hashBuffer.Length != hashAlgorithmProvider.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }
            return hashBuffer;
        }
    }
}
