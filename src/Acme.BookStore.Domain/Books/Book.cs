using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.BookStore.Books;

public class Book : FullAuditedAggregateRoot<Guid>
{
    public string Title { get; set; }
    public string Language { get; set; }
    public string Country { get; set; }
    public string Author { get; set; }
    public int TotalPage { get; set; }
    public int Year { get; set; }
    public string Link { get; set; }
}