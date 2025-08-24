import type { Debt } from "./Debt";
import type { DebtStatistics } from "./DebtStatistics";

export interface UseDebtsReturn {
  // Deudas
  lentDebts: Debt[];
  owedDebts: Debt[];
  statistics: DebtStatistics | null;
  
  // Estados de carga
  loading: boolean;
  statsLoading: boolean;
  
  // Estados de error
  error: string | null;
  
  // Funciones
  refreshDebts: () => Promise<void>;
  refreshStats: () => Promise<void>;
  markAsPaid: (debtId: number) => Promise<void>;
  deleteDebt: (debtId: number) => Promise<void>;
}