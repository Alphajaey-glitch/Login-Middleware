# LoginMiddleware

A small ASP.NET Core project that implements a **login validation component as custom middleware**. It intercepts login requests in the HTTP pipeline, validates the submitted email and password format with regular expressions, and writes the result directly to the response.

> **Note:** This is a learning project for understanding the middleware pipeline. It validates input *format* only. It does not check credentials against a user store, issue tokens, or create sessions, so it is not a real authentication system.

## Features

- Convention-based custom middleware (`Invoke(HttpContext)` with `RequestDelegate next`)
- Parses `application/x-www-form-urlencoded` request bodies
- Email and password format validation using precompiled regex
- Clear 400 responses for missing, empty, or invalid input
- Non-login requests are passed down the pipeline
- `UseLoginMiddleware()` extension method for clean registration

## Project Structure

```
LoggingUsingMiddleware/
├── CustomMiddleware/
│   └── LoginMiddleware.cs       # Middleware + UseLoginMiddleware extension
├── RegularExpessions/
│   ├── EmailRegex.cs            # Email format validation
│   └── PasswordRegex.cs         # Password format validation
├── Properties/
│   └── launchSettings.json      # Launch profile (http://localhost:5062)
└── Program.cs                   # Pipeline configuration
```

## Getting Started

### Prerequisites

- .NET SDK (the version targeted in the `.csproj`)
- Visual Studio, or any editor with the `dotnet` CLI

### Run

```powershell
dotnet run
```

Or press **F5** in Visual Studio. The `http` launch profile starts the app at:

```
http://localhost:5062
```

## Usage

Register the middleware in `Program.cs` **before** any middleware or endpoint it should protect or precede:

```csharp
app.UseLoginMiddleware();
```

## How It Works

The middleware only acts on requests to the root path `/`. Everything else is passed to the next middleware.

| Request | Behavior |
|---|---|
| `POST /` | Parses the body, validates `email` and `password`, writes a result, and **short-circuits** (does not call `next`) |
| `GET /` | Sets status `200` and passes control to the next middleware |
| Any other request | Passed to the next middleware unchanged |

### POST flow

1. Read the request body and parse it as a form-encoded query string.
2. Extract the `email` and `password` values (missing keys are treated as empty).
3. If `email` is missing or blank, respond `400`.
4. If `password` is missing or blank, respond `400`.
5. If both pass their regex checks, respond with success. Otherwise respond `400`.

### Responses

| Condition | Status | Body |
|---|---|---|
| Email missing or empty | `400` | `Invalid input for email` |
| Password missing or empty | `400` | `Invalid input for password` |
| Email or password fails the format check | `400` | `Invalid login` |
| Both valid | `200` | `Successful login` |

## Validation Rules

### Email

```
^[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(\.[A-Za-z0-9\-]+)*\.[A-Za-z]{2,}$
```

- Local part: letters, digits, and `. _ % + -`
- A single `@`
- Domain with optional subdomains
- A top-level domain of at least 2 letters

Valid: `admin@example.com`, `first.last+tag@mail.example.co.uk`
Invalid: `admin@example`, `@example.com`, `admin@.com`

### Password

```
^(?=.*[A-Za-z])(?=.*\d).{8,64}$
```

- 8 to 64 characters
- At least one letter and one digit
- All other characters (symbols, spaces) are allowed

Valid: `Pass1234`, `my password 2024`
Invalid: `short1`, `onlyletters`, `12345678`

## Testing

The body **must** be sent as `application/x-www-form-urlencoded`. JSON and multipart `form-data` will not be parsed.

### curl

In PowerShell, use `curl.exe` (plain `curl` is an alias for `Invoke-WebRequest`):

```powershell
# Successful login
curl.exe -i -X POST http://localhost:5062/ -d "email=admin%40example.com&password=Pass1234"

# Missing password
curl.exe -i -X POST http://localhost:5062/ -d "email=admin%40example.com"

# Invalid format
curl.exe -i -X POST http://localhost:5062/ -d "email=not-an-email&password=short"
```

### Postman

1. Set the method to `POST` and the URL to `http://localhost:5062/`.
2. In **Body**, choose **x-www-form-urlencoded** (not `form-data`).
3. Add the keys `email` and `password`.

### Special characters

In form-encoded bodies, `+` decodes to a space and `@` is conventionally sent as `%40`. To send `a+b@x.com`, encode the plus as `%2B`:

```
email=a%2Bb%40x.com&password=Pass1234
```

## Configuration

`launchSettings.json`:

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5062",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## Limitations and Possible Improvements

- Validates format only. Add a user store and password hashing for real authentication.
- Credentials are sent over plain HTTP in development. Use HTTPS in any real deployment.
- Regexes have no match timeout. Adding one (for example, 250 ms) protects against ReDoS.
- Only form-encoded bodies are supported. Add JSON support if needed.
- Response bodies are plain text. Consider JSON responses with proper error objects.
- Move login logic into an endpoint or service and keep the middleware thin.

## Concepts Demonstrated

- The middleware pipeline and short-circuiting with and without `next`
- Convention-based middleware and how the framework discovers `Invoke`
- Extension methods for clean pipeline registration
- Parsing request bodies with `QueryHelpers.ParseQuery`
- Compiled static `Regex` instances for input validation