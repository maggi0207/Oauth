import { useAuth0 } from '@auth0/auth0-react';
import { Link } from 'react-router-dom';

const Navbar = () => {
  const { isAuthenticated, user, loginWithRedirect, logout, isLoading } = useAuth0();

  return (
    <nav style={styles.nav}>
      <div>
        <Link to="/" style={styles.link}> Home </Link>
        <Link to="/UBMediaUpdates" style={styles.link}> Updates </Link>
        {isAuthenticated && (
          <Link to="/UBMOperations" style={styles.link}> Operations Update </Link>
        )}
      </div>

      <div style={styles.authBlock}>
        {!isLoading && isAuthenticated && (
          <span style={styles.user}> {user?.name || user?.email}</span>
        )}
        {!isAuthenticated ? (
          <button style={styles.button} onClick={() => loginWithRedirect()}>
            Login
          </button>
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
