# OAuthLoginApp Server – Technical Documentation

---

**File:** `server/index.js`  
**Purpose:** Provides a Node.js/Express backend API to securely fetch Auth0 user details (such as `last_login`, `logins_count`, and `last_ip`) using the Auth0 Management API.

---

## Overview

This Express server exposes an endpoint to retrieve user details from Auth0. It uses the Client Credentials grant to obtain a Management API token and fetches user information securely, so sensitive Auth0 credentials are never exposed to the frontend.

---

## Key Features

- CORS enabled for requests from `http://localhost:3000`
- Securely fetches Auth0 Management API token using client credentials
- Exposes `/api/user/:userId` endpoint to return selected user details
- Handles errors and logs issues for debugging

---

## Environment Variables

The server expects the following variables in a `.env` file in the `server/` directory:

AUTH0_DOMAIN=your-tenant.us.auth0.com
AUTH0_MGMT_CLIENT_ID=your-m2m-client-id
AUTH0_MGMT_CLIENT_SECRET=your-m2m-client-secret
PORT=5000

---

## Main Components

### 1. CORS Middleware

Allows cross-origin requests from the React frontend:

```js
app.use(
  cors({
    origin: "http://localhost:3000",
    credentials: true,
  })
);
```

### 2. Management API Token Helper

Fetches a Management API token using the client credentials grant:

```js
async function getManagementToken() {
  // ...see code for details...
}
```

### 3. User Details Endpoint

Exposes a GET endpoint to fetch user details from Auth0:

```js
app.get("/api/user/:userId", async (req, res) => {
  // ...see code for details...
});
```

Calls getManagementToken() to get a valid token

Fetches user details from Auth0 Management API

Returns last_login, logins_count, and last_ip fields

### 4. Error Handling

Logs errors to the console for debugging.
Returns a 500 status with error message if any step fails

### 5. Server Startup

Starts the Express server on the configured port:

```js
const PORT = process.env.PORT || 5000;
app.listen(PORT, () => {
  console.log(`✅ Server running at http://localhost:${PORT}`);
});
```

### Example Usage

Request:

GET http://localhost:5000/api/user/auth0|xxxxxxxxxxxxxx

Response:

```
{
  "last_login": "2025-07-09T03:04:52.050Z",
  "logins_count": 5,
  "last_ip": "1.2.3.4"
}
```
