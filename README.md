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

- 