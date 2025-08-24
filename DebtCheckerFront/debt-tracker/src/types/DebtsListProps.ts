import type { Debt } from "./Debt";

export interface DebtsListProps {
  debts: Debt[];
  loading: boolean;
  type: 'lent' | 'owed';
  onEdit?: (debt: Debt) => void;
  onDelete?: (debtId: number) => void;
  onMarkAsPaid?: (debtId: number) => void;
}