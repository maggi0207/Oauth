# 🔐 OAuthLoginApp

OAuthLoginApp is a full-stack demo application that showcases how to implement **Google OAuth 2.0 login** using a **React frontend** and a **Node.js + Express backend**. It demonstrates secure authentication using Passport.js and session handling with express-session.

---

## 🚀 Features

- Google Login integration
- Session-based authentication
- Displays logged-in user’s name and profile picture
- Logout functionality
- Cross-origin support for frontend/backend on different ports

---

## 🎥 Demo Flow

1. User visits the app at `http://localhost:3000`
2. Clicks **"Login with Google"**
3. Redirected to Google for authentication
4. Once authenticated, user is redirected back
5. App displays:
   - Google display name
   - Profile photo
6. Clicking **"Logout"** clears the session

---

## 🖥️ Tech Stack

| Layer      | Technology         |
|------------|--------------------|
| Frontend   | React, Axios       |
| Backend    | Node.js, Express   |
| Auth       | Passport.js (Google OAuth 2.0) |
| Session    | express-session    |
| Misc       | dotenv, CORS       |

---

## 📦 Quick Setup

### 1️⃣ Prerequisites

- Node.js (v14+)
- A Google Cloud project with OAuth 2.0 Client ID
- Test user (Gmail ID) added in OAuth consent screen

### 2️⃣ Clone and Run

```bash
# Clone the project
git clone https://github.com/yourusername/OAuthLoginApp.git
cd OAuthLoginApp
