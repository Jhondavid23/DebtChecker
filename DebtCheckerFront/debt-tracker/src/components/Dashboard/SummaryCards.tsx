import type { SummaryCardsProps } from "../../types/SummaryCardsProps";

const SummaryCards: React.FC<SummaryCardsProps> = ({ statistics, loading, owedDebts = [] }) => {
  if (loading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        {[1, 2, 3].map((i) => (
          <div key={i} className="bg-white rounded-lg shadow p-6">
            <div className="animate-pulse">
              <div className="h-4 bg-gray-200 rounded w-1/2 mb-2"></div>
              <div className="h-8 bg-gray-200 rounded w-3/4 mb-1"></div>
              <div className="h-3 bg-gray-200 rounded w-1/3"></div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0,
    }).format(amount);
  };

  // Calcular el total de dinero que debo basándose en owedDebts
  const totalOwedAmount = owedDebts.reduce((total, debt) => {
    return total + (debt.isPaid ? 0 : debt.amount);
  }, 0);

  const totalOwedCount = owedDebts.filter(debt => !debt.isPaid).length;

  const balanceNet = (statistics?.totalAmount || 0) - totalOwedAmount;
  const balanceColor = balanceNet >= 0 ? 'text-green-600' : 'text-red-600';
  const balanceIcon = balanceNet >= 0 ? '📈' : '📉';

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
      {/* Dinero que Presto */}
      <div className="bg-white rounded-lg shadow hover:shadow-md transition-shadow p-6">
        <div className="flex items-center">
          <div className="flex-shrink-0">
            <span className="text-2xl">🤝</span>
          </div>
          <div className="ml-4">
            <p className="text-sm font-medium text-gray-600">
              Dinero que Presto
            </p>
            <p className="text-2xl font-bold text-blue-600">
              {formatCurrency(statistics?.totalAmount || 0)}
            </p>
            <p className="text-sm text-gray-500">
              {statistics?.totalAmount || 0} deudas
            </p>
          </div>
        </div>
      </div>

      {/* Dinero que Debo */}
      <div className="bg-white rounded-lg shadow hover:shadow-md transition-shadow p-6">
        <div className="flex items-center">
          <div className="flex-shrink-0">
            <span className="text-2xl">💳</span>
          </div>
          <div className="ml-4">
            <p className="text-sm font-medium text-gray-600">
              Dinero que Debo
            </p>
            <p className="text-2xl font-bold text-orange-600">
              {formatCurrency(totalOwedAmount)}
            </p>
            <p className="text-sm text-gray-500">
              {totalOwedCount} deudas
            </p>
          </div>
        </div>
      </div>

      {/* Balance Neto */}
      <div className="bg-white rounded-lg shadow hover:shadow-md transition-shadow p-6">
        <div className="flex items-center">
          <div className="flex-shrink-0">
            <span className="text-2xl">{balanceIcon}</span>
          </div>
          <div className="ml-4">
            <p className="text-sm font-medium text-gray-600">
              Balance Neto
            </p>
            <p className={`text-2xl font-bold ${balanceColor}`}>
              {formatCurrency(Math.abs(balanceNet))}
            </p>
            <p className="text-sm text-gray-500">
              {balanceNet >= 0 ? 'A favor' : 'En deuda'}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SummaryCards;