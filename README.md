# LendCore

**Intelligent lending decision platform**

A full-stack professional lending platform that evaluates loan applications against a centralized lending policy. It provides a React dashboard to submit new loan applications, review the latest decisions, and browse historical applications with real-time portfolio statistics.


## Setup Instructions

### Backend Startup

1. Open a terminal in the root directory and navigate to the backend API folder:
   ```bash
   cd backend/LendingPlatform.Api
   ```
2. Restore packages and build:
   ```bash
   dotnet restore
   dotnet build
   ```
3. Run the API:
   ```bash
   dotnet run
   ```
   *Note: In the Development environment, EF Core migrations are applied automatically on startup. The API will start on `http://localhost:5207`.*
4. Access Swagger UI: [http://localhost:5207/swagger](http://localhost:5207/swagger)

### Frontend Startup

1. Open a new terminal in the root directory and navigate to the frontend folder:
   ```bash
   cd frontend/lending-platform-client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm run dev
   ```
   *The frontend will start on `http://localhost:5173`.*

### Test Commands

To run the comprehensive test suite (Unit tests + In-Memory Integration tests):

```bash
cd backend/LendingPlatform.Tests
dotnet test
```


## Features

- **Automated Decision Engine**: Evaluates applications instantly based on loan amount, asset value, credit score, and calculated LTV (Loan-to-Value).
- **Dashboard**: Professional, responsive React-based dashboard displaying live portfolio statistics.
- **Application History**: View previous loan applications, search by ID, export as CSV, expand for detailed reasoning, and filter by decision.
- **Data Persistence**: Valid applications (both approved and declined) are persisted to a SQLite database. Invalid inputs are rejected without persistence.
- **Productivity Tools**: One-click ID copying, quick form resets, and live API connectivity monitoring.

## Technology Stack

### Backend
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite Database
- xUnit (for unit & integration testing)

### Frontend
- React 18
- Vite
- Axios
- Lucide React (icons)
- Vanilla CSS (No heavy UI frameworks)

## Architecture

The application follows a Clean Modular-Monolith architecture.

```text
React Frontend (Vite)
       |
       | REST / HTTP
       v
ASP.NET Core Web API (Controllers)
       |
       v
Application Services
       |
       +---- Loan Decision Service
       |
       +---- Statistics Service
       |
       v
Entity Framework Core
       |
       v
SQLite Database
```

### Directory Structure

```text
lending-platform/
├── backend/
│   ├── LendingPlatform.Api/
│   │   ├── Controllers/     # HTTP endpoint handlers
│   │   ├── Data/            # EF Core DbContext and Migrations
│   │   ├── Models/          # Entities, DTOs, and Domain results
│   │   ├── Policy/          # Centralized business rule constants
│   │   └── Services/        # Domain logic and statistics generation
│   └── LendingPlatform.Tests/ # xUnit test suites (Unit & Integration)
├── frontend/
│   └── lending-platform-client/
│       ├── src/
│       │   ├── components/  # React components
│       │   ├── services/    # API communication (Axios)
│       │   └── utils/       # Formatting helpers
└── docs/                    # Additional architectural and business documentation
```

See [docs/architecture.md](docs/architecture.md) for more details.

## API Endpoints

- `POST /api/applications` - Submit a new loan application. Returns the persisted decision and evaluation reasoning.
- `GET /api/applications?decision={filter}` - Retrieve persisted applications (optionally filtered by 'Approved' or 'Declined').
- `GET /api/statistics` - Retrieve real-time portfolio statistics calculated from the SQLite database.

See [Example API Requests](docs/architecture.md) for payload details.

## Business Rules & LTV Calculation

LTV is calculated dynamically as a percentage: `(Loan Amount / Asset Value) × 100`. 
Lending decisions depend on strict LTV bands, credit score minimums, and loan boundaries.

See [docs/business-rules.md](docs/business-rules.md) for a complete breakdown of the lending policy.

## Design Decisions

- **Precision & Storage**: Decimal types are used extensively in .NET and SQLite mapping to avoid floating-point drift in financial data. LTV is stored as a direct percentage value (e.g., 50.00 for 50%) rather than a fractional multiplier.
- **Service Isolation**: The core decision engine (`LoanDecisionService`) contains no I/O operations, ensuring it is 100% pure and seamlessly unit-testable in isolation.
- **Validation vs. Persistence**: Application payloads failing HTTP constraints (e.g., negative loan values) yield immediate HTTP 400s without DB trips. Valid boundary edge-cases (e.g., 95% LTV, which auto-declines) are processed and persisted as valid historical business decisions.
- **Test Strategy**: A mix of pure unit tests for the core engine + WebApplicationFactory integration tests with an in-memory EF Core provider ensures full coverage without slowing down the test runner with file-I/O locks.

## Screenshots

<img width="1897" height="867" alt="image" src="https://github.com/user-attachments/assets/81b310fd-2e7d-4fea-9069-785f8f839ed1" />
<img width="1906" height="862" alt="image" src="https://github.com/user-attachments/assets/5f414aa1-1407-493c-bdc8-29bd7ca590aa" />
<img width="1491" height="617" alt="image" src="https://github.com/user-attachments/assets/a582adf5-261e-484c-bef2-6a371c181eca" />


