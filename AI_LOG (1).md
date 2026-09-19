# AI Development Log

## 1. Purpose

AI tools were used as a development assistant throughout the Lending Platform project. The main uses were requirements analysis, architecture planning, implementation support, test design, debugging, UI/UX refinement, documentation, and final verification.

AI-generated suggestions were treated as proposals. Important decisions and outputs were reviewed against the assignment requirements and verified through tests, builds, API behavior, and browser testing.

---

## 2. Development Approach

The development process followed this sequence:

1. Requirements analysis
2. Business-rule clarification
3. Architecture design
4. Backend implementation
5. Database and API implementation
6. Automated test development
7. Frontend implementation
8. API/frontend integration
9. Debugging and correction
10. UI/UX refinement
11. Documentation
12. Final verification

The goal was to use AI to accelerate development without allowing generated code or suggestions to replace engineering validation.

---

## 3. AI-Assisted Requirements Analysis

### Objective
Convert the assignment into precise, testable business rules.

### Key AI Prompt
> Analyse the lending platform requirements and convert the business rules into explicit decision conditions, including LTV calculation, loan amount boundaries, credit score thresholds, validation rules, and statistics requirements. Identify all boundary cases that should be tested.

### Output / Use
The requirements were organized into:
- Input validation rules
- LTV calculation
- Low-value loan rules
- High-value loan rules
- LTV/credit-score decision bands
- Statistics requirements
- Persistence requirements

Special attention was given to exact boundaries such as £100,000, £1,000,000, £1,500,000 and LTV values of 60%, 80% and 90%.

### Engineering Review
The final rules were checked against the original assignment before implementation. Boundary behavior was explicitly tested rather than assumed.

---

## 4. AI-Assisted Architecture Design

### Objective
Design a maintainable full-stack structure without unnecessary complexity.

### Key AI Prompt
> Design a simple production-style lending platform using ASP.NET Core Web API, EF Core with SQLite, React with Vite, DTOs, services, controllers, automated tests, and Swagger. Keep the architecture modular but avoid unnecessary microservices or infrastructure.

### Output / Use
The application was structured as a modular monolith:

Controllers
→ Services / Interfaces
→ DTOs / Entities
→ Data / EF Core

The backend owns the lending decision logic, while the React frontend is responsible for presentation and user interaction.

### Engineering Review
The design was checked against the assignment scope. Unnecessary components such as microservices, authentication, payment processing, and unrelated infrastructure were intentionally excluded.

---

## 5. AI-Assisted Backend Implementation

### Objective
Implement the lending decision engine, persistence, APIs, and statistics.

### Key Prompt
> Implement the lending decision service using the documented lending rules. Keep business logic in the backend service, use full precision for calculations, persist valid applications, and expose clean API endpoints for applications and statistics.

### Output / Use
AI assistance was used for implementation patterns and code structure around:
- Decision service
- DTOs
- Entities
- EF Core DbContext
- Application service
- Statistics service
- Controllers
- Dependency injection
- Swagger/OpenAPI

The backend was implemented using .NET 10, ASP.NET Core Web API, EF Core and SQLite.

### Engineering Review
Generated implementation was reviewed against the business rules and verified through automated tests and API execution.

---

## 6. AI-Assisted Testing

### Objective
Ensure the decision engine behaves correctly for normal cases, invalid inputs, and boundary conditions.

### Key Prompt
> Create xUnit tests for the lending decision engine. Cover approved and declined applications, every LTV band, credit score boundaries, loan amount boundaries, invalid input, exact threshold values, and statistics.

### Output / Use
Tests were created for:
- Input validation
- LTV calculation
- Approval rules
- Decline rules
- Credit-score thresholds
- LTV thresholds
- Loan amount boundaries
- Statistics
- Integration behavior

### Engineering Review
The test suite was executed after implementation and corrections. The final backend test suite passed with 81 tests.

---

## 7. AI Output That Was Questioned or Corrected

### 7.1 LTV Calculation

An unexpectedly large LTV value was investigated rather than accepted automatically.

The calculation was checked using:

LTV = (Loan Amount / Asset Value) × 100

The review specifically checked:
- Division by zero
- Negative or zero asset values
- Percentage conversion
- Whether ×100 was accidentally applied twice
- Full precision during rule evaluation
- Rounding only for display

This confirmed that unusual LTV values can be valid when the loan amount is much larger than the asset value.

---

### 7.2 Frontend/API Integration

A `Failed to fetch` error occurred during browser testing.

The issue was investigated across:
- Backend process
- API URL
- Frontend URL
- Vite proxy configuration
- CORS configuration
- Browser network requests
- Backend availability

The frontend and backend were then verified independently and together.

---

### 7.3 Runtime Port Issue

A development backend process was already occupying the configured API port.

The process was identified and stopped, after which the backend was restarted and verified.

This was treated as a local runtime/environment issue rather than a business-logic defect.

---

### 7.4 Integration Test Provider Conflict

An integration-test configuration issue involving database providers was identified.

Instead of weakening the tests, the integration test setup was isolated with a dedicated in-memory test factory so that integration tests could run independently from the production SQLite configuration.

---

## 8. AI-Assisted Frontend and UI/UX

### Objective
Create a professional lending dashboard without overloading the assignment with unnecessary features.

### Key Prompt
> Review the lending dashboard from a professional product perspective. Keep it simple and interview-friendly. Improve hierarchy, spacing, table readability, decision visibility, responsive behavior, and usability without adding unnecessary enterprise features.

### Output / Use
The frontend was refined to include:
- Responsive dashboard
- Application form
- Decision result card
- Application history
- Statistics cards
- Search/filter controls
- Application detail view
- Lending policy drawer
- Refresh and reset actions
- Loading, error, and empty states
- Responsive mobile layout

A short decision animation was also used to improve the submission experience while keeping the backend response authoritative.

### Engineering Review
The UI was reviewed for clarity, scope, responsiveness, and consistency. Decorative complexity was avoided.

---

## 9. AI-Assisted Documentation

AI assistance was used to structure:
- README.md
- Business rules documentation
- Architecture documentation
- AI development log
- Key prompts

Documentation was checked against the implemented application so that documented behavior remained consistent with the actual system.

---

## 10. Engineering Principles Used

Throughout AI-assisted development:

1. Assignment requirements remained the source of truth.
2. Backend business logic remained authoritative.
3. AI-generated code was reviewed before acceptance.
4. Boundary conditions were explicitly tested.
5. Unexpected numerical results were investigated.
6. Tests were not weakened to hide implementation problems.
7. Runtime issues were separated from application-logic issues.
8. Features were added only when they improved the assignment.
9. Unnecessary architecture and scope were avoided.
10. Documentation was kept aligned with the implementation.

---

## 11. Final Verification

The completed project was verified through:

- Backend build
- xUnit automated tests
- Frontend build
- Swagger/API verification
- Application submission
- Approval and decline scenarios
- Invalid-input handling
- SQLite persistence
- Statistics calculation
- Application history
- Backend/frontend integration
- Responsive browser behavior
- Runtime error investigation

The final backend test suite passed 81 tests.

---

## 12. Summary

AI was used as an engineering productivity tool for analysis, implementation assistance, testing, debugging, UI refinement, and documentation.

The final implementation was validated through source review, automated tests, builds, API verification, and browser testing. Decisions were based on the assignment requirements and observed system behavior rather than accepting AI output without verification.
