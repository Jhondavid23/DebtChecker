import type { Debt } from "./Debt";

export interface EditDebtModalProps {
  isOpen: boolean;
  debt: Debt | null;
  onClose: () => void;
  onSuccess: () => void;
}