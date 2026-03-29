import api from './api';

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

const transactionService = {
  async getTransactions(year: number, month: number, type: string = 'All'): Promise<TransactionDto[]> {
    const response = await api.get<TransactionDto[]>('/api/v1/transactions', {
      params: { year, month, type }
    });
    return response.data;
  }
};

export default transactionService;
