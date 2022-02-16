$(function () {
    var l = abp.localization.getResource('BookStore');

    var getFilter = function () {
        return {
            totalPage: {
                min: $('#TotalPageMin').val(),
                max: $('#TotalPageMax').val()
            },
            year: {
                min: $('#YearMin').val(),
                max: $('#YearMax').val()
            }
        };
    };

    $("#books-filter-wrapper :input").on('input', function () {
        dataTable.ajax.reload();
    });

    var dataTable = $('#BooksTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: true,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(acme.bookStore.books.book.getList, getFilter),
            columnDefs: [
                {
                    title: l('Title'),
                    data: "title"
                },
                {
                    title: l('Language'),
                    data: "language",
                },
                {
                    title: l('Country'),
                    data: "country",
                },
                {
                    title: l('Author'),
                    data: "author"
                },
                {
                    title: l('TotalPage'),
                    data: "totalPage",
                    render: function (data) {
                        return data + ' pages'
                    }
                },
                {
                    title: l('Year'),
                    data: "year"
                },
                {
                    title: l('Link'),
                    data: "link",
                    render: function (data) {
                        return '<a href="' + data + '" target="_blank">Link</a>';
                    }
                },
            ]
        })
    );
});
