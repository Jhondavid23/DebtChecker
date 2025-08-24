import { PlusIcon } from "lucide-react";
import type { FloatingActionButtonProps } from "../../types/FloatingActionButtonProps";

const FloatingActionButton: React.FC<FloatingActionButtonProps> = ({ onClick, disabled = false }) => {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      className={`fixed bottom-6 right-6 z-40 bg-blue-600 text-white rounded-full p-4 shadow-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all duration-200 ${
        disabled ? 'opacity-50 cursor-not-allowed' : 'hover:scale-105 active:scale-95'
      }`}
      title="Crear nueva deuda"
    >
      <PlusIcon className="h-6 w-6" />
    </button>
  );
};

export default FloatingActionButton;