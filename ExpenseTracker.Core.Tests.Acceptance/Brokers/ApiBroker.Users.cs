﻿using ExpenseTracker.Core.Models.Users;
using RESTFulSense.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        public async ValueTask<User> PostUserAsync(User user)
        {
            var registerRequest = new UserRequest
            {
                Email = user.Email,
                Password = user.PasswordHash
            };

            await this.apiFactoryClient.PostContentWithNoResponseAsync("/register", registerRequest, "application/json");

            return user;
        }

        public async ValueTask<User> LoginUserAsync(User user)
        {
            var loginRequest =
                new UserRequest { Email = user.Email, Password = "*1Mar1988#"/*user.PasswordHash*/ };

            UserResponse response =
                await this.apiFactoryClient.PostContentAsync<UserRequest, UserResponse>("/login", loginRequest, "application/json");

            ConfigureHttpClient(response.AccessToken);

            return user;
        }

        private void ConfigureHttpClient(string accessToken)
        {
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

    }

    public class UserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserResponse
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}
