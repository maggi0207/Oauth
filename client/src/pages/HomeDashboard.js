import React, { useEffect, useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import axios from "axios";

const HomeDashboard = () => {
  const { user, isAuthenticated, getAccessTokenSilently } = useAuth0();
  const [profile, setProfile] = useState(null);

  useEffect(() => {
    if (!isAuthenticated || !user) return;

    const fetchProfileFromBackend = async () => {
      try {
        // Optionally, get an access token if your backend is protected
        const token = await getAccessTokenSilently();
        const res = await axios.get(`http://localhost:5000/api/user/${encodeURIComponent(user.sub)}`, {
          // Uncomment below if your backend requires Auth0 access token
          headers: { Authorization: `Bearer ${token}` },
          withCredentials: true,
        });
        setProfile({
          last_login: res.data.last_login,
          logins_count: res.data.logins_count,
          last_ip: res.data.last_ip,
        });
      } catch (err) {
        console.error("Failed to fetch user profile from backend:", err);
      }
    };

    fetchProfileFromBackend();
  }, [isAuthenticated, user, getAccessTokenSilently]);

  return (
    <div style={{ padding: 20 }}>
      <h1> Welcome to UB Media </h1>
      {isAuthenticated ? (
        <React.Fragment>
          <p>
            You're logged in as <strong>{user.name}</strong>
          </p>
          {profile && (
            <React.Fragment>
              <h3>Last Access Details</h3>
              <div>
                <p>Last login: {profile.last_login}</p>
                <p>Login count: {profile.logins_count}</p>
                <p>Last IP: {profile.last_ip}</p>
              </div>
            </React.Fragment>
          )}
        </React.Fragment>
      ) : (
        <p>Please login to access personalized company resources.</p>
      )}
    </div>
  );
};

export default HomeDashboard;
