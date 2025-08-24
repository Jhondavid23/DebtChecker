import type { TabContentProps } from "../../types/TabContextProps";

const TabContent: React.FC<TabContentProps> = ({ activeTab, children }) => {
  return (
    <div className="transition-all duration-200 ease-in-out">
      <div className={`${activeTab === 'lent' ? 'block' : 'hidden'}`}>
        {activeTab === 'lent' && children}
      </div>
      <div className={`${activeTab === 'owed' ? 'block' : 'hidden'}`}>
        {activeTab === 'owed' && children}
      </div>
    </div>
  );
};

export default TabContent;