import { useAuth0 } from '@auth0/auth0-react';

const UBMOperations = () => {
  const { user } = useAuth0();
  return (
    <div style={{ padding: 20 }}>
      <h2> UBM Operations </h2>
      <p>Welcome, {user.name}</p>
      <ul>
        <li> Employees List:</li>
      </ul>
    </div>
  );
};

export default UBMOperations;
