import React from 'react';
import { useAuth } from '../../context/AuthContext';
import type { HeaderProps } from '../../types/HeaderProps';
import { Bars3Icon, XMarkIcon } from '@heroicons/react/24/outline';

const Header: React.FC<HeaderProps> = ({ onMenuToggle, isMobileMenuOpen }) => {
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
  };

  return (
    <header className="bg-white shadow-sm border-b border-gray-200">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="flex h-16 justify-between items-center">
          {/* Logo y título */}
          <div className="flex items-center">
            <div className="flex-shrink-0 flex items-center">
              <span className="text-2xl">💰</span>
              <h1 className="ml-2 text-xl font-bold text-gray-900">
                DebtTracker
              </h1>
            </div>
          </div>

          {/* Desktop navigation y usuario */}
          <div className="hidden md:flex md:items-center md:space-x-4">
            <div className="flex items-center space-x-3">
              <div className="text-sm text-gray-700">
                Hola, <span className="font-medium">{user?.firstName}</span>
              </div>
              <button
                onClick={handleLogout}
                className="text-sm text-gray-500 hover:text-gray-700 transition-colors"
              >
                Cerrar sesión
              </button>
            </div>
          </div>

          {/* Mobile menu button */}
          <div className="md:hidden">
            <button
              onClick={onMenuToggle}
              className="text-gray-400 hover:text-gray-600 hover:bg-gray-100 p-2 rounded-md"
            >
              {isMobileMenuOpen ? (
                <XMarkIcon className="h-6 w-6" />
              ) : (
                <Bars3Icon className="h-6 w-6" />
              )}
            </button>
          </div>
        </div>
      </div>

      {/* Mobile menu */}
      {isMobileMenuOpen && (
        <div className="md:hidden">
          <div className="px-2 pt-2 pb-3 space-y-1 sm:px-3 bg-gray-50 border-t border-gray-200">
            <div className="px-3 py-2 text-sm text-gray-700">
              Hola, <span className="font-medium">{user?.firstName}</span>
            </div>
            <button
              onClick={handleLogout}
              className="block w-full text-left px-3 py-2 text-sm text-gray-500 hover:text-gray-700"
            >
              Cerrar sesión
            </button>
          </div>
        </div>
      )}
    </header>
  );
};

export default Header;