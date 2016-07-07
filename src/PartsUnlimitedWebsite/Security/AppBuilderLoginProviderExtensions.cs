// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;

namespace PartsUnlimited.Security
{
    internal static class AppBuilderLoginProviderExtensions
    {
        public static void AddLoginProviders(this IApplicationBuilder app, ILoginProviders loginProviders)
        {
            if (loginProviders.Azure.Use)
            {
                app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions()
                {
                    ClientId = loginProviders.Azure.ClientId,
                    Authority = loginProviders.Azure.Authority
                });
            }

            if (loginProviders.Facebook.Use)
            {
                app.UseFacebookAuthentication(new FacebookOptions()
                {
                    AppId = loginProviders.Facebook.Key,
                    AppSecret = loginProviders.Facebook.Secret
                });
            }

            if (loginProviders.Google.Use)
            {
                app.UseGoogleAuthentication(new GoogleOptions()
                {
                    ClientId = loginProviders.Google.Key,
                    ClientSecret = loginProviders.Google.Secret
                });              
            }

            if (loginProviders.Twitter.Use)
            {
                app.UseTwitterAuthentication(new TwitterOptions() 
                {
                    ConsumerKey = loginProviders.Twitter.Key,
                    ConsumerSecret = loginProviders.Twitter.Secret
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

                app.UseMicrosoftAccountAuthentication(new MicrosoftAccountOptions()
                {
                    ClientId = loginProviders.Microsoft.Key,
                    ClientSecret = loginProviders.Microsoft.Secret
                });
            }
        }
    }
}