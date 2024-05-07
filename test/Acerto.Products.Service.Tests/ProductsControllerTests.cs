using Acerto.Shared.Contracts;
using Acerto.Shared.Contracts.Messages;
using Acerto.Shared.Domain.Abstractions;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Acerto.Products.Service.Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturn200OK_And_EmptyResult_WhenProductsTableIsEmpty_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();

            // act
            var result = await context.Scope.Controller.GetAllAsync();

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<IEnumerable<ProductResponse>>>();

            var objectResult = result.As<ActionResult<IEnumerable<ProductResponse>>>().Result.As<OkObjectResult>();
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.As<IEnumerable<ProductResponse>>().Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn200OK_And_NoEmptyResult_WhenProductsTableContainsData_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();
            await context.SeedSampleDataAsync();

            // act
            using var scope = context.UseNewScope();
            var result = await scope.Controller.GetAllAsync();

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<IEnumerable<ProductResponse>>>();

            var objectResult = result.As<ActionResult<IEnumerable<ProductResponse>>>().Result.As<OkObjectResult>();

            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.As<IEnumerable<ProductResponse>>().Should().NotBeNull();
            objectResult.Value.As<IEnumerable<ProductResponse>>().Should().HaveCount(2);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 0)]

        public async Task GetPagedAsync_ShouldReturn400BadRequest_WhenPageOrPageSizeAreNotValid_Async(int page, int pageSize)
        {
            // arrange
            using var context = TestContext.UseEmptyDb();

            // act
            var result = await context.Scope.Controller.GetPagedAsync(page, pageSize);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<IPagedResult<ProductResponse>>>();

            var badRequestResult = result.As<ActionResult<IPagedResult<ProductResponse>>>().Result.As<BadRequestObjectResult>();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(ResultMessages.PageAndPageSizeErrorMessage);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]

        public async Task GetPagedAsync_ShouldReturn200OK_And_EmptyResult_WhenProductsTableIsEmpty_Async(int page, int pageSize)
        {
            // arrange
            using var context = TestContext.UseEmptyDb();

            // act
            var result = await context.Scope.Controller.GetPagedAsync(page, pageSize);

            // assert
            result.Should().NotBeNull();

            result.Should().BeOfType<ActionResult<IPagedResult<ProductResponse>>>();
            var objectResult = result.As<ActionResult<IPagedResult<ProductResponse>>>().Result.As<OkObjectResult>();

            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.As<IPagedResult<ProductResponse>>().Should().NotBeNull();
            objectResult.Value.As<IPagedResult<ProductResponse>>().CurrentPage.Should().Be(page);
            objectResult.Value.As<IPagedResult<ProductResponse>>().PageSize.Should().Be(pageSize);
            objectResult.Value.As<IPagedResult<ProductResponse>>().RowCount.Should().Be(0);
            objectResult.Value.As<IPagedResult<ProductResponse>>().Results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]

        public async Task GetPagedAsync_ShouldReturn200OK_And_NoEmptyResult_WhenProductsTableContainsData_Async(int page, int pageSize)
        {
            // arrange
            using var context = TestContext.UseEmptyDb();
            await context.SeedSampleDataAsync();

            // act
            using var scope = context.UseNewScope();
            var result = await context.Scope.Controller.GetPagedAsync(page, pageSize);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<IPagedResult<ProductResponse>>>();
            var objectResult = result.As<ActionResult<IPagedResult<ProductResponse>>>().Result.As<OkObjectResult>();

            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.As<IPagedResult<ProductResponse>>().Should().NotBeNull();
            objectResult.Value.As<IPagedResult<ProductResponse>>().CurrentPage.Should().Be(page);
            objectResult.Value.As<IPagedResult<ProductResponse>>().PageSize.Should().Be(pageSize);
            objectResult.Value.As<IPagedResult<ProductResponse>>().RowCount.Should().Be(2);
            objectResult.Value.As<IPagedResult<ProductResponse>>().Results.Should().HaveCount(pageSize);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn404NotFound_WithNoBodyResponse_WhenTheGivenProductDoesNotExist_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();

            // act
            using var scope = context.UseNewScope();
            var productId = Guid.NewGuid();
            var result = await context.Scope.Controller.GetByIdAsync(productId);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ProductResponse>>();
            var notFoundResult = result.As<ActionResult<ProductResponse>>().Result.As<NotFoundResult>();
            notFoundResult.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn200OK_With_ProductResponse_WhenTheGivenProductExists_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();
            await context.SeedSampleDataAsync();

            // act
            using var scope = context.UseNewScope();
            var productId = TestContext.GetSampleData().First().Id;
            var result = await context.Scope.Controller.GetByIdAsync(productId);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ProductResponse>>();
            var objectResult = result.As<ActionResult<ProductResponse>>().Result.As<OkObjectResult>();
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.As<ProductResponse>().Should().NotBeNull();
            objectResult.Value.As<ProductResponse>().Id.Should().Be(productId);
        }

        [Fact]
        public async Task PostAsync_ShouldReturn422UnprocessableEntity_WhenProductCreationRequestIsNotValid_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();

            // act
            var product = new ProductRequest(Guid.Empty, "Samsung S23 Ultra", "Samsung", 0);
            var result = await context.Scope.Controller.PostAsync(product);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ProductResponse>>();
            var unprocessableResult = result.As<ActionResult<ProductResponse>>().Result.As<UnprocessableEntityObjectResult>();
            unprocessableResult.StatusCode.Should().Be(422);

            unprocessableResult.Value.As<ValidationResult>().Errors.Should().HaveCount(1);
            unprocessableResult.Value.As<ValidationResult>().Errors.First().PropertyName.Should().Be(nameof(ProductRequest.Price));
            unprocessableResult.Value.As<ValidationResult>().Errors.First().ErrorMessage.Should().Be(ResultMessages.PriceMustBeGreaterThanZero);
        }

        [Fact]
        public async Task PostAsync_ShouldReturn200OK_WhenProductCreationRequestIsValid_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();

            // act
            var product = new ProductRequest(Guid.Empty, "Samsung S23 Ultra", "Samsung", 3_000);
            var result = await context.Scope.Controller.PostAsync(product);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ProductResponse>>();
            var objectResult = result.As<ActionResult<ProductResponse>>().Result.As<OkObjectResult>();

            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.As<ProductResponse>().Should().NotBeNull();
            objectResult.Value.As<ProductResponse>().Id.Should().NotBeEmpty();
            objectResult.Value.As<ProductResponse>().Name.Should().Be(product.Name);
            objectResult.Value.As<ProductResponse>().Brand.Should().Be(product.Brand);
            objectResult.Value.As<ProductResponse>().Price.Should().Be(product.Price);
            objectResult.Value.As<ProductResponse>().Description.Should().Be(product.Description);
            objectResult.Value.As<ProductResponse>().Color.Should().Be(product.Color);
        }

        [Fact]
        public async Task PutAsync_ShouldReturn422UnprocessableEntity_WhenProductUpdateRequestIsNotValid_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();
            await context.SeedSampleDataAsync();

            // act - update price to 0
            using var updateScope = context.UseNewScope();
            var productId = TestContext.GetSampleData().First().Id;
            var product = new ProductRequest(productId, "Samsung Galaxy S24 Ultra", "Samsung", 0);
            var result = await context.Scope.Controller.PutAsync(product);

            // assert
            result.Should().BeOfType<ActionResult<ProductResponse>>();
            var unprocessableResult = result.As<ActionResult<ProductResponse>>().Result.As<UnprocessableEntityObjectResult>();
            unprocessableResult.StatusCode.Should().Be(422);

            unprocessableResult.Value.As<ValidationResult>().Errors.Should().HaveCount(1);
            unprocessableResult.Value.As<ValidationResult>().Errors.First().PropertyName.Should().Be(nameof(ProductRequest.Price));
            unprocessableResult.Value.As<ValidationResult>().Errors.First().ErrorMessage.Should().Be(ResultMessages.PriceMustBeGreaterThanZero);
        }

        [Fact]
        public async Task PucAsync_ShouldReturn200OK_WhenProductCreationRequestIsValid_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();
            await context.SeedSampleDataAsync();

            // act - update description price to 8_000
            using var updateScope = context.UseNewScope();
            var productId = TestContext.GetSampleData().First().Id;
            var product = new ProductRequest(productId, "Samsung Galaxy S24 Ultra", "Samsung", 8_000) { Description = "Best smartphone yet!" };
            var result = await context.Scope.Controller.PutAsync(product);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<ProductResponse>>();
            var objectResult = result.As<ActionResult<ProductResponse>>().Result.As<OkObjectResult>();
            objectResult.StatusCode.Should().Be(200);

            objectResult.Value.As<ProductResponse>().Should().NotBeNull();
            objectResult.Value.As<ProductResponse>().Id.Should().NotBeEmpty();
            objectResult.Value.As<ProductResponse>().Name.Should().Be(product.Name);
            objectResult.Value.As<ProductResponse>().Brand.Should().Be(product.Brand);
            objectResult.Value.As<ProductResponse>().Price.Should().Be(product.Price);
            objectResult.Value.As<ProductResponse>().Description.Should().Be(product.Description);
            objectResult.Value.As<ProductResponse>().Color.Should().Be(product.Color);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturn404NotFound_WithNoBodyResponse_WhenTheGivenProductDoesNotExist_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();

            // act
            using var scope = context.UseNewScope();
            var productId = Guid.NewGuid();
            var result = await context.Scope.Controller.DeleteAsync(productId);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<int>>();
            var notFoundObject = result.As<ActionResult<int>>().Result.As<NotFoundResult>();
            notFoundObject.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturn200OK_With_AffectedRowsCount_WhenTheGivenProductExists_Async()
        {
            // arrange
            using var context = TestContext.UseEmptyDb();
            await context.SeedSampleDataAsync();

            // act
            using var scope = context.UseNewScope();
            var productId = TestContext.GetSampleData().First().Id;
            var result = await context.Scope.Controller.DeleteAsync(productId);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<int>>();
            var objectResult = result.As<ActionResult<int>>().Result.As<OkObjectResult>();
            objectResult.StatusCode.Should().Be(200);
            objectResult.StatusCode.Should().Be(200);
            objectResult.Value.Should().NotBeNull();
            objectResult.Value.As<int>().Should().Be(1);
        }
    }
}
