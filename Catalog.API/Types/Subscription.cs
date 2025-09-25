using Catalog.API.Models;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace Catalog.API.Types;

[SubscriptionType]
public class Subscription
{
    // Book Subscriptions
    [GraphQLDescription("Subscription for when a book is added")]
    [Subscribe]
    public Book BookAdded([EventMessage] Book book) => book;

    [GraphQLDescription("Subscription for when a book is updated")]
    [Subscribe]
    public Book BookUpdated([EventMessage] Book book) => book;

    [GraphQLDescription("Subscription for when a book is deleted")]
    [Subscribe]
    public  Guid BookDeleted([EventMessage] Guid bookId) => bookId;

    // Author Subscriptions
    [GraphQLDescription("Subscription for when an author is added")]
    [Subscribe]
    public Author AuthorAdded([EventMessage] Author author) => author;

    [GraphQLDescription("Subscription for when an author is updated")]
    [Subscribe]
    public Author AuthorUpdated([EventMessage] Author author) => author;

    [GraphQLDescription("Subscription for when an author is deleted")]
    [Subscribe]
    public Guid AuthorDeleted([EventMessage] Guid authorId) => authorId;
}