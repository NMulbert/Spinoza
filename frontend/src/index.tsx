import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";
import "./index.css";
import { BrowserRouter } from "react-router-dom";
import { Provider } from "react-redux";
import store from "./redux/store";
import AzureAD from "react-aad-msal";
import { signInAuthProvider } from "./authProvider";
import { Notify } from "./components/Notify";

ReactDOM.render(
  <Provider store={store}>
    {/* <AzureAD provider={signInAuthProvider} forceLogin={true}> */}
      <BrowserRouter>
        <App />
      </BrowserRouter>
      <Notify />
   {/* </AzureAD> */}
  </Provider>,
  document.getElementById("root")
);
