// src/hooks/useRoleAuth.js
import { useAuth0 } from '@auth0/auth0-react';
import { useMemo } from 'react';

export const useRoleAuth = () => {
  const { user, isAuthenticated, isLoading } = useAuth0();
  const userRoles = useMemo(() => {
    if (!isAuthenticated || !user) return [];
    return user['https://ubmedia.com/roles'] || []; // Use the same namespace from your Auth0 Action
  }, [user, isAuthenticated]);

  const hasRole = (role) => userRoles.includes(role);
  const hasAnyRole = (roles) => roles.some((role) => userRoles.includes(role));
  const hasAllRoles = (roles) => roles.every((role) => userRoles.includes(role));

  return {
    userRoles,
    hasRole,
    hasAnyRole,
    hasAllRoles,
    isAuthenticated,
    isLoading,
    user
  };
};
