using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Features.Tests;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [TestCaseOrderer("Features.Tests.PriorityOrderer", "Features.Tests")]
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class UsuarioTestes
    {
        private readonly IntegrationTestsFixture<StartupWebTests> _testsFixture;

        public UsuarioTestes(IntegrationTestsFixture<StartupWebTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Register User"), TestPriority(1)]
        [Trait("Category", "Web Integration - User")]
        public async Task User_Register_ShouldRegisterUser()
        {
            // Arrange
            _testsFixture.GerarUserSenha();
            string email = _testsFixture.UsuarioEmail;
            string pass = _testsFixture.UsuarioSenha;

            var inicitalResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Register");
            inicitalResponse.EnsureSuccessStatusCode();

            var antiForgeryToken =
                _testsFixture.ObterAntiForgeryToken(await inicitalResponse.Content.ReadAsStringAsync());

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


        [Fact(DisplayName = "Register User Weak Password")]
        [Trait("Category", "Web Integration - User"), TestPriority(3)]
        public async Task User_RegisterWeakPass_ShouldNotRegisterUser()
        {
            // Arrange
            _testsFixture.GerarUserSenha();
            string email = _testsFixture.UsuarioEmail;
            string pass = "123456"; //weak password

            var inicitalResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Register");
            inicitalResponse.EnsureSuccessStatusCode();

            var antiForgeryToken =
                _testsFixture.ObterAntiForgeryToken(await inicitalResponse.Content.ReadAsStringAsync());

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
            Assert.Contains($"Passwords must have at least one non alphanumeric character.", responseString);
            Assert.Contains($"Passwords must have at least one lowercase", responseString);
            Assert.Contains($"Passwords must have at least one uppercase", responseString);
        }


        [Fact(DisplayName = "Login User"), TestPriority(2)]
        [Trait("Category", "Web Integration - User")]
        public async Task User_LoginUser_ShouldLogin()
        {
            // Arrange
            string email = _testsFixture.UsuarioEmail;
            string pass = _testsFixture.UsuarioSenha;

            var inicitalResponse = await _testsFixture.Client.GetAsync("/Identity/Account/Login");
            inicitalResponse.EnsureSuccessStatusCode();

            var antiForgeryToken =
                _testsFixture.ObterAntiForgeryToken(await inicitalResponse.Content.ReadAsStringAsync());

            var formData = new Dictionary<string, string>
            {
                {_testsFixture.AntiForgeryFieldName, antiForgeryToken},
                {"Input.Email", email},
                {"Input.Password", pass}
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Login")
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