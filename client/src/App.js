import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar";
import UBMediaUpdates from "./pages/UBMediaUpdates";
import UBMOperations from "./pages/UBMOperations";
import AuthRouteGuard from "./components/AuthRouteGuard";
import HomeDashboard from "./pages/HomeDashboard";
import SessionExpiryDialog from "./components/SessionExpiryDialog";

const App = () => (
  <Router>
    <>
      <SessionExpiryDialog />
      <Navbar /> {/* Now within Router context */}
      <Routes>
        <Route path="/" element={<HomeDashboard />} />
        <Route path="/UBMediaUpdates" element={<UBMediaUpdates />} />
        <Route
          path="/UBMOperations"
          element={
            <AuthRouteGuard>
              <UBMOperations />
            </AuthRouteGuard>
          }
        />
      </Routes>
    </>
  </Router>
);

export default App;
