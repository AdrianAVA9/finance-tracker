import api from './api';

export interface ExpenseItemDto {
    id: number;
    description: string;
    amount: number;
    date: string;
}

export interface MonthlyExpenseSummaryDto {
    month: number;
    year: number;
    totalExpense: number;
    expenses: ExpenseItemDto[];
}

export interface BudgetDetailsDto {
    id: number;
    categoryName: string;
    limitAmount: number;
    monthlyHistory: MonthlyExpenseSummaryDto[];
}

export interface BudgetEntryDto {
    categoryId: string;
    amount: number;
    isRecurrent?: boolean;
}

export interface UpsertBudgetsRequest {
    month: number;
    year: number;
    budgets: BudgetEntryDto[];
}

export interface Budget {
    id: string;
    categoryId: string;
    categoryName: string;
    categoryIcon?: string;
    categoryColor?: string;
    categoryGroup?: string;
    limitAmount: number;
    spentAmount: number;
    isRecurrent?: boolean;
}

export interface BudgetsResponse {
    budgets: Budget[];
    monthlyIncome: number;
}

const budgetService = {
    async getBudgets(month: number, year: number): Promise<BudgetsResponse> {
        const response = await api.get<BudgetsResponse>('/api/v1/budgets', {
            params: { month, year }
        });
        return response.data;
    },

    async getBudgetDetails(id: string, month: number, year: number): Promise<BudgetDetailsDto> {
        const response = await api.get<BudgetDetailsDto>(`/api/v1/budgets/${id}/details`, {
            params: { month, year }
        });
        return response.data;
    },

    async upsertBatch(request: UpsertBudgetsRequest): Promise<void> {
        await api.post('/api/v1/budgets/batch', request);
    }
};

export default budgetService;
