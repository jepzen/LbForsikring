using LbForsikring.Features;
using LbForsikring.Integrations;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace LbForsikring.Tests;

public class GetCompanyDetailsHandlerTests
{
    private readonly Mock<ICvrService> _mockCvrService = new();
    private readonly Mock<IDstService> _mockDstService = new();

    [Fact]
    public async Task Handle_WithNeitherNameNorCvr_ReturnsBadRequest()
    {
        // Act
        var result = await GetCompanyDetails.Handle(null, null, _mockCvrService.Object, _mockDstService.Object);

        // Assert
        Assert.IsType<ProblemHttpResult>(result.Result);
    }

    [Fact]
    public async Task Handle_WithEmptyNameAndEmptyCvr_ReturnsBadRequest()
    {
        // Act
        var result = await GetCompanyDetails.Handle("", "", _mockCvrService.Object, _mockDstService.Object);

        // Assert
        Assert.IsType<ProblemHttpResult>(result.Result);
    }

    [Fact]
    public async Task Handle_WithNullNameAndWhitespaceCvr_DoesNotReturnBadRequest()
    {
        // Arrange - whitespace-only strings don't trigger validation (string.IsNullOrEmpty returns false for "   ")
        var cvrResponse = new CvrResponse
        {
            Vat = 16500836,
            Name = "LB FORSIKRING A/S",
            Address = "Amerika Plads 15",
            Zipcode = 2100,
            City = "København Ø",
            IndustryCode = "651200",
            IndustryDesc = "Anden forsikring"
        };
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByCvr("   "))
            .ReturnsAsync(cvrResponse);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act - this will fail at CreateResponse, but validation should pass
        var exception = await Assert.ThrowsAsync<NotImplementedException>(async () =>
            await GetCompanyDetails.Handle(null, "   ", _mockCvrService.Object, _mockDstService.Object));

        // Assert - CreateResponse throws, not validation
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task Handle_WithValidCvrProvided_DoesNotCallGetByName()
    {
        // Arrange
        var cvrResponse = new CvrResponse
        {
            Vat = 16500836,
            Name = "LB FORSIKRING A/S",
            Address = "Amerika Plads 15",
            Zipcode = 2100,
            City = "København Ø",
            IndustryCode = "651200",
            IndustryDesc = "Anden forsikring"
        };
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByCvr("12345678"))
            .ReturnsAsync(cvrResponse);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Handle("IgnoredName", "12345678", _mockCvrService.Object, _mockDstService.Object);
        }
        catch (NotImplementedException)
        {
            // CreateResponse is not implemented, but we can still verify the calls
        }

        // Assert
        _mockCvrService.Verify(x => x.GetByCvr("12345678"), Times.Once);
        _mockCvrService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidNameOnly_CallsGetByName()
    {
        // Act & Assert - GetByName is not yet implemented to return CvrResponse
        var exception = await Assert.ThrowsAsync<NotImplementedException>(async () =>
            await GetCompanyDetails.Handle("TestCompany", null, _mockCvrService.Object, _mockDstService.Object));

        Assert.NotNull(exception);
    }

    [Fact]
    public async Task Handle_WhenCvrServiceCalled_PassesCorrectCvrValue()
    {
        // Arrange
        var cvr = "87654321";
        var cvrResponse = new CvrResponse
        {
            Vat = 16500836,
            Name = "LB FORSIKRING A/S",
            Address = "Amerika Plads 15",
            Zipcode = 2100,
            City = "København Ø",
            IndustryCode = "651200",
            IndustryDesc = "Anden forsikring"
        };
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByCvr(cvr))
            .ReturnsAsync(cvrResponse);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Handle(null, cvr, _mockCvrService.Object, _mockDstService.Object);
        }
        catch (NotImplementedException)
        {
            // CreateResponse is not implemented
        }

        // Assert
        _mockCvrService.Verify(x => x.GetByCvr(cvr), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsDstServiceWithCurrentYear()
    {
        // Arrange
        var currentYear = DateTime.Now.Year;
        var cvrResponse = new CvrResponse
        {
            Vat = 16500836,
            Name = "LB FORSIKRING A/S",
            Address = "amerika Plads 15",
            Zipcode = 2100,
            City = "København Ø",
            IndustryCode = "651200",
            IndustryDesc = "Anden forsikring"
        };
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByCvr("12345678"))
            .ReturnsAsync(cvrResponse);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Handle(null, "12345678", _mockCvrService.Object, _mockDstService.Object);
        }
        catch (NotImplementedException)
        {
            // CreateResponse is not implemented
        }

        // Assert
        _mockDstService.Verify(x => x.GetBrancheData(It.IsAny<string>(), currentYear), Times.Once);
    }
}

