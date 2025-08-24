export interface DebtStatistics {
  totalDebts: number;
  pendingDebts: number;
  paidDebts: number;
  overdueDebts: number;
  totalAmount: number;
  pendingAmount: number;
  paidAmount: number;
  overdueAmount: number;
  currency: string;
  lastUpdated: string;
}