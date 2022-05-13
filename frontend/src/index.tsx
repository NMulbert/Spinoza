import React from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App";
import { BrowserRouter } from "react-router-dom";
import { Provider } from "react-redux";
import store from "./redux/store";
import AzureAD from "react-aad-msal";
import { signInAuthProvider } from "./authProvider";
import { Notify } from "./components/Notify";

const root = createRoot(document.getElementById("root") as HTMLDivElement);
root.render(
  <Provider store={store}>
    {/* <AzureAD provider={signInAuthProvider} forceLogin={true}> */}
      <BrowserRouter>
        <App />
      </BrowserRouter>
      <Notify />
    {/* </AzureAD> */}
  </Provider>
);
