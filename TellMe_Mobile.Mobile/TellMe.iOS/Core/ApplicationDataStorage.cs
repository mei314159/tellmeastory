using Foundation;
using Newtonsoft.Json;
using TellMe.Core.Contracts;
using TellMe.Core.DTO;

namespace TellMe.iOS.Core
{
	public class ApplicationDataStorage : IApplicationDataStorage
	{
		public AuthenticationInfoDTO AuthInfo
		{
			get
			{
				var data = NSUserDefaults.StandardUserDefaults.StringForKey(nameof(IApplicationDataStorage.AuthInfo));

				if (!string.IsNullOrWhiteSpace(data))
				{
					var result = JsonConvert.DeserializeObject<AuthenticationInfoDTO>(data);
					return result;
				}

				return null;
			}
			set
			{
				if (value == null)
				{
					NSUserDefaults.StandardUserDefaults.RemoveObject(nameof(IApplicationDataStorage.AuthInfo));
				}
				else
				{
					var data = JsonConvert.SerializeObject(value);
					NSUserDefaults.StandardUserDefaults.SetString(data, nameof(IApplicationDataStorage.AuthInfo));
					NSUserDefaults.StandardUserDefaults.Synchronize();
				}
			}
		}

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
	}
}
