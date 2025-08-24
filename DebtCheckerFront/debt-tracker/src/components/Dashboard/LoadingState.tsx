const LoadingState: React.FC = () => {
  return (
    <div className="overflow-hidden shadow ring-1 ring-black ring-opacity-5 md:rounded-lg">
      <table className="min-w-full divide-y divide-gray-300">
        <thead className="bg-gray-50">
          <tr>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Deuda
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Monto
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Persona
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Estado
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Fecha creación
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Vencimiento
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Acciones
            </th>
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {[1, 2, 3, 4, 5].map((i) => (
            <tr key={i}>
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="animate-pulse">
                  <div className="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
                  <div className="h-3 bg-gray-200 rounded w-1/2"></div>
                </div>
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="animate-pulse">
                  <div className="h-4 bg-gray-200 rounded w-20 mb-1"></div>
                  <div className="h-3 bg-gray-200 rounded w-12"></div>
                </div>
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="animate-pulse">
                  <div className="h-4 bg-gray-200 rounded w-24 mb-1"></div>
                  <div className="h-3 bg-gray-200 rounded w-32"></div>
                </div>
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="animate-pulse">
                  <div className="h-6 bg-gray-200 rounded-full w-20"></div>
                </div>
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="animate-pulse">
                  <div className="h-4 bg-gray-200 rounded w-24"></div>
                </div>
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="animate-pulse">
                  <div className="h-4 bg-gray-200 rounded w-20"></div>
                </div>
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <div className="animate-pulse">
                  <div className="h-5 w-5 bg-gray-200 rounded"></div>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default LoadingState;