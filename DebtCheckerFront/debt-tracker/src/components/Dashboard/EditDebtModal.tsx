import { XMarkIcon } from "@heroicons/react/24/solid";
import DebtForm from "./DebtForm";
import type { CreateDebtRequest } from "../../types/CreateDebtRequest";
import { useEffect, useState } from "react";
import type { EditDebtModalProps } from "../../types/EditDebtModalProps";
import { debtService } from "../../services/api";

const EditDebtModal: React.FC<EditDebtModalProps> = ({ isOpen, debt, onClose, onSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Limpiar error cuando cambia la deuda
  useEffect(() => {
    setError(null);
  }, [debt]);

  const handleSubmit = async (data: CreateDebtRequest) => {
    if (!debt) return;

    setLoading(true);
    setError(null);

    try {
      const response = await debtService.updateDebt(debt.id, data);
      
      if (response.data.success) {
        onSuccess();
        onClose();
        // TODO: Mostrar notificación de éxito
      } else {
        setError(response.data.message || 'Error al actualizar la deuda');
      }
    } catch (error: any) {
      const message = error.response?.data?.message || 'Error al actualizar la deuda';
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    if (!loading) {
      setError(null);
      onClose();
    }
  };

  if (!isOpen || !debt) return null;

  // No permitir editar deudas pagadas
  if (debt.isPaid) {
    return (
      <div className="fixed inset-0 z-50 overflow-y-auto">
        <div className="flex items-center justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
          <div 
            className="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"
            onClick={handleClose}
          />
          
          <div className="inline-block align-bottom bg-white rounded-lg px-4 pt-5 pb-4 text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-sm sm:w-full sm:p-6">
            <div className="text-center">
              <div className="text-6xl mb-4">🔒</div>
              <h3 className="text-lg font-medium text-gray-900 mb-2">
                Deuda no editable
              </h3>
              <p className="text-gray-500 mb-6">
                Las deudas marcadas como pagadas no pueden ser modificadas.
              </p>
              <button
                onClick={handleClose}
                className="bg-gray-600 text-white px-4 py-2 rounded-lg hover:bg-gray-700 transition-colors"
              >
                Entendido
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        {/* Overlay */}
        <div 
          className="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"
          onClick={handleClose}
        />

        {/* Modal */}
        <div className="inline-block align-bottom bg-white rounded-lg px-4 pt-5 pb-4 text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full sm:p-6">
          {/* Header */}
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-lg font-medium text-gray-900">
              Editar Deuda
            </h3>
            <button
              onClick={handleClose}
              disabled={loading}
              className="text-gray-400 hover:text-gray-600 transition-colors disabled:cursor-not-allowed"
            >
              <XMarkIcon className="h-6 w-6" />
            </button>
          </div>

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

          {/* Form */}
          <DebtForm
            debt={debt}
            onSubmit={handleSubmit}
            onCancel={handleClose}
            loading={loading}
          />
        </div>
      </div>
    </div>
  );
};

export default EditDebtModal;