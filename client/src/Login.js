import { useAuth0 } from '@auth0/auth0-react';

const styles = {
  container: {
    fontFamily: 'Arial, sans-serif',
    padding: '2rem',
    maxWidth: 500,
    margin: '3rem auto',
    textAlign: 'center',
    border: '1px solid #ddd',
    borderRadius: 12,
    boxShadow: '0 0 10px rgba(0,0,0,0.1)',
  },
  heading: {
    fontSize: '2rem',
    color: '#333',
    marginBottom: '1rem',
  },
  profileImg: {
    borderRadius: '50%',
    width: 80,
    height: 80,
    marginTop: '1rem',
  },
  button: {
    backgroundColor: '#2e77d0',
    color: 'white',
    padding: '0.8rem 1.5rem',
    fontSize: '1rem',
    border: 'none',
    borderRadius: 6,
    cursor: 'pointer',
    marginTop: '1rem',
  },
  logoutLink: {
    display: 'inline-block',
    marginTop: '1rem',
    color: '#d9534f',
    textDecoration: 'none',
    fontWeight: 'bold',
  },
};

const Login = () => {
  const {
    loginWithRedirect,
    logout,
    user,
    isAuthenticated,
    isLoading,
  } = useAuth0();


  if (isLoading) {
    return (
      <div style={styles.container}>
        <p>Loading...</p>
      </div>
    );
  }

  console.log("isAuthenticated value",isAuthenticated)
   console.log("User details",user)

  return (
    <div style={styles.container}>
      <h1 style={styles.heading}>Login Page</h1>
      {isAuthenticated ? (
        <>
          <img src={user.picture} alt="Profile" style={styles.profileImg} />
          <h2>Welcome, {user.name}</h2>
      
          <br />
          <button
            onClick={() =>
              logout({ logoutParams: { returnTo: window.location.origin } })
            }
            style={styles.logoutLink}
          >
            Logout
          </button>
        </>
      ) : (
        <button onClick={() => loginWithRedirect()} style={styles.button}>
          Login with Auth0
        </button>
      )}
    </div>
  );
};

export default Login;
