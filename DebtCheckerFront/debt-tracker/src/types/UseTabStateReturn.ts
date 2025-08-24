import type { TabType } from "./Tab";

export interface UseTabStateReturn {
  activeTab: TabType;
  setActiveTab: (tab: TabType) => void;
  isLentTab: boolean;
  isOwedTab: boolean;
}