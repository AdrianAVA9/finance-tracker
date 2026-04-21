# CeroBase (Finance Tracker)

CeroBase is a web-based personal budgeting application that helps users track income and expenses, manage monthly budgets, define savings goals, and explore financial what-if scenarios.

## Domain Model (Conceptual View)

The following Mermaid diagram represents the CeroBase domain model at a conceptual level:

```mermaid
graph LR
  %% Area de Realidad
  subgraph Reality_Area["Reality Area"]
    Transactions[Transactions]
    Accounts[Accounts]
    Categories[Categories]
  end

  %% Area de Planificacion
  subgraph Planning_Area["Planning Area"]
    Budgets[Budgets]
    SavingsGoals[Savings Goals]
  end

  %% Area de Simulacion (Sandbox)
  subgraph Simulation_Area["Simulation Area (Sandbox)"]
    Scenarios(Scenarios)
    ProjectedAdjustments(Projected Adjustments)
  end

  Transactions -- "is recorded in" --> Accounts
  Transactions -- "is grouped by" --> Categories
  Transactions -- "affects" --> Budgets
  Categories -- "organizes" --> Budgets
  Budgets -- "guides" --> SavingsGoals
  SavingsGoals -- "is projected in" --> Scenarios
  Scenarios -- "simulates impact on" --> Budgets
  ProjectedAdjustments -- "is applied to" --> Scenarios
  ProjectedAdjustments -- "affects" --> SavingsGoals
  ProjectedAdjustments -- "reclassifies" --> Categories
```
