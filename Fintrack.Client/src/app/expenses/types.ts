export interface Expense {
  id: string;
  amount: number;
  currency: string;
  date: string;
  description?: string;
  categoryId: string;
  merchant?: string;
}

export interface InvoiceProduct {
  name: string;
  quantity: number;
  unitPrice: number;
  total: number;
}
