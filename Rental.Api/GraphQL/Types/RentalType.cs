namespace Rental.Api.GraphQL.Types;

public class RentalType : ObjectType<Models.Rental>
{
    protected override void Configure(IObjectTypeDescriptor<Models.Rental> descriptor)
    {
        descriptor
            .Field(r => r.Id)
            .Type<NonNullType<StringType>>()
            .Description("The unique identifier of the rental");

        descriptor
            .Field(r => r.CustomerId)
            .Type<NonNullType<StringType>>()
            .Description("The ID of the customer who rented the book");

        descriptor
            .Field(r => r.CustomerName)
            .Type<NonNullType<StringType>>()
            .Description("The name of the customer who rented the book");

        descriptor
            .Field(r => r.Book)
            .Type<NonNullType<BookReferenceType>>()
            .Description("Information about the rented book");

        descriptor
            .Field(r => r.RentalDate)
            .Type<NonNullType<DateTimeType>>()
            .Description("The date when the book was rented");

        descriptor
            .Field(r => r.DueDate)
            .Type<NonNullType<DateTimeType>>()
            .Description("The date when the book is due to be returned");

        descriptor
            .Field(r => r.ReturnDate)
            .Type<DateTimeType>()
            .Description("The date when the book was returned, if applicable");

        descriptor
            .Field(r => r.Status)
            .Type<NonNullType<StringType>>()
            .Description("The current status of the rental (Active, Returned, Overdue)");

        descriptor
            .Field(r => r.LateFee)
            .Type<NonNullType<DecimalType>>()
            .Description("The late fee charged, if applicable");

        descriptor
            .Field(r => r.Notes)
            .Type<StringType>()
            .Description("Additional notes about the rental");
    }
}

public class BookReferenceType : ObjectType<Models.BookReference>
{
    protected override void Configure(IObjectTypeDescriptor<Models.BookReference> descriptor)
    {
        descriptor
            .Field(b => b.BookId)
            .Type<NonNullType<UuidType>>()
            .Description("The ID of the book in the catalog");

        descriptor
            .Field(b => b.Title)
            .Type<NonNullType<StringType>>()
            .Description("The title of the book");

        descriptor
            .Field(b => b.ISBN)
            .Type<NonNullType<StringType>>()
            .Description("The ISBN of the book");
    }
}