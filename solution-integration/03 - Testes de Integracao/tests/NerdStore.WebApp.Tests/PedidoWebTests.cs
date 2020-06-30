using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Features.Tests;
using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class PedidoWebTests
    {
        private readonly IntegrationTestsFixture<StartupWebTests> _testsFixture;

        public PedidoWebTests(IntegrationTestsFixture<StartupWebTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Add Item in New Order")]
        [Trait("Category", "Web Integration - Order")]
        public async Task Order_AddItemNewOrder_ShouldAddItemAndUpdateTotal()
        {
            // Arrange
            var productId = new Guid("4251ebd0-0e7e-4169-8ea6-50588d6f1714");
            const int quantity = 2;

            var inicitalResponse = await _testsFixture.Client.GetAsync($"/produto-detalhe/{productId}");
            inicitalResponse.EnsureSuccessStatusCode();

            var formData = new Dictionary<string, string>
            {
                {"Id", productId.ToString()},
                {"quantidade", quantity.ToString()}
            };

            await _testsFixture.RealizarLogin();

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/meu-carrinho")
            {
                Content = new FormUrlEncodedContent(formData)
            };

            // act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            // Assert
            var html = new HtmlParser()
                .ParseDocumentAsync(await postResponse.Content.ReadAsStringAsync())
                .Result
                .All;

            var formQuantity = html?.FirstOrDefault(c => c.Id == "quantidade")?.GetAttribute("value").ApenasNumeros();
            var valorUnitario = html?.FirstOrDefault(c => c.Id == "valorUnitario")?.TextContent.Split(".")[0]?.ApenasNumeros();
            var valorTotal = html?.FirstOrDefault(c => c.Id == "valorTotal")?.TextContent.Split(".")[0]?.ApenasNumeros();

            Assert.Equal(valorTotal, valorUnitario * formQuantity);

        }
    }
}
