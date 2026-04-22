# CeroBase (Finance Tracker)

CeroBase is a web-based personal budgeting application that helps users track income and expenses, manage monthly budgets, define savings goals, and explore financial what-if scenarios.

## Domain Model (Conceptual View)

The following Mermaid diagram represents the CeroBase domain model at a conceptual level:

```mermaid
graph TD
  CeroBase[CeroBase]
  Users[Users]

  CeroBase -- "serves" --> Users

  subgraph reality["Reality Area"]
    Expenses[Expenses]
    Incomes[Incomes]
    ExpenseCategories[Expense Categories]
    IncomeCategories[Income Categories]
    RecurringExpenses[Recurring Expenses]
    RecurringIncomes[Recurring Incomes]
  end

  subgraph planning["Planning Area"]
    Budgets[Budgets]
    SavingsGoals[Savings Goals]
  end

  subgraph simulation["Simulation Workspace (What-If Analysis)"]
    Scenarios(Scenarios)
    ProjectedAdjustments["Projected Adjustments"]
  end

  Users -- "records" --> Expenses
  Users -- "records" --> Incomes
  Users -- "defines" --> ExpenseCategories
  Users -- "defines" --> IncomeCategories
  Users -- "schedules" --> RecurringExpenses
  Users -- "schedules" --> RecurringIncomes
  Users -- "plans with" --> Budgets
  Users -- "sets" --> SavingsGoals
  Users -- "creates" --> Scenarios

  Expenses -- "is grouped by" --> ExpenseCategories
  Incomes -- "is grouped by" --> IncomeCategories
  Expenses -- "affects" --> Budgets
  RecurringExpenses -- "generates" --> Expenses
  RecurringIncomes -- "generates" --> Incomes
  Budgets -- "supports" --> SavingsGoals
  Scenarios -- "simulates impact on" --> Budgets
  ProjectedAdjustments -- "is applied to" --> Scenarios
  ProjectedAdjustments -- "reclassifies" --> ExpenseCategories
  ProjectedAdjustments -- "reclassifies" --> IncomeCategories
```
