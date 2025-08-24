import { UserIcon } from "lucide-react";
import Input from "../Common/Input";
import { MagnifyingGlassIcon } from "@heroicons/react/24/solid";
import type { CreateDebtRequest } from "../../types/CreateDebtRequest";
import type { DebtFormProps } from "../../types/DebtFormProps";
import { useEffect, useState } from "react";
import type { UserSearchResult } from "../../types/UserSearchResult";
import { debounce } from "../../utils/formatters";
import { userService } from "../../services/api";

const DebtForm: React.FC<DebtFormProps> = ({ debt, onSubmit, onCancel, loading }) => {
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    amount: '',
    currency: 'COP',
    debtorEmail: '',
    dueDate: ''
  });

  const [errors, setErrors] = useState<{[key: string]: string}>({});
  const [userSearch, setUserSearch] = useState({
    query: '',
    results: [] as UserSearchResult[],
    loading: false,
    showDropdown: false,
    selectedUser: null as UserSearchResult | null
  });

  // Inicializar formulario si estamos editando
  useEffect(() => {
    if (debt) {
      setFormData({
        title: debt.title,
        description: debt.description || '',
        amount: debt.amount.toString(),
        currency: debt.currency || 'COP',
        debtorEmail: debt.debtorEmail || '',
        dueDate: debt.dueDate ? debt.dueDate.split('T')[0] : ''
      });

      // Si tenemos datos del deudor, establecer usuario seleccionado
      if (debt.debtorEmail && debt.debtorName) {
        setUserSearch(prev => ({
          ...prev,
          query: debt.debtorEmail || '',
          selectedUser: {
            id: debt.debtorId || 0,
            email: debt.debtorEmail || '',
            name: debt.debtorName || ''
          }
        }));
      }
    }
  }, [debt]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    
    // Limpiar error cuando el usuario empieza a escribir
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  // Búsqueda de usuarios con debounce
  const searchUsers = debounce(async (query: string) => {
    if (query.length < 3) {
      setUserSearch(prev => ({ ...prev, results: [], showDropdown: false }));
      return;
    }

    setUserSearch(prev => ({ ...prev, loading: true }));

    try {
      const response = await userService.searchUsers(query);
      console.log('Respuesta de searchUsers:', response.data);
      // La API devuelve directamente un array de usuarios
      if (response.data && Array.isArray(response.data)) {
        console.log('Datos de usuarios:', response.data);
        // Convertir User[] a UserSearchResult[]
        const userSearchResults = response.data.map(user => ({
          id: user.id,
          email: user.email,
          name: user.fullName
        }));
        console.log('UserSearchResults mapeados:', userSearchResults);
        
        setUserSearch(prev => ({
          ...prev,
          results: userSearchResults,
          loading: false,
          showDropdown: true
        }));
      } else {
        // Si es un solo usuario, convertirlo a array
        if (response.data && typeof response.data === 'object' && 'id' in response.data) {
          const singleUser = response.data as any;
          const userSearchResults = [{
            id: singleUser.id,
            email: singleUser.email,
            name: singleUser.fullName
          }];
          console.log('Usuario único mapeado:', userSearchResults);
          
          setUserSearch(prev => ({
            ...prev,
            results: userSearchResults,
            loading: false,
            showDropdown: true
          }));
        }
      }
    } catch (error) {
      console.error('Error buscando usuarios:', error);
      setUserSearch(prev => ({
        ...prev,
        results: [],
        loading: false,
        showDropdown: false
      }));
    }
  }, 500);

  const handleUserSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    const query = e.target.value;
    setUserSearch(prev => ({ ...prev, query, selectedUser: null }));
    setFormData(prev => ({ ...prev, debtorEmail: query }));
    
    if (errors.debtorEmail) {
      setErrors(prev => ({ ...prev, debtorEmail: '' }));
    }

    searchUsers(query);
  };

  const handleUserSelect = (user: UserSearchResult) => {
    setUserSearch(prev => ({
      ...prev,
      query: user.email,
      selectedUser: user,
      showDropdown: false
    }));
    setFormData(prev => ({ ...prev, debtorEmail: user.email }));
  };

  const validateForm = () => {
    const newErrors: {[key: string]: string} = {};

    if (!formData.title.trim()) {
      newErrors.title = 'El título es requerido';
    }

    if (!formData.amount) {
      newErrors.amount = 'El monto es requerido';
    } else {
      const amount = parseFloat(formData.amount);
      if (isNaN(amount) || amount <= 0) {
        newErrors.amount = 'El monto debe ser un número positivo';
      }
    }

    // El email del deudor es opcional, pero si se proporciona debe ser válido
    if (formData.debtorEmail.trim() && !/\S+@\S+\.\S+/.test(formData.debtorEmail)) {
      newErrors.debtorEmail = 'El email no es válido';
    }

    // Validar fecha de vencimiento si se proporciona
    if (formData.dueDate) {
      const dueDate = new Date(formData.dueDate);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      
      if (dueDate < today) {
        newErrors.dueDate = 'La fecha de vencimiento no puede ser en el pasado';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    try {
      let debtorId: number | undefined = undefined;
      
      // Solo buscar usuario si se proporcionó un email
      if (formData.debtorEmail.trim()) {
        console.log('Buscando usuario con email:', formData.debtorEmail);
        
        // Si tenemos un usuario seleccionado de la búsqueda, usar ese
        if (userSearch.selectedUser) {
          debtorId = userSearch.selectedUser.id;
          console.log('Usando usuario seleccionado:', debtorId);
        } else {
          // Si ya tenemos resultados de búsqueda y hay un match exacto, usar ese
          const existingMatch = userSearch.results.find(
            user => user.email.toLowerCase() === formData.debtorEmail.toLowerCase()
          );
          
          if (existingMatch) {
            console.log('Usando match existente:', existingMatch);
            debtorId = existingMatch.id;
          } else {
            // Hacer nueva búsqueda
            try {
              const userSearchResponse = await userService.searchUsers(formData.debtorEmail);
              console.log('Respuesta de búsqueda:', userSearchResponse.data);
              
              // La API devuelve directamente un array de usuarios o un usuario único
              let users: any[] = [];
              if (Array.isArray(userSearchResponse.data)) {
                users = userSearchResponse.data;
              } else if (userSearchResponse.data && typeof userSearchResponse.data === 'object' && 'id' in userSearchResponse.data) {
                users = [userSearchResponse.data];
              }
              
              if (users.length > 0) {
                console.log('Usuarios encontrados:', users);
                // Buscar usuario con email exacto
                const exactMatch = users.find(
                  (user: any) => user.email.toLowerCase() === formData.debtorEmail.toLowerCase()
                );
                console.log('Match exacto:', exactMatch);
                
                if (exactMatch) {
                  debtorId = exactMatch.id;
                  console.log('DebtorId encontrado:', debtorId);
                } else {
                  // Usuario no encontrado, mostrar warning pero continuar sin debtorId
                  console.log('No se encontró usuario exacto, continuando sin debtorId');
                  setErrors(prev => ({ 
                    ...prev, 
                    debtorEmail: 'Usuario no encontrado. La deuda se creará sin asignar deudor específico.' 
                  }));
                  // No hacer return, continuar con la creación
                }
              } else {
                // No se encontraron usuarios, mostrar warning pero continuar sin debtorId
                console.log('No se encontraron usuarios, continuando sin debtorId');
                setErrors(prev => ({ 
                  ...prev, 
                  debtorEmail: 'Usuario no encontrado. La deuda se creará sin asignar deudor específico.' 
                }));
                // No hacer return, continuar con la creación
              }
            } catch (searchError) {
              // Error en la búsqueda, mostrar warning pero continuar sin debtorId
              console.log('Error en búsqueda de usuario, continuando sin debtorId:', searchError);
              setErrors(prev => ({ 
                ...prev, 
                debtorEmail: 'No se pudo verificar el usuario. La deuda se creará sin asignar deudor específico.' 
              }));
              // No hacer return, continuar con la creación
            }
          }
        }
      }

      const submitData: CreateDebtRequest = {
        title: formData.title.trim(),
        description: formData.description.trim() || undefined,
        amount: parseFloat(formData.amount),
        currency: formData.currency,
        dueDate: formData.dueDate ? new Date(formData.dueDate).toISOString() : undefined
      };

      // Solo agregar debtorId si tenemos uno válido
      if (debtorId) {
        submitData.debtorId = debtorId;
      }

      console.log('Datos a enviar:', submitData);
      await onSubmit(submitData);
    } catch (error: any) {
      // Los errores se manejan en el componente padre
      console.error('Error al procesar el formulario:', error);
    }
  };

  const handleCancel = () => {
    setUserSearch(prev => ({ ...prev, showDropdown: false }));
    onCancel();
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      {/* Título */}
      <div>
        <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-2">
          Título *
        </label>
        <Input
          id="title"
          name="title"
          type="text"
          value={formData.title}
          onChange={handleInputChange}
          placeholder="Ej: Préstamo para almuerzo"
          error={errors.title}
          disabled={loading}
        />
      </div>

      {/* Descripción */}
      <div>
        <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-2">
          Descripción (opcional)
        </label>
        <textarea
          id="description"
          name="description"
          rows={3}
          value={formData.description}
          onChange={handleInputChange}
          placeholder="Detalles adicionales sobre la deuda..."
          disabled={loading}
          className={`w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
            loading ? 'bg-gray-50 text-gray-500' : ''
          }`}
        />
      </div>

      {/* Monto y Moneda */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div className="sm:col-span-2">
          <label htmlFor="amount" className="block text-sm font-medium text-gray-700 mb-2">
            Monto *
          </label>
          <Input
            id="amount"
            name="amount"
            type="number"
            step="0.01"
            min="0"
            value={formData.amount}
            onChange={handleInputChange}
            placeholder="0.00"
            error={errors.amount}
            disabled={loading}
          />
        </div>
        
        <div>
          <label htmlFor="currency" className="block text-sm font-medium text-gray-700 mb-2">
            Moneda
          </label>
          <select
            id="currency"
            name="currency"
            value={formData.currency}
            onChange={handleInputChange}
            disabled={loading}
            className={`w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
              loading ? 'bg-gray-50 text-gray-500' : ''
            }`}
          >
            <option value="COP">COP</option>
            <option value="USD">USD</option>
            <option value="EUR">EUR</option>
          </select>
        </div>
      </div>

      {/* Búsqueda de deudor */}
      <div className="relative">
        <label htmlFor="debtorEmail" className="block text-sm font-medium text-gray-700 mb-2">
          Email del deudor (opcional)
        </label>
        <p className="text-sm text-gray-500 mb-3">
          Puedes buscar a tu amigo que esté registrado por medio de su correo. Si está registrado te aparecerá en la lista para seleccionarlo.
        </p>
        <div className="relative">
          <input
            id="debtorEmail"
            name="debtorEmail"
            type="email"
            value={userSearch.query}
            onChange={handleUserSearch}
            placeholder="email@ejemplo.com (opcional)"
            disabled={loading}
            className={`w-full pl-10 pr-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors ${
              errors.debtorEmail 
                ? 'border-red-300 text-red-900 focus:ring-red-500 focus:border-red-500' 
                : 'border-gray-300'
            } ${loading ? 'bg-gray-50 text-gray-500' : ''}`}
          />
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            {userSearch.loading ? (
              <div className="animate-spin rounded-full h-4 w-4 border-2 border-blue-500 border-t-transparent" />
            ) : (
              <MagnifyingGlassIcon className="h-4 w-4 text-gray-400" />
            )}
          </div>
        </div>
        {errors.debtorEmail && (
          <p className="mt-1 text-sm text-red-600">{errors.debtorEmail}</p>
        )}

        {/* Dropdown de resultados de búsqueda */}
        {userSearch.showDropdown && userSearch.results.length > 0 && (
          <div className="absolute z-10 w-full mt-1 bg-white border border-gray-200 rounded-lg shadow-lg max-h-60 overflow-y-auto">
            {userSearch.results.map((user) => (
              <button
                key={user.id}
                type="button"
                onClick={() => handleUserSelect(user)}
                className="w-full px-4 py-3 text-left hover:bg-gray-50 flex items-center space-x-3 first:rounded-t-lg last:rounded-b-lg"
              >
                <UserIcon className="h-5 w-5 text-gray-400" />
                <div>
                  <div className="font-medium text-gray-900">{user.name}</div>
                  <div className="text-sm text-gray-500">{user.email}</div>
                </div>
              </button>
            ))}
          </div>
        )}

        {/* Usuario seleccionado */}
        {userSearch.selectedUser && (
          <div className="mt-2 p-3 bg-green-50 border border-green-200 rounded-lg flex items-center space-x-2">
            <UserIcon className="h-5 w-5 text-green-600" />
            <div className="text-sm">
              <span className="font-medium text-green-800">{userSearch.selectedUser.name}</span>
              <span className="text-green-600 ml-1">seleccionado</span>
            </div>
          </div>
        )}
      </div>

      {/* Fecha de vencimiento */}
      <div>
        <label htmlFor="dueDate" className="block text-sm font-medium text-gray-700 mb-2">
          Fecha de vencimiento (opcional)
        </label>
        <Input
          id="dueDate"
          name="dueDate"
          type="date"
          value={formData.dueDate}
          onChange={handleInputChange}
          error={errors.dueDate}
          disabled={loading}
          min={new Date().toISOString().split('T')[0]}
        />
      </div>

      {/* Botones */}
      <div className="flex justify-end space-x-3 pt-6 border-t border-gray-200">
        <button
          type="button"
          onClick={handleCancel}
          disabled={loading}
          className="px-4 py-2 text-sm font-medium text-gray-900 bg-gray-200 border border-gray-300 rounded-lg hover:bg-gray-300 focus:ring-2 focus:ring-gray-500 focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Cancelar
        </button>
        
        <button
          type="submit"
          disabled={loading}
          className="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-lg hover:bg-blue-700 focus:ring-2 focus:ring-blue-500 focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
        >
          {loading ? (
            <>
              <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Creando...
            </>
          ) : (
            debt ? 'Actualizar Deuda' : 'Crear Deuda'
          )}
        </button>
      </div>
    </form>
  );
};

export default DebtForm;