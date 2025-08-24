
// src/components/Dashboard/DebtDetailModal.tsx
import React, { useState } from 'react';
import { XMarkIcon, CalendarIcon, CurrencyDollarIcon, UserIcon, ClockIcon, CheckCircleIcon } from '@heroicons/react/24/outline';
import type { DebtDetailModalProps } from '../../types/DebtDetailModalProps';
import { debtService } from '../../services/api';

const DebtDetailModal: React.FC<DebtDetailModalProps> = ({ isOpen, debt, type, onClose, onMarkAsPaid }) => {
  const [isMarkingAsPaid, setIsMarkingAsPaid] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  if (!isOpen || !debt) return null;

  const handleMarkAsPaid = async () => {
    if (!debt || isMarkingAsPaid) return;
    
    setIsMarkingAsPaid(true);
    setError(null);
    
    try {
      await debtService.markAsPaid(debt.id);
      
      // Si hay callback, ejecutarlo
      if (onMarkAsPaid) {
        await onMarkAsPaid(debt.id);
      }
      
      // Cerrar el modal después de marcar como pagada
      onClose();
    } catch (error: any) {
      console.error('Error al marcar como pagada:', error);
      setError(error.response?.data?.message || 'Error al marcar la deuda como pagada');
    } finally {
      setIsMarkingAsPaid(false);
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0,
    }).format(amount);
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('es-CO', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  const formatTime = (date: string) => {
    return new Date(date).toLocaleTimeString('es-CO', {
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const getStatusInfo = () => {
    if (debt.isPaid) {
      return {
        status: 'Pagada',
        color: 'text-green-600 bg-green-100',
        icon: '✅'
      };
    }
    
    const now = new Date();
    const dueDate = debt.dueDate ? new Date(debt.dueDate) : null;
    
    if (dueDate && dueDate < now) {
      const daysOverdue = Math.floor((now.getTime() - dueDate.getTime()) / (1000 * 60 * 60 * 24));
      return {
        status: `Vencida (${daysOverdue} días)`,
        color: 'text-red-600 bg-red-100',
        icon: '🚨'
      };
    }
    
    if (dueDate) {
      const daysUntilDue = Math.floor((dueDate.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
      return {
        status: `Pendiente (vence en ${daysUntilDue} días)`,
        color: 'text-yellow-600 bg-yellow-100',
        icon: '⏳'
      };
    }
    
    return {
      status: 'Pendiente',
      color: 'text-yellow-600 bg-yellow-100',
      icon: '⏳'
    };
  };

  const statusInfo = getStatusInfo();

  return (
    <>
      {/* Overlay backdrop with blur effect */}
      <div 
        className="fixed inset-0 bg-white bg-opacity-20 backdrop-blur-sm z-40"
        onClick={onClose}
      />
      
      {/* Modal container */}
      <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
          <div className="p-6">
            {/* Error */}
            {error && (
              <div className="mb-6 bg-red-50 border border-red-200 rounded-lg p-4">
                <div className="flex">
                  <div className="flex-shrink-0">
                    <span className="text-red-500">❌</span>
                  </div>
                  <div className="ml-3">
                    <p className="text-sm text-red-700">{error}</p>
                  </div>
                </div>
              </div>
            )}
            
            {/* Header */}
            <div className="flex items-center justify-between mb-6">
              <div>
                <h3 className="text-lg font-medium text-gray-900">
                  Detalle de Deuda
                </h3>
                <p className="text-sm text-gray-500 mt-1">
                  {type === 'lent' ? 'Deuda que registraste' : 'Deuda que te registraron'}
                </p>
              </div>
              <button
                onClick={onClose}
                disabled={isMarkingAsPaid}
                className="text-gray-400 hover:text-gray-600 transition-colors disabled:cursor-not-allowed"
              >
                <XMarkIcon className="h-6 w-6" />
              </button>
            </div>

          {/* Contenido principal */}
          <div className="space-y-6">
            {/* Título y Estado */}
            <div className="border-b border-gray-200 pb-6">
              <h2 className="text-2xl font-bold text-gray-900 mb-3">
                {debt.title}
              </h2>
              
              <div className="flex items-center space-x-3">
                <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${statusInfo.color}`}>
                  <span className="mr-2">{statusInfo.icon}</span>
                  {statusInfo.status}
                </span>
                
                <div className="text-3xl font-bold text-blue-600">
                  {formatCurrency(debt.amount)}
                </div>
              </div>
            </div>

            {/* Descripción */}
            {debt.description && (
              <div>
                <h4 className="text-sm font-medium text-gray-900 mb-2">Descripción</h4>
                <p className="text-gray-700 bg-gray-50 p-4 rounded-lg">
                  {debt.description}
                </p>
              </div>
            )}

            {/* Información de personas */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="bg-blue-50 p-4 rounded-lg">
                <div className="flex items-center mb-3">
                  <UserIcon className="h-5 w-5 text-blue-600 mr-2" />
                  <h4 className="text-sm font-medium text-blue-900">
                    {type === 'lent' ? 'Deudor' : 'Acreedor'}
                  </h4>
                </div>
                <div className="text-lg font-semibold text-blue-900">
                  {type === 'lent' ? debt.debtorName : debt.debtorName}
                </div>
                <div className="text-sm text-blue-700">
                  {type === 'lent' ? debt.debtorEmail : debt.debtorEmail}
                </div>
              </div>

              <div className="bg-gray-50 p-4 rounded-lg">
                <div className="flex items-center mb-3">
                  <CurrencyDollarIcon className="h-5 w-5 text-gray-600 mr-2" />
                  <h4 className="text-sm font-medium text-gray-900">Detalles financieros</h4>
                </div>
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <span className="text-sm text-gray-600">Monto:</span>
                    <span className="text-sm font-medium">{formatCurrency(debt.amount)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-sm text-gray-600">Moneda:</span>
                    <span className="text-sm font-medium">{debt.currency}</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Información de fechas */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <div className="flex items-center mb-2">
                  <CalendarIcon className="h-5 w-5 text-gray-600 mr-2" />
                  <h4 className="text-sm font-medium text-gray-900">Fecha de creación</h4>
                </div>
                <div className="text-gray-700">
                  <div>{formatDate(debt.createdAt)}</div>
                  <div className="text-sm text-gray-500">
                    a las {formatTime(debt.createdAt)}
                  </div>
                </div>
              </div>

              {debt.dueDate && (
                <div>
                  <div className="flex items-center mb-2">
                    <ClockIcon className="h-5 w-5 text-gray-600 mr-2" />
                    <h4 className="text-sm font-medium text-gray-900">Fecha de vencimiento</h4>
                  </div>
                  <div className="text-gray-700">
                    <div>{formatDate(debt.dueDate)}</div>
                    {new Date(debt.dueDate) < new Date() && !debt.isPaid && (
                      <div className="text-sm text-red-600 font-medium">
                        ⚠️ Vencida
                      </div>
                    )}
                  </div>
                </div>
              )}
            </div>

            {/* Información adicional */}
            <div className="bg-gray-50 p-4 rounded-lg">
              <h4 className="text-sm font-medium text-gray-900 mb-3">Información adicional</h4>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="text-gray-600">ID de deuda:</span>
                  <div className="font-mono text-gray-900">#{debt.id}</div>
                </div>
                <div>
                  <span className="text-gray-600">Tipo:</span>
                  <div className="text-gray-900">
                    {type === 'lent' ? 'Dinero que presté' : 'Dinero que debo'}
                  </div>
                </div>
              </div>
            </div>
          </div>

            {/* Footer */}
            <div className="flex justify-between items-center mt-8 pt-6 border-t border-gray-200">
              <div>
                {/* Mostrar botón de marcar como pagada solo si no está pagada y es del tipo 'lent' */}
                {!debt.isPaid && type === 'lent' && (
                  <button
                    onClick={handleMarkAsPaid}
                    disabled={isMarkingAsPaid}
                    className="bg-green-600 text-white px-6 py-2 rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
                  >
                    {isMarkingAsPaid ? (
                      <>
                        <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                        </svg>
                        Marcando...
                      </>
                    ) : (
                      <>
                        <CheckCircleIcon className="h-4 w-4 mr-2" />
                        Marcar como Pagada
                      </>
                    )}
                  </button>
                )}
              </div>
              
              <button
                onClick={onClose}
                disabled={isMarkingAsPaid}
                className="bg-gray-600 text-white px-6 py-2 rounded-lg hover:bg-gray-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Cerrar
              </button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default DebtDetailModal;