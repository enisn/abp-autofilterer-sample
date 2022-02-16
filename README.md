# Using AutoFilterer with ABP

This article is about filtering data automatically without writing any LINQ via using AutoFilterer.

[AutoFilterer](https://github.com/enisn/AutoFilterer) is a mini filtering framework library for dotnet. The main purpose of the library is to generate LINQ expressions for Entities over DTOs automatically. Creating queries without writing any expression code is the most powerful feature that is provided. The first aim of AutoFilterer is to be compatible with Open API 3.0 Specifications, unlike oData & GraphQL.

**Disclaimer:** AutoFilterer is one of my personal projects. It's not supported by ABP Framework or ABP Team officially. This article can be shown as self-promotion, so I needed to explain that.


## Initializing a New Project
If you are familiar to application development with ABP Framework, you can skip to the next step **"Implementing AutoFilterer"**.

- Create a new project:
_(I prefer mongodb as database provider to get rid of Ef migrations. You can go with Ef on your own.)_
```shell
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