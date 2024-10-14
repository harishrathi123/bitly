## Architecture
- user Clean Architecture / Vertical Slice Architecture
- Carter/MediatR separate dependencies between Api Layer vs Business Logic
- Endpoint contains all the logic right from url, to business code to persistance
- - easy to test / mock the flow


# Considerations
- Code generated is using random characters for now
- can pre-generate/cache codes for better performance


# Libraries
- Minimal API: goes better with REPR pattern / VSA architecture
- IDistributedCache: used in-memory cache for now, but can easily wrap around RedisCache
- EF Core - for database persistance
- Carter - to scan & register all minimal-api endpoints
- MediatR - to execute client requests/commands
- FluentValidation - to fluent validation request DTO
- FluentAssetion - better testing library
- FakeItEasy - to create fake object/responses
- Serilog - for logging
- Swagger - to test the endpoints


## Open Items
- Can use repository to easily mock database dependency
- - not done here, as persistance was not a requirement
- FastEndpoints - provides all the goodies, but lot more stuff with it


## Running the code
- to run
dotnet watch run --project ./Bitly/Bitly.csproj
- to test
dotnet test ./Bitly.sln

