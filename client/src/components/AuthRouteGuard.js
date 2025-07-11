import { useAuth0 } from '@auth0/auth0-react';
import { Navigate } from 'react-router-dom';
import { useRoleAuth } from '../hooks/useRoleAuth';
import LoadingSpinner from './LoadingSpinner';

const AuthRouteGuard = ({ children, roles = [], requireAll = false, fallback = null }) => {
  const { isAuthenticated, isLoading } = useAuth0();
  const { hasAnyRole, hasAllRoles } = useRoleAuth();

  if (isLoading) return <LoadingSpinner/>;
  if (!isAuthenticated) return <Navigate to="/access-denied" replace />;

  if (roles.length > 0) {
    const permitted = requireAll ? hasAllRoles(roles) : hasAnyRole(roles);
    if (!permitted) return fallback || <Navigate to="/access-denied" replace />;
  }

  return children;
};

export default AuthRouteGuard;
