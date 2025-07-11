import { useRoleAuth } from '../hooks/useRoleAuth';

const RoleGuard = ({ roles = [], requireAll = false, fallback = null, children }) => {
  const { hasAnyRole, hasAllRoles, isAuthenticated } = useRoleAuth();
  if (!isAuthenticated) return fallback;
  if (roles.length === 0) return children;

  const permitted = requireAll ? hasAllRoles(roles) : hasAnyRole(roles);
  return permitted ? children : fallback;
};

export default RoleGuard;
