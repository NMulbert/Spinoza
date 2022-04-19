import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";
import { BrowserRouter } from "react-router-dom";

import AzureAD from "react-aad-msal";
import { signInAuthProvider } from "./authProvider";



ReactDOM.render(
<React.StrictMode>
    <AzureAD provider={signInAuthProvider} forceLogin={true}>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </AzureAD>
  </React.StrictMode>,
  document.getElementById("root")
);

