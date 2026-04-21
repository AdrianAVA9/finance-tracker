# CeroBase (Finance Tracker)

CeroBase is a web-based personal budgeting application that helps users track income and expenses, manage monthly budgets, define savings goals, and explore financial what-if scenarios.

## Domain Model (Conceptual View)

The following Mermaid diagram represents the CeroBase domain model at a conceptual level:

```mermaid
graph TD
  CeroBase[CeroBase]
  Users[Users]

  CeroBase -- "serves" --> Users

  subgraph Reality_Area["Reality Area"]
    Transactions[Transactions]
    Accounts[Accounts]
    Categories[Categories]
  end

  subgraph Planning_Area["Planning Area"]
    Budgets[Budgets]
    SavingsGoals[Savings Goals]
  end

  subgraph Simulation_Area["Simulation Area (Sandbox)"]
    Scenarios(Scenarios)
    ProjectedAdjustments(Projected Adjustments)
  end

  Users -- "records" --> Transactions
  Users -- "manages" --> Accounts
  Users -- "defines" --> Categories
  Users -- "plans with" --> Budgets
  Users -- "sets" --> SavingsGoals
  Users -- "creates" --> Scenarios

  Transactions -- "is recorded in" --> Accounts
  Transactions -- "is grouped by" --> Categories
  Transactions -- "affects" --> Budgets
  Budgets -- "supports" --> SavingsGoals
  Scenarios -- "simulates impact on" --> Budgets
  ProjectedAdjustments -- "is applied to" --> Scenarios
  ProjectedAdjustments -- "reclassifies" --> Categories
```
