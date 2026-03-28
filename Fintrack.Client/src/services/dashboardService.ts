import api from './api';

export interface MonthlyDataDto {
  month: string;
  income: number;
  expense: number;
}

export interface CategorySummaryDto {
  categoryName: string;
  amount: number;
  percentage: number;
  color: string | null;
}

export interface BudgetSummaryDto {
  id: number;
  categoryName: string;
  totalBudget: number;
  spentAmount: number;
  remainingAmount: number;
  percentage: number;
  icon: string | null;
  color: string | null;
}

export interface TransactionDto {
  id: string;
  description: string;
  categoryName: string;
  categoryIcon: string | null;
  categoryColor: string | null;
  date: string;
  amount: number;
  type: 'Income' | 'Expense';
}

export interface DashboardSummaryDto {
  totalBalance: number;
  monthlyIncome: number;
  monthlyExpenses: number;
  incomeChangePercentage: number;
  expenseChangePercentage: number;
  monthlyData: MonthlyDataDto[];
  topSpendingCategories: CategorySummaryDto[];
  recentTransactions: TransactionDto[];
  topBudgets: BudgetSummaryDto[];
}

const dashboardService = {
  async getDashboardSummary(): Promise<DashboardSummaryDto> {
    const response = await api.get<DashboardSummaryDto>('/api/v1/dashboard/summary');
    return response.data;
  },
};

export default dashboardService;
