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