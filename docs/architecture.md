# System Architecture

The Lending Platform follows a clean Modular Monolith architectural style.

## Data Persistence

### Database
- **SQLite**: Selected for its portability and zero-configuration setup, allowing immediate out-of-the-box execution for reviewers. 
- **Database File**: The application generates a `lending-platform-dev.db` file in the `backend/LendingPlatform.Api` folder at startup.

### Entity Framework Core Setup
- **Migrations**: Automatically applied at startup when running in the `Development` environment.
- **Data Types**: Because SQLite lacks a native decimal type, `decimal` properties (LoanAmount, AssetValue, LTV) are strictly mapped to `TEXT` columns in `ApplicationDbContext` to prevent float rounding errors.

## Backend Architecture

### Layers & Responsibilities

1. **Controllers (`Controllers/`)**: Thin presentation layer bridging HTTP and core logic. Translates DTOs, extracts configuration, and returns uniform HTTP responses.
2. **Services (`Services/`)**: Pure business logic orchestrators. `LoanDecisionService` executes the policy without side-effects. `StatisticsService` processes portfolio calculations via EF Core.
3. **Data (`Data/`)**: EF Core DbContext and migrations.
4. **Domain/Models (`Models/`)**: Encompasses Entities (database persistence), DTOs (Data Transfer Objects with DataAnnotation validations), and internal domain models (`LoanDecisionResult`).
5. **Policy (`Policy/`)**: Centralized static constants for business configuration.

### Cross-Origin Resource Sharing (CORS)
- CORS is strictly configured to only allow requests from `http://localhost:5173` and `http://127.0.0.1:5173` (the Vite local server).

## Testing Strategy

- **xUnit**: The core test runner.
- **Unit Tests**: Over 65 tests isolate the `LoanDecisionService` to rapidly evaluate every combination of the lending rules.
- **Integration Tests**: Utilizes `WebApplicationFactory` to spin up a mock server. Replaces the underlying SQLite provider with the `InMemory` database provider to rapidly evaluate HTTP behavior, dependency injection correctness, and controller logic without producing file locks.

## Frontend Architecture

- **React + Vite**: A fast-compiling toolchain for a responsive dashboard.
- **State Management**: Simple React hooks (`useState`, `useEffect`) manage API responses. No complex reducers or external state management libraries (Redux/MobX) were necessary given the bounded context of the app.
- **Styling**: Vanilla CSS in `index.css`. Makes extensive use of CSS variables for a consistent theme, Flexbox/Grid for layout, and modern responsive patterns without the overhead of heavy utility frameworks.
- **Service Layer**: An isolated Axios configuration (`src/services/api.js`) centralizes HTTP interactions and endpoint URLs, easily toggled via the `.env` configuration file.
