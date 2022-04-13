import React from "react";
import "./App.css";
import { Route, Routes } from "react-router-dom";
import Dashboard from "./components/Dashboards/Dashboard";
import { AuthenticationTitle } from "./components/Login/Login";
import { ForgotPassword } from "./components/Login/ForgotPassword";
function App() {
  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<AuthenticationTitle />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/login" element={<AuthenticationTitle />} />
        <Route path="/forgot-password" element={<ForgotPassword />} />
      </Routes>
    </div>
  );
}

export default App;
