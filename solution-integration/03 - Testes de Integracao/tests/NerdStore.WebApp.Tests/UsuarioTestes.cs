using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class UsuarioTestes
    {
        private readonly IntegrationTestsFixture<StartupWebTests> _testsFixture;

        public UsuarioTestes(IntegrationTestsFixture<StartupWebTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Register User")]
        [Trait("Category", "Web Integration - User")]
        public async Task User_Register_ShouldRegisterUser()
        {
            // Arrange
            _testsFixture.GerarUserSenha();
            string email = _testsFixture.UsuarioEmail;
            string pass = _testsFixture.UsuarioSenha;

            var inicitalResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Register");
            inicitalResponse.EnsureSuccessStatusCode();

            var antiForgeryToken = _testsFixture.ObterAntiForgeryToken(await inicitalResponse.Content.ReadAsStringAsync());

            var formData = new Dictionary<string, string>
            {
                {_testsFixture.AntiForgeryFieldName, antiForgeryToken},
                {"Input.Email", email},
                {"Input.Password", pass},
                {"Input.ConfirmPassword", pass}
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Register")
            {
                Content = new FormUrlEncodedContent(formData)

            };

            // Act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            // Assert
            var responseString = await postResponse.Content.ReadAsStringAsync();

            postResponse.EnsureSuccessStatusCode();
            Assert.Contains($"Hello {email}", responseString);
        }
    }
}


