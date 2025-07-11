import { useAuth0 } from '@auth0/auth0-react';
import { Link } from 'react-router-dom';
import { useRoleAuth } from '../hooks/useRoleAuth';
import RoleGuard from './RoleGuard';

const Navbar = () => {
  const { isAuthenticated, isLoading, loginWithRedirect, logout } = useAuth0();
  const { user, userRoles } = useRoleAuth();

  return (
    <nav style={styles.nav}>
      <div>
        <Link to="/" style={styles.link}>Home</Link>
        <Link to="/UBMediaUpdates" style={styles.link}>Updates</Link>

        {isAuthenticated && (
          <Link to="/UBMOperations" style={styles.link}>Operations</Link>
        )}

        <RoleGuard roles={['admin']}>
          <Link to="/admin" style={styles.link}>Admin Panel</Link>
        </RoleGuard>

        <RoleGuard roles={['manager', 'admin']}>
          <Link to="/management" style={styles.link}>Management</Link>
        </RoleGuard>

        <RoleGuard roles={['hr']}>
          <Link to="/employees" style={styles.link}>Employee Management</Link>
        </RoleGuard>
      </div>

      <div style={styles.authBlock}>
        {!isLoading && isAuthenticated && (
          <div>
            <span style={styles.user}>{user?.name || user?.email}</span>
            {userRoles.length > 0 && (
              <span style={{ fontSize: '0.8rem', color: '#777' }}>
                ({userRoles.join(', ')})
              </span>
            )}
          </div>
        )}

        {!isAuthenticated ? (
          <button style={styles.button} onClick={() => loginWithRedirect()}>Login</button>
        ) : (
          <button
            style={styles.button}
            onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}
          >
            Logout
          </button>
        )}
      </div>
    </nav>
  );
};

const styles = {
  nav: {
    padding: '12px 24px',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#f2f2f2',
    borderBottom: '1px solid #ccc',
  },
  link: {
    marginRight: 15,
    textDecoration: 'none',
    color: '#333',
    fontWeight: 'bold',
  },
  button: {
    marginLeft: 10,
    padding: '6px 12px',
    fontSize: '1rem',
    cursor: 'pointer',
    backgroundColor: '#007bff',
    color: '#fff',
    border: 'none',
    borderRadius: 5,
  },
  authBlock: {
    display: 'flex',
    alignItems: 'center',
  },
  user: {
    fontWeight: 'bold',
    marginRight: 10,
    color: '#333',
  },
};

export default Navbar;
