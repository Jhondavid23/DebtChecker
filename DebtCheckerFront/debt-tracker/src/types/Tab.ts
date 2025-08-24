export type TabType = 'lent' | 'owed';
export interface Tab {
  id: TabType;
  label: string;
  count: number;
  icon: string;
}
