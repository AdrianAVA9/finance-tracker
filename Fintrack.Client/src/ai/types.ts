export interface ExtractedInvoiceData {
  merchant: string;
  date: string;
  totalAmount: number;
  currency: string;
  lineItems: Array<{
    description: string;
    amount: number;
  }>;
}

export interface ClassificationResult {
  categoryId: string;
  confidence: number;
}
