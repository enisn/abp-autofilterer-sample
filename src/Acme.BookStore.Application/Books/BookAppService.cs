using AutoFilterer.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Books;

public class BookAppService : CrudAppService<Book, BookDto, Guid, BookGetListInput>
{
    public BookAppService(IRepository<Book, Guid> repository) : base(repository)
    {
    }

    protected override async Task<IQueryable<Book>> CreateFilteredQueryAsync(BookGetListInput input)
    {
        return (await base.CreateFilteredQueryAsync(input))
            .ApplyFilter(input);
    }
}