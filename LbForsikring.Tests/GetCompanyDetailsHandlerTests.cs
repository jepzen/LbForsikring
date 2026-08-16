using LbForsikring.Features;
using LbForsikring.Integrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace LbForsikring.Tests;

public class GetCompanyDetailsHandlerTests
{
    private readonly Mock<ICvrService> _mockCvrService;
    private readonly Mock<IDstService> _mockDstService;

    public GetCompanyDetailsHandlerTests()
    {
        _mockCvrService = new Mock<ICvrService>();
        _mockDstService = new Mock<IDstService>();
    }

    [Fact]
    public async Task Handle_WithNeitherNameNorCvr_ReturnsBadRequest()
    {
        // Arrange
        var request = new GetCompanyDetails.GetCompanyDetailsRequest { Name = null, Cvr = null };

        // Act
        var result = await GetCompanyDetails.Handle(request, _mockCvrService.Object, _mockDstService.Object);

        // Assert
        Assert.IsType<ProblemHttpResult>(result.Result);
    }

    [Fact]
    public async Task Handle_WithEmptyNameAndEmptyCvr_ReturnsBadRequest()
    {
        // Arrange
        var request = new GetCompanyDetails.GetCompanyDetailsRequest { Name = "", Cvr = "" };

        // Act
        var result = await GetCompanyDetails.Handle(request, _mockCvrService.Object, _mockDstService.Object);

        // Assert
        Assert.IsType<ProblemHttpResult>(result.Result);
    }

    [Fact]
    public async Task Handle_WithNullNameAndWhitespaceCvr_DoesNotReturnBadRequest()
    {
        // Arrange - whitespace-only strings don't trigger validation (string.IsNullOrEmpty returns false for "   ")
        var request = new GetCompanyDetails.GetCompanyDetailsRequest { Name = null, Cvr = "   " };
        var cvrResponseJson = "{}";
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByCvr("   "))
            .ReturnsAsync(cvrResponseJson);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act - this will fail at CreateResponse, but validation should pass
        var exception = await Assert.ThrowsAsync<NotImplementedException>(async () =>
            await GetCompanyDetails.Handle(request, _mockCvrService.Object, _mockDstService.Object));

        // Assert - CreateResponse throws, not validation
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task Handle_WithValidCvrProvided_DoesNotCallGetByName()
    {
        // Arrange
        var request = new GetCompanyDetails.GetCompanyDetailsRequest { Cvr = "12345678", Name = "IgnoredName" };
        var cvrResponseJson = "{}";
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByCvr("12345678"))
            .ReturnsAsync(cvrResponseJson);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Handle(request, _mockCvrService.Object, _mockDstService.Object);
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
        var request = new GetCompanyDetails.GetCompanyDetailsRequest { Name = "TestCompany", Cvr = null };
        var cvrResponseJson = "{}";
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByName("TestCompany"))
            .ReturnsAsync(cvrResponseJson);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Handle(request, _mockCvrService.Object, _mockDstService.Object);
        }
        catch (NotImplementedException)
        {
            // CreateResponse is not implemented, but we can still verify the calls
        }

        // Assert
        _mockCvrService.Verify(x => x.GetByName("TestCompany"), Times.Once);
        _mockCvrService.Verify(x => x.GetByCvr(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCvrServiceCalled_PassesCorrectCvrValue()
    {
        // Arrange
        var cvr = "87654321";
        var request = new GetCompanyDetails.GetCompanyDetailsRequest { Cvr = cvr };
        var cvrResponseJson = "{}";
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByCvr(cvr))
            .ReturnsAsync(cvrResponseJson);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Handle(request, _mockCvrService.Object, _mockDstService.Object);
        }
        catch (NotImplementedException)
        {
            // CreateResponse is not implemented
        }

        // Assert
        _mockCvrService.Verify(x => x.GetByCvr(cvr), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNameServiceCalled_PassesCorrectNameValue()
    {
        // Arrange
        var name = "TestCompanyName";
        var request = new GetCompanyDetails.GetCompanyDetailsRequest { Name = name };
        var cvrResponseJson = "{}";
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByName(name))
            .ReturnsAsync(cvrResponseJson);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Handle(request, _mockCvrService.Object, _mockDstService.Object);
        }
        catch (NotImplementedException)
        {
            // CreateResponse is not implemented
        }

        // Assert
        _mockCvrService.Verify(x => x.GetByName(name), Times.Once);
    }

    [Fact]
    public async Task Handle_CallsDstServiceWithCurrentYear()
    {
        // Arrange
        var request = new GetCompanyDetails.GetCompanyDetailsRequest { Cvr = "12345678" };
        var currentYear = DateTime.Now.Year;
        var cvrResponseJson = "{}";
        var dstResponseJson = "{}";

        _mockCvrService.Setup(x => x.GetByCvr("12345678"))
            .ReturnsAsync(cvrResponseJson);
        _mockDstService.Setup(x => x.GetBrancheData(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(dstResponseJson);

        // Act
        try
        {
            await GetCompanyDetails.Handle(request, _mockCvrService.Object, _mockDstService.Object);
        }
        catch (NotImplementedException)
        {
            // CreateResponse is not implemented
        }

        // Assert
        _mockDstService.Verify(x => x.GetBrancheData(It.IsAny<string>(), currentYear), Times.Once);
    }
}

