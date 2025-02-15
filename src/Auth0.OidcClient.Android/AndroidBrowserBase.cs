﻿using Android.App;
using Android.Content;
using IdentityModel.OidcClient.Browser;
using System;
using System.Threading.Tasks;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Provides common IBrowser logic for Android.
    /// </summary>
    public abstract class AndroidBrowserBase : IBrowser
    {
        protected Context context;
        protected bool IsNewTask;

        public AndroidBrowserBase(Context context = null)
        {
            this.context = context;
            IsNewTask = context == null;
        }

        /// <inheritdoc/>
        public Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.StartUrl))
                throw new ArgumentException("Missing StartUrl", nameof(options));

            if (string.IsNullOrWhiteSpace(options.EndUrl))
                throw new ArgumentException("Missing EndUrl", nameof(options));

            var tcs = new TaskCompletionSource<BrowserResult>();

            void Callback(string response)
            {
                ActivityMediator.Instance.ActivityMessageReceived -= Callback;

                var cancelled = response == "UserCancel";
                tcs.SetResult(new BrowserResult
                {
                    ResultType = cancelled ? BrowserResultType.UserCancel : BrowserResultType.Success,
                    Response = response
                });
            }

            ActivityMediator.Instance.ActivityMessageReceived += Callback;

            OpenBrowser(Android.Net.Uri.Parse(options.StartUrl), context ?? Application.Context);

            return tcs.Task;
        }

        /// <summary>
        /// Open a web browser with the given uri.
        /// </summary>
        /// <param name="uri">The uri address to open in the browser.</param>
        protected abstract void OpenBrowser(Android.Net.Uri uri, Context context = null);
    }
}