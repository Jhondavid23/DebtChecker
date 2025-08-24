import type { Debt } from "./Debt";

export interface DebtDetailModalProps {
  isOpen: boolean;
  debt: Debt | null;
  type: 'lent' | 'owed';
  onClose: () => void;
  onMarkAsPaid?: (debtId: number) => Promise<void>;
}
