import { useCallback, useState } from "react";
import type { TabType } from "../types/Tab";
import type { UseTabStateReturn } from "../types/UseTabStateReturn";

export const useTabState = (initialTab: TabType = 'lent'): UseTabStateReturn => {
  const [activeTab, setActiveTabState] = useState<TabType>(() => {
    // Intentar recuperar el estado del localStorage
    if (typeof window !== 'undefined') {
      const stored = localStorage.getItem('debtTracker_activeTab');
      if (stored === 'lent' || stored === 'owed') {
        return stored;
      }
    }
    return initialTab;
  });

  const setActiveTab = useCallback((tab: TabType) => {
    setActiveTabState(tab);
    // Persistir el estado
    if (typeof window !== 'undefined') {
      localStorage.setItem('debtTracker_activeTab', tab);
    }
  }, []);

  return {
    activeTab,
    setActiveTab,
    isLentTab: activeTab === 'lent',
    isOwedTab: activeTab === 'owed'
  };
};