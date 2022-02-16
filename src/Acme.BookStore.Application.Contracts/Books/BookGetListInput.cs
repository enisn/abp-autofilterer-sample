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