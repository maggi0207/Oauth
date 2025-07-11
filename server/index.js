// ✅ Updated backend to support Auth0 Management API role-based token inspection
const express = require("express");
const cors = require("cors");
const axios = require("axios");
require("dotenv").config();

const app = express();

app.use(
  cors({
    origin: "http://localhost:3000",
    credentials: true,
  })
);

// ✅ Helper: Get Management API token
async function getManagementToken() {
  try {
    const options = {
      method: "POST",
      url: `https://${process.env.AUTH0_DOMAIN}/oauth/token`,
      headers: { "content-type": "application/x-www-form-urlencoded" },
      data: new URLSearchParams({
        grant_type: "client_credentials",
        client_id: process.env.AUTH0_MGMT_CLIENT_ID,
        client_secret: process.env.AUTH0_MGMT_CLIENT_SECRET,
        audience: `https://${process.env.AUTH0_DOMAIN}/api/v2/`,
      }),
    };

    const response = await axios.request(options);
    return response.data.access_token;
  } catch (err) {
    console.error(" Error fetching management token:", err.response?.data || err.message);
    throw err;
  }
}

// ✅ Route: Get Auth0 user details by user_id
app.get("/api/user/:userId", async (req, res) => {
  try {
    const token = await getManagementToken();
    const userId = req.params.userId;

    const userRes = await axios.get(
      `https://${process.env.AUTH0_DOMAIN}/api/v2/users/${encodeURIComponent(userId)}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    const { last_login, logins_count, last_ip, ...rest } = userRes.data;
    res.json({ last_login, logins_count, last_ip, ...rest });
  } catch (err) {
    console.error("❌ Error fetching user details:", err.response?.data || err.message);
    res.status(500).json({ error: err.message });
  }
});

// ✅ Role validation middleware (optional backend use)
const jwt = require("express-jwt");
const jwksRsa = require("jwks-rsa");

const checkJwt = jwt({
  secret: jwksRsa.expressJwtSecret({
    cache: true,
    rateLimit: true,
    jwksRequestsPerMinute: 5,
    jwksUri: `https://${process.env.AUTH0_DOMAIN}/.well-known/jwks.json`,
  }),
  audience: process.env.AUTH0_AUDIENCE,
  issuer: `https://${process.env.AUTH0_DOMAIN}/`,
  algorithms: ["RS256"],
});

const requireRole = (roles) => {
  return (req, res, next) => {
    const userRoles = req.user["https://ubmedia.com/roles"] || [];
    const hasRole = roles.some((role) => userRoles.includes(role));
    if (!hasRole) {
      return res.status(403).json({ error: "Insufficient permissions" });
    }
    next();
  };
};

// Example secured routes
app.get("/api/admin", checkJwt, requireRole(["admin"]), (req, res) => {
  res.json({ message: "Admin-only data." });
});

app.get("/api/management", checkJwt, requireRole(["manager", "admin"]), (req, res) => {
  res.json({ message: "Management access data." });
});

// ✅ Start server
const PORT = process.env.PORT || 5000;
app.listen(PORT, () => {
  console.log(`🚀 Server running at http://localhost:${PORT}`);
});
