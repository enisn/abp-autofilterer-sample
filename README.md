# Using AutoFilterer with ABP

This article is about filtering data automatically without writing any LINQ via using AutoFilterer.

[AutoFilterer](https://github.com/enisn/AutoFilterer) is a mini filtering framework library for dotnet. The main purpose of the library is to generate LINQ expressions for Entities over DTOs automatically. Creating queries without writing any expression code is the most powerful feature that is provided. The first aim of AutoFilterer is to be compatible with Open API 3.0 Specifications, unlike oData & GraphQL.

**Disclaimer:** AutoFilterer is one of my personal projects. It's not supported by ABP Framework or ABP Team officially. This article can be shown as self-promotion, so I needed to explain that.


## Initializing a New Project
If you are familiar to application development with ABP Framework, you can skip to the next step **"Implementing AutoFilterer"**.

- Create a new project:
_(I prefer mongodb as database provider to get rid of Ef migrations. You can go with Ef on your own.)_
```bash
abp new Acme.BookStore -t app -d mongodb
```

- Create an entity named **Book**

```csharp
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
```

- Create a DataSeedContributor
    - Add this [initial-books.json](src/Acme.BookStore.Domain/Books/initial-books.json) file to `Acme.BookStore.Domain/Books/` path and make build action as **Embedded Resource**.
    
```csharp
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Books;

public class BookstoreDataSeederContributor : IDataSeedContributor, ITransientDependency
{
    protected readonly IRepository<Book, Guid> _repository;

    public BookstoreDataSeederContributor(IRepository<Book, Guid> repository)
    {
        _repository = repository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (!await _repository.AnyAsync())
        {
            await _repository.InsertManyAsync(GetInitialBooks());
        }
    }

    private Book[] GetInitialBooks()
    {
        var json = GetEmbeddedResourceAsText("Acme.Bookstore.Books.initial-books.json");

        return JsonConvert.DeserializeObject<Book[]>(json);
    }

    private string GetEmbeddedResourceAsText(string nameWithNamespace)
    {
        using var stream = GetType().Assembly.GetManifestResourceStream(nameWithNamespace);

        return Encoding.UTF8.GetString(stream.GetAllBytes());
    }
}
```

- Run the **DbMigrator** and database with existing data is ready!

---

## Implementing AutoFilterer

- Add `AutoFilterer` package to your **Application.Contracts** project.
```bash
dotnet add package AutoFilterer
```

### Designing the Application.Contracts Layer
In this section, We'll implement AutoFilterer package and use it for only filtering data. We'll leave **Sorting** and **Paging** to ABP Framework, because it already does it well and works with more than one UI compatible.


- Let's start with creating DTOs.
    - BookDto
    ```csharp
    using System;
    using Volo.Abp.Application.Dtos;

    namespace Acme.BookStore.Books;

    [Serializable]
    public class BookDto : AuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Author { get; set; }
        public int TotalPage { get; set; }
        public int Year { get; set; }
        public string Link { get; set; }
    }
    ```

    - BookGetListInput
    ```csharp
    using AutoFilterer.Attributes;
    using AutoFilterer.Enums;
    using AutoFilterer.Types;
    using System;
    using Volo.Abp.Application.Dtos;

    namespace Acme.BookStore.Books;

    [Serializable]
    // We'll leave Paging and Sorting to ABP, we'll use only filtering feature of AutoFilterer.
    // So using FilterBase as a base class is enough.
    public class BookGetListInput : FilterBase, IPagedAndSortedResultRequest
    {
        // Configure 'Filter' property for built-in search boxes.
        [CompareTo(
            nameof(BookDto.Title),
            nameof(BookDto.Language),
            nameof(BookDto.Author),
            nameof(BookDto.Country)
            )]
        [StringFilterOptions(StringFilterOption.Contains)]
        public string Filter { get; set; }

        // IPagedAndSortedResultRequest implementation below.
        public int SkipCount { get; set; }

        public int MaxResultCount { get; set; }

        public string Sorting { get; set; }
    }
    ```

    - IBookAppService
    ```csharp
    using System;
    using Volo.Abp.Application.Services;

    namespace Acme.BookStore.Books;

    public interface IBookAppService : ICrudAppService<BookDto, Guid, BookGetListInput>
    {
    }
    ```