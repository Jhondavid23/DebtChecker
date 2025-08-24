import React, { useState, useMemo } from 'react';
import Layout from '../components/Layout/Layout';
import SummaryCards from '../components/Dashboard/SummaryCards';
import DebtTable from '../components/Dashboard/DebtTable';
import DebtDetailModal from '../components/Dashboard/DebtDetailModal';
import CreateDebtModal from '../components/Dashboard/CreateDebtModal';
import FloatingActionButton from '../components/Dashboard/FloatingActionButton';
import FilterBar from '../components/Dashboard/FilterBar';
import { useDebts } from '../hooks/useDebts';
import type { Debt } from '../types/Debt';
import type { FilterStatus } from '../types/FilterBarProps';
import DebtTabs from '../components/Dashboard/DebTabs';

const DashboardPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'lent' | 'owed'>('lent');
  const [activeFilter, setActiveFilter] = useState<FilterStatus>('all');
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [selectedDebt, setSelectedDebt] = useState<Debt | null>(null);
  
  const {
    lentDebts,
    owedDebts,
    statistics,
    loading,
    statsLoading,
    error,
    refreshDebts,
  } = useDebts();

  const handleTabChange = (tab: 'lent' | 'owed') => {
    setActiveTab(tab);
    setActiveFilter('all'); // Reset filter when changing tabs
  };

  const handleFilterChange = (filter: FilterStatus) => {
    setActiveFilter(filter);
  };

  const handleViewDetail = (debt: Debt) => {
    setSelectedDebt(debt);
    setShowDetailModal(true);
  };

  const handleCloseDetailModal = () => {
    setShowDetailModal(false);
    setSelectedDebt(null);
  };

  const handleCreateDebt = () => {
    setShowCreateModal(true);
  };

  const handleCloseCreateModal = () => {
    setShowCreateModal(false);
  };

  const handleDebtSuccess = async () => {
    await refreshDebts();
  };

  const handleMarkAsPaid = async (_debtId: number) => {
    // Refresh the debts after marking as paid
    await refreshDebts();
  };

  // Filtrar deudas según el filtro activo
  const filteredDebts = useMemo(() => {
    const currentDebts = activeTab === 'lent' ? lentDebts : owedDebts;
    
    switch (activeFilter) {
      case 'pending':
        return currentDebts.filter(debt => !debt.isPaid);
      case 'paid':
        return currentDebts.filter(debt => debt.isPaid);
      default:
        return currentDebts;
    }
  }, [activeTab, activeFilter, lentDebts, owedDebts]);

  // Calcular contadores para filtros
  const getDebtCounts = (debts: Debt[]) => {
    return {
      all: debts.length,
      pending: debts.filter(debt => !debt.isPaid).length,
      paid: debts.filter(debt => debt.isPaid).length,
    };
  };

  const currentDebts = activeTab === 'lent' ? lentDebts : owedDebts;
  const debtCounts = getDebtCounts(currentDebts);

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
            <p className="text-gray-600">Gestiona tus deudas de manera bidireccional</p>
          </div>
        </div>

        {/* Error message */}
        {error && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-4">
            <div className="flex">
              <div className="flex-shrink-0">
                <span className="text-red-500 text-lg">⚠️</span>
              </div>
              <div className="ml-3">
                <p className="text-sm text-red-700">{error}</p>
                <button
                  onClick={() => window.location.reload()}
                  className="mt-2 text-sm text-red-600 hover:text-red-800 underline"
                >
                  Recargar página
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Cards de resumen */}
        <SummaryCards 
          statistics={statistics} 
          loading={statsLoading}
          owedDebts={owedDebts}
        />

        {/* Sistema de pestañas y tabla */}
        <div className="bg-white rounded-lg shadow">
          <div className="px-6 pt-6">
            <DebtTabs
              activeTab={activeTab}
              onTabChange={handleTabChange}
              lentCount={lentDebts.length}
              owedCount={owedDebts.length}
            />
          </div>

          <div className="p-6">
            {/* Filtros */}
            <FilterBar
              activeFilter={activeFilter}
              onFilterChange={handleFilterChange}
              debtCounts={debtCounts}
            />

            {/* Tabla de deudas */}
            <DebtTable
              debts={filteredDebts}
              loading={loading}
              type={activeTab}
              onViewDetail={handleViewDetail}
              onCreateDebt={handleCreateDebt}
            />
          </div>
        </div>

        {/* Información adicional */}
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <div className="flex">
            <div className="flex-shrink-0">
              <span className="text-blue-500 text-lg">💡</span>
            </div>
            <div className="ml-3">
              <h3 className="text-sm font-medium text-blue-800">
                ¿Cómo usar el sistema?
              </h3>
              <div className="mt-2 text-sm text-blue-700">
                <ul className="list-disc list-inside space-y-1">
                  <li>
                    <strong>"Dinero que Presté":</strong> Deudas que tú registraste donde alguien te debe dinero
                  </li>
                  <li>
                    <strong>"Dinero que Debo":</strong> Deudas que otros registraron donde tú les debes dinero
                  </li>
                  <li>Haz clic en cualquier fila para ver el detalle completo de la deuda</li>
                  <li>Usa los filtros para ver solo deudas pendientes o pagadas</li>
                  <li>Usa el botón + para crear una nueva deuda</li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Botón flotante para crear deuda - solo visible en pestaña "lent" */}
      {activeTab === 'lent' && (
        <FloatingActionButton
          onClick={handleCreateDebt}
          disabled={loading}
        />
      )}

      {/* Modal de detalle */}
      <DebtDetailModal
        isOpen={showDetailModal}
        debt={selectedDebt}
        type={activeTab}
        onClose={handleCloseDetailModal}
        onMarkAsPaid={handleMarkAsPaid}
      />

      {/* Modal de crear deuda */}
      <CreateDebtModal
        isOpen={showCreateModal}
        onClose={handleCloseCreateModal}
        onSuccess={handleDebtSuccess}
      />
    </Layout>
  );
};

export default DashboardPage;