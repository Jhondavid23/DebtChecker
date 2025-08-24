import { useCallback, useEffect, useState } from "react";
import type { UseDebtsReturn } from "../types/UseDebtReturn";
import { debtService } from '../services/api';
import type { DebtStatistics } from "../types/DebtStatistics";
import type { Debt } from "../types/Debt";

export const useDebts = (): UseDebtsReturn => {
  const [lentDebts, setLentDebts] = useState<Debt[]>([]);
  const [owedDebts, setOwedDebts] = useState<Debt[]>([]);
  const [statistics, setStatistics] = useState<DebtStatistics | null>(null);
  const [loading, setLoading] = useState(true);
  const [statsLoading, setStatsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Cargar deudas que presté
  const loadLentDebts = async () => {
    try {
      const response = await debtService.getMyDebts();
      if (response.data.success && response.data.data) {
        setLentDebts(response.data.data.items);
      }
    } catch (error: any) {
      console.error('Error cargando deudas prestadas:', error);
      setError('Error al cargar las deudas prestadas');
    }
  };

  // Cargar deudas que debo
  const loadOwedDebts = async () => {
    try {
      const response = await debtService.getDebtsIOwe();
      if (response.data.success && response.data.data) {
        setOwedDebts(response.data.data.items);
      }
    } catch (error: any) {
      console.error('Error cargando deudas que debo:', error);
      setError('Error al cargar las deudas que debo');
    }
  };

  // Cargar estadísticas
  const loadStatistics = async () => {
    try {
      setStatsLoading(true);
      const response = await debtService.getStatistics();
      if (response.data.success && response.data.data) {
        setStatistics(response.data.data);
      }
    } catch (error: any) {
      console.error('Error cargando estadísticas:', error);
      setError('Error al cargar las estadísticas');
    } finally {
      setStatsLoading(false);
    }
  };

  // Cargar todas las deudas
  const refreshDebts = useCallback(async () => {
    setLoading(true);
    setError(null);
    
    try {
      await Promise.all([
        loadLentDebts(),
        loadOwedDebts()
      ]);
    } catch (error) {
      // Los errores ya se manejan en las funciones individuales
    } finally {
      setLoading(false);
    }
  }, []);

  // Refrescar estadísticas
  const refreshStats = useCallback(async () => {
    await loadStatistics();
  }, []);

  // Marcar como pagada
  const markAsPaid = useCallback(async (debtId: number) => {
    try {
      await debtService.markAsPaid(debtId);
      
      // Actualizar estado local - optimistic update
      setLentDebts(prev => 
        prev.map(debt => 
          debt.id === debtId 
            ? { ...debt, isPaid: true }
            : debt
        )
      );

      // Refrescar estadísticas
      await refreshStats();
    } catch (error: any) {
      console.error('Error marcando deuda como pagada:', error);
      setError('Error al marcar la deuda como pagada');
      // Refrescar para revertir el cambio optimista
      await refreshDebts();
    }
  }, [refreshStats, refreshDebts]);

  // Eliminar deuda
  const deleteDebt = useCallback(async (debtId: number) => {
    try {
      await debtService.deleteDebt(debtId);
      
      // Actualizar estado local
      setLentDebts(prev => prev.filter(debt => debt.id !== debtId));
      
      // Refrescar estadísticas
      await refreshStats();
    } catch (error: any) {
      console.error('Error eliminando deuda:', error);
      setError('Error al eliminar la deuda');
      // Refrescar para revertir el cambio
      await refreshDebts();
    }
  }, [refreshStats, refreshDebts]);

  // Efecto inicial
  useEffect(() => {
    const initialize = async () => {
      await Promise.all([
        refreshDebts(),
        refreshStats()
      ]);
    };

    initialize();
  }, []);

  return {
    lentDebts,
    owedDebts,
    statistics,
    loading,
    statsLoading,
    error,
    refreshDebts,
    refreshStats,
    markAsPaid,
    deleteDebt
  };
};