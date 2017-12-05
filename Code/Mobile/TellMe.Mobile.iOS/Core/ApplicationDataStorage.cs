using Foundation;
using Newtonsoft.Json;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.iOS.Core
{
    public class ApplicationDataStorage : IApplicationDataStorage
    {
        public OsType OsType => OsType.iOS;

        public string AppVersion => NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();

        public T Get<T>(string key)
        {
            var data = NSUserDefaults.StandardUserDefaults.StringForKey(key);

            if (!string.IsNullOrWhiteSpace(data))
            {
                var result = JsonConvert.DeserializeObject<T>(data);
                return result;
            }

            return default(T);
        }

        public void Set<T>(string key, T value) where T : class
        {
            if (value == null)
            {
                NSUserDefaults.StandardUserDefaults.RemoveObject(key);
            }
            else
            {
                var data = JsonConvert.SerializeObject(value);
                NSUserDefaults.StandardUserDefaults.SetString(data, key);
                NSUserDefaults.StandardUserDefaults.Synchronize();
            }
        }


        public bool GetBool(string key)
        {
            return NSUserDefaults.StandardUserDefaults.BoolForKey(key);
        }

        public void SetBool(string key, bool value)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(value, key);
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }
    }
}