import { useAuth0 } from '@auth0/auth0-react';
import { Navigate } from 'react-router-dom';

const AuthRouteGuard = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth0();
  if (isLoading) return <p>Loading...</p>;
  if (!isAuthenticated) return <Navigate to="/unauthorized-access" replace />;
  return children;
};

export default AuthRouteGuard;
