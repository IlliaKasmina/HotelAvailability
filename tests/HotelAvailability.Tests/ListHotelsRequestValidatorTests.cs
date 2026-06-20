using FluentValidation.TestHelper;
using HotelAvailability.Api.Features.Hotels.List;

namespace HotelAvailability.Tests;

public sealed class ListHotelsRequestValidatorTests
{
    private static ListHotelsRequestValidator CreateValidator() => new();

    [Fact]
    public void Omitted_paging_is_valid()
    {
        var result = CreateValidator().TestValidate(new ListHotelsRequest());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Valid_page_passes(int page)
    {
        var result = CreateValidator().TestValidate(new ListHotelsRequest { Page = page });
        result.ShouldNotHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Page_below_one_fails(int page)
    {
        var result = CreateValidator().TestValidate(new ListHotelsRequest { Page = page });
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Valid_page_size_passes(int pageSize)
    {
        var result = CreateValidator().TestValidate(new ListHotelsRequest { PageSize = pageSize });
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Page_size_out_of_range_fails(int pageSize)
    {
        var result = CreateValidator().TestValidate(new ListHotelsRequest { PageSize = pageSize });
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
