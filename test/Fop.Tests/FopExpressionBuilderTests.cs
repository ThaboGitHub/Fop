using System.Linq;
using Fop.Exceptions;
using Fop.Filter;
using Fop.FopExpression;
using Fop.Order;
using Sample.Entity;
using Xunit;

namespace Fop.Tests;

public class FopExpressionBuilderTests
{
    private readonly IQueryable<Student> _students;

    public FopExpressionBuilderTests()
    {
        _students = DataInitializer.GenerateStudentList();
    }

    [Fact]
    public void FopExpressionBuilder_Should_Build_FopRequest_With_Single_FilterList()
    {
        // Arrange
        var filterQueryString = "Midterm>10;Name_=A;and";
        var orderQueryString = "Midterm;desc";
        var pageNumber = 1;
        var pageSize = 50;

        // Act
        var result = FopExpressionBuilder<Student>.Build(filterQueryString, orderQueryString, pageNumber, pageSize);

        // Assert
        Assert.True(result.PageNumber == 1);
        Assert.True(result.PageSize == 50);
        Assert.True(result.OrderList.FirstOrDefault()?.OrderBy == "midterm");
        Assert.True(result.OrderList.FirstOrDefault()?.Direction == OrderDirection.Desc);
        Assert.True(result.FilterList.FirstOrDefault()?.Logic == FilterLogic.And);
        Assert.True(result.FilterList.FirstOrDefault()?.Filters.Any(x => x.Key == "Student.Name"));
    }


    [Fact]
    public void FopExpressionBuilder_Should_Build_FopRequest_With_Multiple_FilterList()
    {
        // Arrange
        var filterQueryString = "Midterm>10;Name_=A$IdentityNumber==100101;IdentityNumber==100001;or";
        var orderQueryString = "Midterm;desc";
        var pageNumber = 1;
        var pageSize = 50;

        // Act
        var result = FopExpressionBuilder<Student>.Build(filterQueryString, orderQueryString, pageNumber, pageSize);

        // Assert
        Assert.True(result.PageNumber == 1);
        Assert.True(result.PageSize == 50);
        Assert.True(result.OrderList.FirstOrDefault()?.OrderBy == "midterm");
        Assert.True(result.OrderList.FirstOrDefault()?.Direction == OrderDirection.Desc);
        Assert.True(result.FilterList.FirstOrDefault()?.Logic == FilterLogic.And);
        Assert.True(result.FilterList.FirstOrDefault()?.Filters.Any(x => x.Key == "Student.Name"));
        Assert.True(result.FilterList.LastOrDefault()?.Filters.Any(x => x.Key == "Student.IdentityNumber"));
    }

    [Fact]
    public void FopExpressionBuilder_Should_Build_FopRequest_With_Multiple_FilterList_With_DateTime()
    {
        // Arrange
        var filterQueryString = "Level==a;Birthday>1993-06-07 00:00:00;and";
        var orderQueryString = "Midterm;desc";
        var pageNumber = 1;
        var pageSize = 50;

        // Act
        var result = FopExpressionBuilder<Student>.Build(filterQueryString, orderQueryString, pageNumber, pageSize);

        // Assert
        Assert.True(result.PageNumber == 1);
        Assert.True(result.PageSize == 50);
        Assert.True(result.OrderList.FirstOrDefault()?.OrderBy == "midterm");
        Assert.True(result.OrderList.FirstOrDefault()?.Direction == OrderDirection.Desc);
        Assert.True(result.FilterList.FirstOrDefault()?.Logic == FilterLogic.And);
        Assert.True(result.FilterList.FirstOrDefault()?.Filters.Any(x => x.DataType == FilterDataTypes.Char));
        Assert.True(result.FilterList.FirstOrDefault()?.Filters.Any(x => x.Key == "Student.Birthday"));
    }

    [Fact]
    public void FopExpressionBuilder_Should_Failed_Returns_FilterOperatorNotFoundException()
    {
        // Arrange
        var filterQueryString = "Midterm>10;Name__A;and$IdentityNumber==100101;IdentityNumber==100001;or";
        var orderQueryString = "Midterm;desc";
        var pageNumber = 1;
        var pageSize = 50;

        // Act
        var ex = Record.Exception(() => FopExpressionBuilder<Student>.Build(filterQueryString, orderQueryString, pageNumber, pageSize));

        // Assert
        Assert.True(ex is FilterOperatorNotFoundException);
    }

    [Fact]
    public void FopExpressionBuilder_Should_Build_FopRequest_With_Multiple_OrderSameDirection()
    {
        // Arrange
        var filterQueryString = "Midterm>10;Name_=A;and";
        var orderQueryString = "Midterm,IdentityNumber;desc";
        var pageNumber = 1;
        var pageSize = 50;

        // Act
        var result = FopExpressionBuilder<Student>.Build(filterQueryString, orderQueryString, pageNumber, pageSize);

        // Assert
        Assert.True(result.PageNumber == 1);
        Assert.True(result.PageSize == 50);
        Assert.True(result.OrderList.Count() == 2);
        Assert.True(result.OrderList?.ElementAt(0).OrderBy == "midterm");
        Assert.True(result.OrderList?.ElementAt(0).Direction == OrderDirection.Desc);
        Assert.True(result.OrderList?.ElementAt(1).OrderBy == "identitynumber");
        Assert.True(result.OrderList?.ElementAt(1).Direction == OrderDirection.Desc);
        Assert.True(result.FilterList.FirstOrDefault()?.Logic == FilterLogic.And);
        Assert.True(result.FilterList.FirstOrDefault()?.Filters.Any(x => x.Key == "Student.Name"));
    }

    [Fact]
    public void FopExpressionBuilder_Should_Build_FopRequest_With_Multiple_OrderDifferentDirection()
    {
        // Arrange
        var filterQueryString = "Midterm>10;Name_=A;and";
        var orderQueryString = "Midterm;desc$IdentityNumber;asc";
        var pageNumber = 1;
        var pageSize = 50;

        // Act
        var result = FopExpressionBuilder<Student>.Build(filterQueryString, orderQueryString, pageNumber, pageSize);

        // Assert
        Assert.True(result.PageNumber == 1);
        Assert.True(result.PageSize == 50);
        Assert.True(result.OrderList.Count() == 2);
        Assert.True(result.OrderList?.ElementAt(0).OrderBy == "midterm");
        Assert.True(result.OrderList?.ElementAt(0).Direction == OrderDirection.Desc);
        Assert.True(result.OrderList?.ElementAt(1).OrderBy == "identitynumber");
        Assert.True(result.OrderList?.ElementAt(1).Direction == OrderDirection.Asc);
        Assert.True(result.FilterList.FirstOrDefault()?.Logic == FilterLogic.And);
        Assert.True(result.FilterList.FirstOrDefault()?.Filters.Any(x => x.Key == "Student.Name"));
    }

    [Fact]
    public void FopExpressionBuilder_Should_Build_FopRequest_With_Multiple_Order_With_DifferentDirectionsAndFields()
    {
        // Arrange
        var filterQueryString = "Midterm>10;Name_=A;and";
        var orderQueryString = "Midterm,IdentityNumber;desc$IdentityNumber,Midterm;asc";
        var pageNumber = 1;
        var pageSize = 50;

        // Act
        var result = FopExpressionBuilder<Student>.Build(filterQueryString, orderQueryString, pageNumber, pageSize);

        // Assert
        Assert.True(result.PageNumber == 1);
        Assert.True(result.PageSize == 50);
        Assert.True(result.OrderList.Count() == 4);
        Assert.True(result.OrderList?.ElementAt(0).OrderBy == "midterm");
        Assert.True(result.OrderList?.ElementAt(0).Direction == OrderDirection.Desc);
        Assert.True(result.OrderList?.ElementAt(1).OrderBy == "identitynumber");
        Assert.True(result.OrderList?.ElementAt(1).Direction == OrderDirection.Desc);
        Assert.True(result.OrderList?.ElementAt(2).OrderBy == "identitynumber");
        Assert.True(result.OrderList?.ElementAt(3).Direction == OrderDirection.Asc);
        Assert.True(result.OrderList?.ElementAt(3).OrderBy == "midterm");
        Assert.True(result.OrderList?.ElementAt(3).Direction == OrderDirection.Asc);
        Assert.True(result.FilterList.FirstOrDefault()?.Logic == FilterLogic.And);
        Assert.True(result.FilterList.FirstOrDefault()?.Filters.Any(x => x.Key == "Student.Name"));
    }
}