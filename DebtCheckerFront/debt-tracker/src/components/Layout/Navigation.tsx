import React from 'react';
import { NavLink } from 'react-router-dom';
import { HomeIcon, ChartBarIcon } from '@heroicons/react/24/outline';

const Navigation: React.FC = () => {
  const navItems = [
    {
      name: 'Dashboard',
      href: '/dashboard',
      icon: HomeIcon,
      enabled: true,
    },
    {
      name: 'Reportes',
      href: '/reports',
      icon: ChartBarIcon,
      enabled: false,
    },
  ];

  return (
    <nav className="bg-gray-50 border-b border-gray-200">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="flex space-x-8">
          {navItems.map((item) => {
            if (!item.enabled) {
              return (
                <div
                  key={item.name}
                  className="flex items-center px-1 pt-4 pb-4 text-sm font-medium border-b-2 border-transparent text-gray-400 cursor-not-allowed"
                >
                  <item.icon className="h-5 w-5 mr-2" />
                  {item.name}
                </div>
              );
            }

            return (
              <NavLink
                key={item.name}
                to={item.href}
                className={({ isActive }) =>
                  `flex items-center px-1 pt-4 pb-4 text-sm font-medium border-b-2 transition-colors ${
                    isActive
                      ? 'border-blue-500 text-blue-600'
                      : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  }`
                }
              >
                <item.icon className="h-5 w-5 mr-2" />
                {item.name}
              </NavLink>
            );
          })}
        </div>
      </div>
    </nav>
  );
};

export default Navigation;