import type { DebtTabsProps } from "../../types/DebtTabsProps";

const DebtTabs: React.FC<DebtTabsProps> = ({ 
  activeTab, 
  onTabChange, 
  lentCount, 
  owedCount 
}) => {
  return (
    <div className="mb-6">
      <div className="border-b border-gray-200">
        <nav className="-mb-px flex space-x-8">
          <button
            onClick={() => onTabChange('lent')}
            className={`whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors ${
              activeTab === 'lent'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Dinero que Presté
            <span className={`ml-2 py-0.5 px-2 rounded-full text-xs ${
              activeTab === 'lent' 
                ? 'bg-blue-100 text-blue-800' 
                : 'bg-gray-100 text-gray-800'
            }`}>
              {lentCount}
            </span>
          </button>
          
          <button
            onClick={() => onTabChange('owed')}
            className={`whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors ${
              activeTab === 'owed'
                ? 'border-orange-500 text-orange-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Dinero que Debo
            <span className={`ml-2 py-0.5 px-2 rounded-full text-xs ${
              activeTab === 'owed' 
                ? 'bg-orange-100 text-orange-800' 
                : 'bg-gray-100 text-gray-800'
            }`}>
              {owedCount}
            </span>
          </button>
        </nav>
      </div>
    </div>
  );
};

export default DebtTabs;