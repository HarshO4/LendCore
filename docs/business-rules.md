# Lending Policy & Business Rules

This document outlines the strict business rules implemented in the `LoanDecisionService`. 

All constants for these rules are centralized in `backend/LendingPlatform.Api/Policy/LendingPolicy.cs`.

## Input Validation
- **Loan Amount**: > £0
- **Asset Value**: > £0
- **Credit Score**: 1 to 999 inclusive

*Invalid inputs immediately return HTTP 400 and are never persisted.*

## LTV (Loan-to-Value) Calculation
LTV is computed internally as a percentage: `(Loan Amount / Asset Value) × 100`.
No rounding is applied prior to evaluating rules; precision is fully maintained during evaluation.

## Lending Rules

Applications are evaluated strictly in the following order:

### 1. Minimum Loan Amount
- **Condition**: Loan Amount < £100,000
- **Decision**: Declined

### 2. Maximum Loan Amount
- **Condition**: Loan Amount > £1,500,000
- **Decision**: Declined

### 3. High-Value Loans
- **Condition**: Loan Amount ≥ £1,000,000
- **Approval Requirements** (MUST meet BOTH):
  - LTV ≤ 60%
  - Credit Score ≥ 950
- **Decision**: Approved if both met, otherwise Declined.

### 4. Standard Loans (Below £1,000,000)
Evaluated based on LTV bands and their respective minimum credit score requirements:

| LTV Band | Required Credit Score | Decision Outcome if Met |
| :--- | :--- | :--- |
| **LTV < 60%** | ≥ 750 | Approved |
| **60% ≤ LTV < 80%** | ≥ 800 | Approved |
| **80% ≤ LTV < 90%** | ≥ 900 | Approved |
| **LTV ≥ 90%** | N/A | **Declined** (Auto-Decline) |

## Persistence Rules
Any application that passes initial input validation is considered a "valid application" and is fully evaluated and persisted to the SQLite database, regardless of whether the final business decision is **Approved** or **Declined**.
