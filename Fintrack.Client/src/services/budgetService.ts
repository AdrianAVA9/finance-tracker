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

const budgetService = {
    async getBudgetDetails(id: number, month: number, year: number): Promise<BudgetDetailsDto> {
        const response = await api.get<BudgetDetailsDto>(`/api/v1/budgets/${id}/details`, {
            params: { month, year }
        });
        return response.data;
    }
};

export default budgetService;
