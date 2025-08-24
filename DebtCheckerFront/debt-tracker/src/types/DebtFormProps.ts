import type { CreateDebtRequest } from "./CreateDebtRequest";
import type { Debt } from "./Debt";

export interface DebtFormProps {
  debt?: Debt | null; // null para crear, Debt para editar
  onSubmit: (data: CreateDebtRequest) => Promise<void>;
  onCancel: () => void;
  loading: boolean;
}