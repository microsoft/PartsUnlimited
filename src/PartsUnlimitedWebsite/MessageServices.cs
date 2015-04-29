// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace PartsUnlimited
{
    public static class MessageServices
    {
        public static Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service
            return Task.FromResult(0);
        }

        public static Task SendSmsAsync(string number, string message)
        {
            // Plug in your sms service
            return Task.FromResult(0);
        }
    }
}