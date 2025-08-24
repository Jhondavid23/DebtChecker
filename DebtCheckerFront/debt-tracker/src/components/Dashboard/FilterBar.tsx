import React from 'react';
import type { FilterBarProps, FilterStatus } from '../../types/FilterBarProps';

const FilterBar: React.FC<FilterBarProps> = ({
  activeFilter,
  onFilterChange,
  debtCounts
}) => {
  const filterButtons: Array<{
    key: FilterStatus;
    label: string;
    count: number;
    variant: 'all' | 'pending' | 'paid';
  }> = [
    {
      key: 'all',
      label: 'Todas',
      count: debtCounts.all,
      variant: 'all'
    },
    {
      key: 'pending',
      label: 'Pendientes',
      count: debtCounts.pending,
      variant: 'pending'
    },
    {
      key: 'paid',
      label: 'Pagadas',
      count: debtCounts.paid,
      variant: 'paid'
    }
  ];

  const getButtonStyles = (filter: FilterStatus, variant: 'all' | 'pending' | 'paid') => {
    const isActive = activeFilter === filter;
    
    const baseStyles = "inline-flex items-center px-4 py-2 rounded-lg text-sm font-medium transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2";
    
    if (isActive) {
      switch (variant) {
        case 'all':
          return `${baseStyles} bg-blue-100 text-blue-800 border-2 border-blue-200 focus:ring-blue-500`;
        case 'pending':
          return `${baseStyles} bg-yellow-100 text-yellow-800 border-2 border-yellow-200 focus:ring-yellow-500`;
        case 'paid':
          return `${baseStyles} bg-green-100 text-green-800 border-2 border-green-200 focus:ring-green-500`;
        default:
          return `${baseStyles} bg-gray-100 text-gray-800 border-2 border-gray-200`;
      }
    } else {
      return `${baseStyles} bg-white text-gray-600 border-2 border-gray-200 hover:bg-gray-50 hover:text-gray-800 focus:ring-gray-500`;
    }
  };

  const getCountStyles = (filter: FilterStatus, variant: 'all' | 'pending' | 'paid') => {
    const isActive = activeFilter === filter;
    
    if (isActive) {
      switch (variant) {
        case 'all':
          return "ml-2 bg-blue-200 text-blue-800 text-xs font-semibold px-2 py-0.5 rounded-full";
        case 'pending':
          return "ml-2 bg-yellow-200 text-yellow-800 text-xs font-semibold px-2 py-0.5 rounded-full";
        case 'paid':
          return "ml-2 bg-green-200 text-green-800 text-xs font-semibold px-2 py-0.5 rounded-full";
        default:
          return "ml-2 bg-gray-200 text-gray-800 text-xs font-semibold px-2 py-0.5 rounded-full";
      }
    } else {
      return "ml-2 bg-gray-100 text-gray-600 text-xs font-semibold px-2 py-0.5 rounded-full";
    }
  };

  return (
    <div className="mb-6">
      <div className="sm:hidden">
        {/* Mobile dropdown */}
        <select
          value={activeFilter}
          onChange={(e) => onFilterChange(e.target.value as FilterStatus)}
          className="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md"
        >
          {filterButtons.map((button) => (
            <option key={button.key} value={button.key}>
              {button.label} ({button.count})
            </option>
          ))}
        </select>
      </div>

      <div className="hidden sm:block">
        {/* Desktop buttons */}
        <div className="flex flex-wrap gap-2">
          {filterButtons.map((button) => (
            <button
              key={button.key}
              onClick={() => onFilterChange(button.key)}
              className={getButtonStyles(button.key, button.variant)}
              type="button"
            >
              <span>{button.label}</span>
              <span className={getCountStyles(button.key, button.variant)}>
                {button.count}
              </span>
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default FilterBar;
