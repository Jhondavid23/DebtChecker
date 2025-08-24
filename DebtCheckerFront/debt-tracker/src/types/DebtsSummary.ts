import type { Debt } from "./Debt";
import type { DebtStatistics } from "./DebtStatistics";
import type { PaginatedResult } from "./PaginatedResult";

export interface DebtsSummary {
  debtsILent: PaginatedResult<Debt> | null;
  debtsIOwn: PaginatedResult<Debt> | null;
  statistics: DebtStatistics | null;
}