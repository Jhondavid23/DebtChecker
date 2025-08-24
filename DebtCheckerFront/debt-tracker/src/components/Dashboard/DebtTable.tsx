import React from 'react';
import { EyeIcon } from '@heroicons/react/24/outline';
import type { Debt } from '../../types/Debt';
import LoadingState from './LoadingState';
import EmptyState from './EmptyState';
import type { DebtTableProps } from '../../types/DebtTableProps';

const DebtTable: React.FC<DebtTableProps> = ({ debts, loading, type, onViewDetail, onCreateDebt }) => {
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0,
    }).format(amount);
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('es-CO', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const getStatusBadge = (debt: Debt) => {
    if (debt.isPaid) {
      return (
        <span className="inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800">
          Pagada
        </span>
      );
    }
    
    const now = new Date();
    const dueDate = debt.dueDate ? new Date(debt.dueDate) : null;
    
    if (dueDate && dueDate < now) {
      return (
        <span className="inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-red-100 text-red-800">
          Vencida
        </span>
      );
    }
    
    return (
      <span className="inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-yellow-100 text-yellow-800">
        Pendiente
      </span>
    );
  };

  if (loading) {
    return <LoadingState />;
  }

  if (debts.length === 0) {
    return <EmptyState type={type} onCreateDebt={onCreateDebt} />;
  }

  return (
    <div className="overflow-hidden shadow ring-1 ring-black ring-opacity-5 md:rounded-lg">
      <table className="min-w-full divide-y divide-gray-300">
        <thead className="bg-gray-50">
          <tr>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Deuda
            </th>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Monto
            </th>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              {type === 'lent' ? 'Deudor' : 'Acreedor'}
            </th>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Estado
            </th>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Fecha creación
            </th>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Vencimiento
            </th>
            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Acciones
            </th>
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {debts.map((debt) => (
            <tr
              key={debt.id}
              className="hover:bg-gray-50 cursor-pointer transition-colors"
              onClick={() => onViewDetail(debt)}
            >
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="text-sm font-medium text-gray-900">{debt.title}</div>
                {debt.description && (
                  <div className="text-sm text-gray-500 truncate max-w-xs">
                    {debt.description}
                  </div>
                )}
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="text-sm font-medium text-gray-900">
                  {formatCurrency(debt.amount)}
                </div>
                <div className="text-sm text-gray-500">
                  {debt.currency}
                </div>
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="text-sm text-gray-900">
                  {type === 'lent' ? debt.debtorName : debt.debtorName}
                </div>
                <div className="text-sm text-gray-500">
                  {type === 'lent' ? debt.debtorEmail : debt.debtorEmail}
                </div>
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                {getStatusBadge(debt)}
              </td>
              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {formatDate(debt.createdAt)}
              </td>
              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {debt.dueDate ? formatDate(debt.dueDate) : '-'}
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    onViewDetail(debt);
                  }}
                  className="text-blue-600 hover:text-blue-900 transition-colors"
                  title="Ver detalle"
                >
                  <EyeIcon className="h-5 w-5" />
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default DebtTable;