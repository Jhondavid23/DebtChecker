export interface Debt {
  id: number;
  userId: number;
  debtorId?: number;
  title: string;
  description?: string;
  amount: number;
  currency: string;
  isPaid: boolean;
  dueDate?: string;
  paidAt?: string;
  createdAt: string;
  updatedAt: string;
  ownerName: string;
  ownerEmail: string;
  debtorName?: string;
  debtorEmail?: string;
  status: string;
  isOverdue: boolean;
  formattedAmount: string;
}