﻿using System;
using TellMe.Core.Contracts;
using TellMe.Core.DTO;
using TellMe.Core.Services;
using TellMe.Core.Validation;

namespace TellMe.Core
{
    public sealed class App
    {
        private static readonly Lazy<App> lazy = new Lazy<App>(() => new App());
        public static App Instance => lazy.Value;

        private App()
        {
        }

        public void Initialize(IApplicationDataStorage dataStorage)
        {
            DataStorage = dataStorage;
        }

        public IApplicationDataStorage DataStorage { get; private set; }

        public event Action<Exception> OnException;
        public event Action<Exception> OnNetworkException;


        public void LogException(Exception ex)
        {
            OnException?.Invoke(ex);
        }

		public void LogNetworkException(Exception ex)
		{
            OnNetworkException?.Invoke(ex);
		}
    }
}
