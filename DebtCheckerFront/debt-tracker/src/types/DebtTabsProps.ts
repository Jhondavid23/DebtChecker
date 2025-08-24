

export interface DebtTabsProps {
  activeTab: 'lent' | 'owed';
  onTabChange: (tab: 'lent' | 'owed') => void;
  lentCount: number;
  owedCount: number;
}