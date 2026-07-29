# Solution Documentation

**Candidate Name:** Rohit Gadewad  
**Completion Date:** 2026-07-29

---

## Problems Identified

- The original implementation used POST-only endpoints for all actions, which is not RESTful.
- `TodoController` created `TodoService` directly instead of using dependency injection.
- SQL queries were built with string interpolation, introducing SQL injection risk.
- There was no request validation and tests were not isolated or meaningful.

---

## Architectural Decisions

- Added `ITodoService` and registered `TodoService` as a singleton service using dependency injection.
- Kept the controller thin and moved database persistence into the service layer.
- Used request DTOs (`TodoRequest`, `TodoUpdateRequest`) for input validation.
- Implemented parameterized SQLite commands to prevent injection and added database initialization.

---

## Trade-offs

- Used synchronous database access to align with the existing codebase and minimize refactor scope.
- Chose a minimal service layer instead of introducing a full repository or ORM.
- Focused on API correctness, test isolation, and safer SQL rather than adding additional features.

---

## How to Run

### Prerequisites

- .NET 10 SDK
- SQLite support on the local machine

### Build

```bash
cd "dotnet-interview"
dotnet build
```

### Run

```bash
dotnet run --project TodoApi
```

### Test

```bash
dotnet test
```

---

## API Documentation

### Create TODO

```
Method: POST
URL: /api/todos
Request Body:
{
  "title": "Buy groceries",
  "description": "Milk, eggs, bread",
  "isCompleted": false
}
Response: 201 Created
```

### Get all TODOs

```
Method: GET
URL: /api/todos
Response: 200 OK
```

### Get TODO by id

```
Method: GET
URL: /api/todos/{id}
Response: 200 OK or 404 Not Found
```

### Update TODO

```
Method: PUT
URL: /api/todos/{id}
Request Body:
{
  "title": "Buy almond milk",
  "description": "Use oat milk instead",
  "isCompleted": true
}
Response: 200 OK
```

### Delete TODO

```
Method: DELETE
URL: /api/todos/{id}
Response: 204 No Content or 404 Not Found
```

---

## Future Improvements

- Add async database access and better error handling.
- Add FluentValidation or data annotations for request validation.
- Add API integration tests for end-to-end coverage.
- Add database migrations and versioning.
- Add swagger request/response schema documentation.
