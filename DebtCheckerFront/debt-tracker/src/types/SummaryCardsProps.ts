import type { DebtStatistics } from "./DebtStatistics";
import type { Debt } from "./Debt";

export interface SummaryCardsProps {
  statistics: DebtStatistics | null;
  loading: boolean;
  owedDebts?: Debt[];
}