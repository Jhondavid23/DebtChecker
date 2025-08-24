import type { Debt } from "./Debt";

export interface DebtTableProps {
  debts: Debt[];
  loading: boolean;
  type: 'lent' | 'owed';
  onViewDetail: (debt: Debt) => void;
  onCreateDebt?: () => void;
}