export type FilterStatus = 'all' | 'pending' | 'paid';

export interface FilterBarProps {
  activeFilter: FilterStatus;
  onFilterChange: (filter: FilterStatus) => void;
  debtCounts: {
    all: number;
    pending: number;
    paid: number;
  };
}
