# Payment Gateway .NET challenge

## Running and Testing instructions

1. Make sure you are using .NET 9.0 (very important, the solution was upgraded!)
2. Make sure you have docker installed
3. Run `docker compose up` to start the Bank simulator (without this an integration test will fail!)
4. Run `dotnet build` - build should pass without warnings.
5. Run `dotnet test`

## Project structure and Architecture

### `BuildingBlocks` class library

This is the top level library. It's purpose is to hold any code that is not related to Payment Gateway and can freely be shared with other projects (no business or domain specific logic should be here). This project also includes common NuGet packages that are used by all other projects.

The packages used are the following:

- `LanguageExt.Core` - This library extends built-in Linq library with many functional programming style helper classes. For example, in the project we're heavily relaying on `Either<T,T>` and `Validation<T,T>` [monads](<https://en.wikipedia.org/wiki/Monad_(functional_programming)>) to avoid passing Nulls around.
- `Mapster` - AutoMapper-like library that is used to map DTOs to Domian Objects without writing much code.
- `Serilog` - Support for Serialized logging
- `Serlog.Sinks.File` - For this demo only file sink is used, but the project can easily be extended to include more powerful logging backends.
- `Microsoft.Extensions.Logging` - allows us to use dependency injection and pass generic logger to all our classes.

In addition to the packages this library also contains `SystemDateTime` class. This class is used to allow easy unit testing of dates. It is expected that our code would use `SystemDateTime.Now` instead of `DateTime.UtcNow` when checking the current time.

### `PaymentGateway.Abstraction` class library

All commonly used data types, models and enums are moved to this project to allow them to be easily accessed by all projects and tests that require the class definitions.
For this demo purpose, one project would suffice but in a real-life example this would be multiple projects.

Most of the DTOs are changed to be Records instead of classes simplify comparisons and make properties immuntable.

### `PaymentGateway.Services` class library

This is the "backend" part of the system. For the demo I used one project - in real-life example this would be a folder with multiple microservices that can be run and scaled separately.

For this demo each "service" is just one file.

#### `PaymentProcessorService`

This is the main back-end service. It's responsibilities are:

- Validate the input details
- Call the bank to process the payment (in this case `SimlatorBank` will be resolved as a dependency)
- Save results into the `PaymentsRepository`

The service returns an `Either<PaymentProcessorResponse, string>` where the string is a list of validation errors.

#### `CardValidator`

This service contains all the validation rules that are attached to a card.

Credit Card Number is being passed in as a `long`. In order to check the card number there are rules to check the number is positive, it is of a required length, and an additional Luhn check was added to check if the card is in the valid. The `long` type supports up to 19 chars, but the card could not start with `93...` or that would cause an overflow. Luckly, there are no valid credit cards that start with number 9 so this should not be an issue.

#### `SimultorBank`

`SimulatorBank` is an implementation of `IBank` - that is why the name is not `BankSimultor` or something more readable. Idea is that different bank integrations could be supported and we could have something like `HsbcBank` etc. to work with real banks.

As for this demo simulator bank just prepares the request and calls the provided bank simulator.

#### `PaymentsRepository`

Is mostly unchanged as provided by the provided code. Only change made is that `GetAsync` returns `Option<PostPaymentResponse>` instead of just response. The reason for this is to avoid returning `null` when the result is not found.

#### `CurrencyProvider`

Very simple service that returns a few supported currencies. The idea of this service would be to allow custom business logic that may be linked to some currencies.

#### `ApiKeysRepository`

The idea of this service is to allow Merchants to authenticate themselves. This functionality is largely incomplete and I will covered in the WebAPI section.

### `PaymentGateway.Api` WebAPI project

This is the application entry point. Most of the original code was removed from this project, in particular the models and enums are moved into the Abstraction project.

The project has main parts.

#### `Program.cs`

Here we configure the web application, configure dependency injection and add middlewares.

Mapster configuration is also setup here, mostly to handle the credit card expiry date being represented as 2 ints in most of the application, while simulator bank expects a string in `MM/YYYY` format.

Authentication middleware is added here and will trigger on every request.

Logs are only sent to a text file inside of a Logs folder (that is part of `.gitignore`).

Swagger is also included and SwaggerUI can be access in dev environment.

#### `ApiKeyValidationMiddleware`

Authentication was not part of the requirements, but it felt wrong not to include any. As an example in this demo, a small API-key based authentication is added.

It does not provide any real security but it can be used to identify the merchant.

#### `PaymentsController`

Actual controller handling the `GET` and the `POST` methods. There's no logic really inside the controllers, they only call the backend services do a simple `Match` on the service result and convert it into a valid HTTP response and status code.

## Recommended Improvements

#### Introduce CQRS and have separate read and write databases

There should be better separation of reads and writes.
There was a requirement to keep the `PaymentsRepository` as only data store and keep it as is. So the separation was not done.

Ideally, the GET request would execute a `GetPaymentQuery` and would target a potentially relational database that would keep the current state of the payment with related meta data (such as owning merchant etc.)

The POST request would call `CreatePaymentCommand` that would probably write into an append-only event log that would keep all the changes on an object. This would allow us to store the object into our DB as draft and then add another record when the response from the bank is received.
Depending on the scale of the system this could either be a PostgresDb (using Marten library) or some other event store database.

#### Add `MediatR` library

For two reasons:

1. Better support for above mentioned CQRS
2. Improve separation of concerns of the Payment Processor Service by removing validation logic from the service (handler) and move it to dedicated validator.

### Authentication Improvements suggestions

First of all, every merchant should have an `API key` (the way it's currently implemented), and this would be used to identify every incoming request and assign it to a specific merchant. It would be very important to add a check into the `GET Payment` endpoint (and payments repository) and see if the requested payment belongs to the same merchant who is doing the request.

Every merchant would also have a `merchant secret`. The secret would not be part of headers in any request, it will only be part of the hash. Both client and server know the secret but it is not passed via http to avoid any interception.

`nonce` could also be used. This would be a random number that a client would pass in the header. This number would then be used to ensure no duplicate requests are being processed (idempotency check) as well as making the hash unique with every request.

`timestamp` could also be used to ensure no stale requests are processed (we could add a logic not to allow any request that is older than 5 minutes for example); as well making the hash more complex and unique with every request.

`hash` would provide actual security of the requests. For example hash could be built with a logic l like this:

```csharp
var data = $"{httpMethod}{url}{nonce}{timestamp}{merchantSecret}{body}";
using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiKey));
var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
```

Note that headers would include `ApiKey`, `noonce` and `timestamp`. The secret would only be known to the merchant and backend service.

### Testing Improvements

There's currently a mix of unit and integration tests bundled together.

Integration tests should be extracted into a separate projects and configured better to initialize the simulator automatically and tear it down when used.

### Deployment and Containerization

Docker compose file should be extended to allow deploying the application into several containers (at least separate web and services)
