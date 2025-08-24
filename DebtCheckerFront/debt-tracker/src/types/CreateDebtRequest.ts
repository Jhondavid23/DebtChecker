export interface CreateDebtRequest {
  title: string;
  description?: string;
  amount: number;
  currency: string;
  debtorId?: number;
  dueDate?: string;
}