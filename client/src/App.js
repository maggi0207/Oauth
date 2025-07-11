// src/App.js
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import HomeDashboard from './pages/HomeDashboard';
import UBMediaUpdates from './pages/UBMediaUpdates';
import UBMOperations from './pages/UBMOperations';
import AdminPanel from './pages/AdminPanel';
import AccessDenied from './pages/AccessDenied';
import Navbar from './components/Navbar';
import AuthRouteGuard from './components/AuthRouteGuard';
import './App.css';


function App() {
  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/" element={<HomeDashboard />} />
        <Route path="/UBMediaUpdates" element={<UBMediaUpdates />} />
        <Route path="/UBMOperations" element={<AuthRouteGuard><UBMOperations /></AuthRouteGuard>} />
        <Route path="/admin" element={<AuthRouteGuard roles={['admin']}><AdminPanel /></AuthRouteGuard>} />
        <Route path="/access-denied" element={<AccessDenied />} />
      </Routes>
    </Router>
  );
}

export default App;
