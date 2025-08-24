
interface EmptyStateProps {
  type: 'lent' | 'owed';
  onCreateDebt?: () => void;
}
const EmptyState: React.FC<EmptyStateProps> = ({ type, onCreateDebt }) => {
  const content = type === 'lent' 
    ? {
        icon: '🤝',
        title: 'No tienes deudas prestadas',
        description: 'Cuando crees una deuda donde alguien te debe dinero, aparecerá aquí.',
        action: 'Crear primera deuda'
      }
    : {
        icon: '💳',
        title: 'No debes dinero',
        description: 'Las deudas que otros registren contigo aparecerán aquí.',
        action: null
      };

  return (
    <div className="text-center py-12">
      <div className="text-6xl mb-4">{content.icon}</div>
      <h3 className="text-lg font-medium text-gray-900 mb-2">
        {content.title}
      </h3>
      <p className="text-gray-500 mb-6 max-w-sm mx-auto">
        {content.description}
      </p>
      
      {content.action && type === 'lent' && onCreateDebt && (
        <button 
          onClick={onCreateDebt}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          {content.action}
        </button>
      )}
    </div>
  );
};

export default EmptyState;