// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Builder;

namespace PartsUnlimited.Security
{
    internal static class AppBuilderLoginProviderExtensions
    {
        public static void AddLoginProviders(this IApplicationBuilder app, ILoginProviders loginProviders)
        {
            if (loginProviders.Azure.Use)
            {
                app.UseOpenIdConnectAuthentication(options =>
                {
                    options.ClientId = loginProviders.Azure.ClientId;
                    options.Authority = loginProviders.Azure.Authority;
                });
            }

            if (loginProviders.Facebook.Use)
            {
                app.UseFacebookAuthentication(options =>
                {
                    options.AppId = loginProviders.Facebook.Key;
                    options.AppSecret = loginProviders.Facebook.Secret;
                });
            }

            if (loginProviders.Google.Use)
            {
                app.UseGoogleAuthentication(options =>
                {
                    options.ClientId = loginProviders.Google.Key;
                    options.ClientSecret = loginProviders.Google.Secret;
                });
            }

            if (loginProviders.Twitter.Use)
            {
                app.UseTwitterAuthentication(options =>
                {
                    options.ConsumerKey = loginProviders.Twitter.Key;
                    options.ConsumerSecret = loginProviders.Twitter.Secret;
                });
            }

            if (loginProviders.Microsoft.Use)
            {
                //The MicrosoftAccount service has restrictions that prevent the use of http://localhost:5001/ for test applications.
                //As such, here is how to change this sample to uses http://ktesting.com:5001/ instead.

                //Edit the Project.json file and replace http://localhost:5001/ with http://ktesting.com:5001/.

                //From an admin command console first enter:
                // notepad C:\Windows\System32\drivers\etc\hosts
                //and add this to the file, save, and exit (and reboot?):
                // 127.0.0.1 ktesting.com

                //Then you can choose to run the app as admin (see below) or add the following ACL as admin:
                // netsh http add urlacl url=http://ktesting:12345/ user=[domain\user]

                //The sample app can then be run via:
                // k web

                app.UseMicrosoftAccountAuthentication(options =>
                {
                    options.ClientId = loginProviders.Microsoft.Key;
                    options.ClientSecret = loginProviders.Microsoft.Secret;
                });
            }
        }
    }
}