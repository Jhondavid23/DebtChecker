export const formatCurrency = (amount: number, currency: string = 'COP'): string => {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: currency,
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  }).format(amount);
};

export const formatDate = (date: string | Date, format: 'short' | 'long' | 'numeric' = 'short'): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date;
  
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: format === 'numeric' ? '2-digit' : format === 'short' ? 'short' : 'long',
    day: 'numeric'
  };

  return dateObj.toLocaleDateString('es-CO', options);
};

export const formatRelativeTime = (date: string | Date): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date;
  const now = new Date();
  const diffInMs = dateObj.getTime() - now.getTime();
  const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

  if (diffInDays === 0) {
    return 'Hoy';
  } else if (diffInDays === 1) {
    return 'Mañana';
  } else if (diffInDays === -1) {
    return 'Ayer';
  } else if (diffInDays > 0) {
    return `En ${diffInDays} días`;
  } else {
    return `Hace ${Math.abs(diffInDays)} días`;
  }
};

export const getDebtStatus = (debt: { isPaid: boolean; dueDate?: string | null }) => {
  if (debt.isPaid) {
    return {
      status: 'paid',
      label: 'Pagada',
      color: 'bg-green-100 text-green-800',
      borderColor: 'border-green-500'
    };
  }

  if (debt.dueDate) {
    const dueDate = new Date(debt.dueDate);
    const now = new Date();
    const diffInMs = dueDate.getTime() - now.getTime();
    const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

    if (diffInDays < 0) {
      return {
        status: 'overdue',
        label: 'Vencida',
        color: 'bg-red-100 text-red-800',
        borderColor: 'border-red-500'
      };
    } else if (diffInDays <= 7) {
      return {
        status: 'due-soon',
        label: 'Próxima a vencer',
        color: 'bg-orange-100 text-orange-800',
        borderColor: 'border-orange-500'
      };
    }
  }

  return {
    status: 'pending',
    label: 'Pendiente',
    color: 'bg-yellow-100 text-yellow-800',
    borderColor: 'border-yellow-500'
  };
};

export const calculateBalance = (lentTotal: number, owedTotal: number) => {
  const balance = lentTotal - owedTotal;
  
  return {
    amount: Math.abs(balance),
    isPositive: balance >= 0,
    label: balance >= 0 ? 'A favor' : 'En deuda',
    color: balance >= 0 ? 'text-green-600' : 'text-red-600',
    icon: balance >= 0 ? '📈' : '📉'
  };
};

export const debounce = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: number | undefined;
  
  return (...args: Parameters<T>) => {
    if (timeout) clearTimeout(timeout);
    timeout = window.setTimeout(() => func(...args), wait);
  };
};