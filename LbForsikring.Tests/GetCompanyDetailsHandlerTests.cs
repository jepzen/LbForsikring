using LbForsikring.Features;
using LbForsikring.Integrations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace LbForsikring.Tests;

public class GetCompanyDetailsHandlerTests
{
    private readonly Mock<ICvrService> _mockCvrService = new();
    private readonly Mock<IDstService> _mockDstService = new();
    private readonly Mock<ILogger<GetCompanyDetails>> _logger = new();

    [Fact]
    public async Task Handle_WithNeitherNameNorCvr_ReturnsBadRequest()
    {
        // Act
        var result = await GetCompanyDetails.Endpoint.Handle(null, null, _mockCvrService.Object, _mockDstService.Object, _logger.Object);

        // Assert
        Assert.IsType<ProblemHttpResult>(result.Result);
    }

    [Fact]
    public async Task Handle_WithEmptyNameAndEmptyCvr_ReturnsBadRequest()
    {
        // Act
        var result = await GetCompanyDetails.Endpoint.Handle("", "", _mockCvrService.Object, _mockDstService.Object, _logger.Object);

        // Assert
        Assert.IsType<ProblemHttpResult>(result.Result);
    }

    [Fact]
    public async Task Handle_WithNullNameAndWhitespaceCvr_CallsCvrService()
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
        _mockDstService.Setup(x => x.GetIndustryStats(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Endpoint.Handle(null, "   ", _mockCvrService.Object, _mockDstService.Object, _logger.Object);
        }
        catch
        {
            // Expected - implementation not complete
        }

        // Assert
        _mockCvrService.Verify(x => x.GetByCvr("   "), Times.Once);
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
        _mockDstService.Setup(x => x.GetIndustryStats(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Endpoint.Handle("IgnoredName", "12345678", _mockCvrService.Object, _mockDstService.Object, _logger.Object);
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
        // Arrange
        var cvrResponse = new CvrResponse
        {
            Vat = 16500836,
            Name = "TestCompany",
            Address = "Test Address",
            Zipcode = 1000,
            City = "Test City",
            IndustryCode = "999999",
            IndustryDesc = "Test Industry"
        };
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByName("TestCompany"))
            .ReturnsAsync(cvrResponse);
        _mockDstService.Setup(x => x.GetIndustryStats(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Endpoint.Handle("TestCompany", null, _mockCvrService.Object, _mockDstService.Object, _logger.Object);
        }
        catch
        {
            // Expected - implementation not complete
        }

        // Assert
        _mockCvrService.Verify(x => x.GetByName("TestCompany"), Times.Once);
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
        _mockDstService.Setup(x => x.GetIndustryStats(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Endpoint.Handle(null, cvr, _mockCvrService.Object, _mockDstService.Object, _logger.Object);
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
        _mockDstService.Setup(x => x.GetIndustryStats(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Endpoint.Handle(null, "12345678", _mockCvrService.Object, _mockDstService.Object, _logger.Object);
        }
        catch (NotImplementedException)
        {
            // CreateResponse is not implemented
        }

        // Assert
        _mockDstService.Verify(x => x.GetIndustryStats(It.IsAny<string>(), 2024), Times.Once);
    }
    
    [Fact]
    public async Task Handle_WhenCvrServiceFails_PropagatesException()
    {
        // Arrange
        _mockCvrService
            .Setup(x => x.GetByCvr("12345678"))
            .ThrowsAsync(new InvalidOperationException("CVR service unavailable"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            GetCompanyDetails.Endpoint.Handle(
                null,
                "12345678",
                _mockCvrService.Object,
                _mockDstService.Object,
                _logger.Object));

        Assert.Equal("CVR service unavailable", exception.Message);
    }
    
    [Fact]
    public async Task Handle_WhenDstServiceFails_PropagatesException()
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

        _mockCvrService
            .Setup(x => x.GetByCvr("12345678"))
            .ReturnsAsync(cvrResponse);

        _mockDstService
            .Setup(x => x.GetIndustryStats("651200", 2024))
            .ThrowsAsync(new HttpRequestException("StatBank unavailable"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
            GetCompanyDetails.Endpoint.Handle(
                null,
                "12345678",
                _mockCvrService.Object,
                _mockDstService.Object,
                _logger.Object));

        Assert.Equal("StatBank unavailable", exception.Message);
    }
}

