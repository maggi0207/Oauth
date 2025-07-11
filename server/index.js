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

// Helper: Get Management API token
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
    console.error("Error fetching management token:", err.response?.data || err.message);
    throw err;
  }
}

// Route: Get Auth0 user details by user_id
app.get("/api/user/:userId", async (req, res) => {
  try {
    const token = await getManagementToken();
    const userId = req.params.userId;
    const userRes = await axios.get(`https://${process.env.AUTH0_DOMAIN}/api/v2/users/${encodeURIComponent(userId)}`, { headers: { Authorization: `Bearer ${token}` } });
    const { last_login, logins_count, last_ip, ...rest } = userRes.data;
    res.json({ last_login, logins_count, last_ip, ...rest });
  } catch (err) {
    console.error("Error fetching user details:", err.response?.data || err.message);
    res.status(500).json({ error: err.message });
  }
});

// Start server
const PORT = process.env.PORT || 5000;
app.listen(PORT, () => {
  console.log(`✅ Server running at http://localhost:${PORT}`);
});
