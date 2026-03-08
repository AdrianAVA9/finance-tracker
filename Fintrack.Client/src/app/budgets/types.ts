export interface Budget {
  id: string;
  name: string;
  limit: number;
  spent: number;
  currency: string;
  startDate: string;
  endDate: string;
}

export interface CreateBudgetDto {
  name: string;
  limit: number;
  currency: string;
}
