import React from "react";
import "./App.css";
import { Route, Routes } from "react-router-dom";
import Dashboard from "./components/Dashboards/Dashboard";
function App() {
  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/dashboard" element={<Dashboard />} />
      </Routes>
    </div>
  );
}

export default App;
