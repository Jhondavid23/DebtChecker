import { XMarkIcon } from "@heroicons/react/24/solid";
import DebtForm from "./DebtForm";
import { useState } from "react";
import type { CreateDebtModalProps } from "../../types/CreateDebtModalProps";
import type { CreateDebtRequest } from "../../types/CreateDebtRequest";
import { debtService } from "../../services/api";

const CreateDebtModal: React.FC<CreateDebtModalProps> = ({ isOpen, onClose, onSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (data: CreateDebtRequest) => {
    setLoading(true);
    setError(null);

    try {
      const response = await debtService.createDebt(data);
      
      if (response.data.success) {
        onSuccess();
        onClose();
        // TODO: Mostrar notificación de éxito
      } else {
        setError(response.data.message || 'Error al crear la deuda');
      }
    } catch (error: any) {
      const message = error.response?.data?.message || 'Error al crear la deuda';
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

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-[9999] overflow-y-auto">
      {/* Overlay */}
      <div 
        className="fixed inset-0 bg-white bg-opacity-20 backdrop-blur-sm"
        onClick={handleClose}
      />
      
      {/* Modal Container */}
      <div className="flex items-center justify-center min-h-full p-4">
        {/* Modal */}
        <div className="relative w-full max-w-lg bg-white rounded-lg shadow-xl transform transition-all" onClick={(e) => e.stopPropagation()}>
          {/* Header */}
          <div className="flex items-center justify-between p-6 border-b border-gray-200">
            <h3 className="text-lg font-medium text-gray-900">
              Crear Nueva Deuda
            </h3>
            <button
              onClick={handleClose}
              disabled={loading}
              className="text-gray-400 hover:text-gray-600 transition-colors disabled:cursor-not-allowed"
            >
              <XMarkIcon className="h-6 w-6" />
            </button>
          </div>

          {/* Content */}
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

            {/* Form */}
            <DebtForm
              onSubmit={handleSubmit}
              onCancel={handleClose}
              loading={loading}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default CreateDebtModal;