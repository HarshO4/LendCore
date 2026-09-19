# Key AI Prompts

This document extracts the main AI prompts used during the development of the Lending Platform. They are presented in development order and correspond to the activities documented in the AI Development Log. The prompts focus on requirements, architecture, implementation, testing, debugging, UI/UX, and final verification.

---

## 1. Requirements Analysis

**Prompt**

> Analyse the lending platform requirements and convert the business rules into explicit decision conditions, including LTV calculation, loan amount boundaries, credit score thresholds, validation rules, and statistics requirements. Identify all boundary cases that should be tested.

**Purpose**

To convert the assignment into precise, testable requirements and identify important boundaries such as £100,000, £1,000,000, £1,500,000 and LTV thresholds of 60%, 80% and 90%. 

---

## 2. Architecture Design

**Prompt**

> Design a simple production-style lending platform using ASP.NET Core Web API, EF Core with SQLite, React with Vite, DTOs, services, controllers, automated tests, and Swagger. Keep the architecture modular but avoid unnecessary microservices or infrastructure.

**Purpose**

To establish a maintainable modular-monolith structure with controllers, services/interfaces, DTOs/entities, and the data layer. The backend should own lending decisions while the frontend handles presentation and interaction. 

---

## 3. Backend Decision Engine

**Prompt**

> Implement the lending decision service using the documented lending rules. Keep business logic in the backend service, use full precision for calculations, persist valid applications, and expose clean API endpoints for applications and statistics.

**Purpose**

To implement the core lending decision logic, persistence, statistics, DTOs, services, controllers, dependency injection, and Swagger/OpenAPI support while keeping the backend authoritative. 

---

## 4. Automated Testing

**Prompt**

> Create xUnit tests for the lending decision engine. Cover approved and declined applications, every LTV band, credit score boundaries, loan amount boundaries, invalid input, exact threshold values, statistics, and integration behavior.

**Purpose**

To verify both normal and boundary scenarios, including validation, LTV calculation, approval/decline rules, thresholds, statistics, and integration behavior. The final backend test suite passed 81 tests. 

---

## 5. Numerical Correctness Review

**Prompt**

> Review the LTV calculation and investigate any unexpectedly large values. Verify the formula, percentage conversion, zero/negative asset handling, precision, and rounding. Explain whether the observed result is mathematically valid.

**Purpose**

To investigate unexpected LTV values instead of assuming they were implementation errors. The review checked the formula, division-by-zero conditions, percentage conversion, duplicate multiplication by 100, calculation precision, and display rounding. 

---

## 6. Frontend/API Integration Debugging

**Prompt**

> The frontend is reporting `Failed to fetch`. Diagnose the issue systematically by checking the backend process, API URL, frontend URL, Vite proxy, CORS configuration, browser network requests, and endpoint availability. Do not change business logic unless evidence points to a logic problem.

**Purpose**

To systematically separate frontend/API configuration issues from business-logic defects and verify the frontend and backend independently and together.

---

## 7. Runtime Issue Investigation

**Prompt**

> The configured backend port is already in use. Diagnose which development process is occupying the port, determine whether it belongs to the application, and resolve the conflict without changing the application architecture.

**Purpose**

To treat port conflicts as runtime/environment issues rather than incorrectly modifying application logic. 

---

## 8. Integration Test Configuration

**Prompt**

> Review the integration-test database configuration for conflicts between the production SQLite provider and the test database provider. Isolate the integration-test database configuration so tests remain reliable without weakening the production configuration or test coverage.

**Purpose**

To resolve the database-provider conflict while preserving test quality. A dedicated in-memory test factory was used rather than weakening the tests. 

---

## 9. Frontend and UI/UX Review

**Prompt**

> Review the lending dashboard from a professional product perspective. Keep it simple and interview-friendly. Improve hierarchy, spacing, table readability, decision visibility, responsive behavior, and usability without adding unnecessary enterprise features.

**Purpose**

To improve the dashboard while keeping the assignment focused. The resulting UI included the application form, decision card, history, statistics, search/filter controls, details, policy drawer, refresh/reset actions, loading/error/empty states, and responsive behavior. 

---

## 10. Documentation Review

**Prompt**

> Review the project documentation against the implemented application. Ensure the README, business rules, architecture documentation, AI development log, and key prompts accurately describe the implemented behavior and do not contain unsupported claims.

**Purpose**

To keep project documentation synchronized with the actual implementation and verification results. 

---

## 11. Final Release Audit

**Prompt**

> Perform a final senior-engineer audit of the lending platform. Check business-rule correctness, validation, boundary cases, persistence, API behavior, automated tests, frontend integration, responsive UI, documentation, and unnecessary complexity. Identify anything that could fail during a live demonstration and recommend only necessary fixes.

**Purpose**

To perform final verification across the complete application before submission. 

---

# Engineering Prompting Principles

The prompts consistently followed these principles:

1. Keep the assignment requirements as the source of truth.
2. Identify ambiguity rather than silently inventing business rules.
3. Keep lending decisions in the backend.
4. Use numerical precision correctly.
5. Test boundary conditions explicitly.
6. Make small and verifiable changes.
7. Investigate unexpected output using evidence.
8. Do not weaken tests to make them pass.
9. Separate runtime/configuration issues from application defects.
10. Avoid unnecessary architecture and features.
11. Keep documentation synchronized with implementation.
12. Distinguish AI-generated suggestions from verified application behavior. 

---

# Development Sequence

The prompts were applied across the development lifecycle:

Requirements Analysis
→ Business Rules
→ Architecture
→ Backend Implementation
→ Database/API
→ Testing
→ Frontend
→ Integration
→ Debugging
→ UI/UX
→ Documentation
→ Final Verification

This reflects the documented development approach and the principle that AI was used as a productivity assistant while final decisions were validated through tests, builds, API behavior, and browser testing. 
