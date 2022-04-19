import { AuthenticationParameters, Configuration } from "msal";
import { MsalAuthProvider, LoginType, IMsalAuthProviderConfig } from "react-aad-msal";
//process.env.NODE_ENV



const tenant: string = process.env.REACT_APP_TENANT as string;
const signInPolicy: string = process.env.REACT_APP_SIGN_IN_POLICY as string;
const applicationID: string = process.env.REACT_APP_APPLICATION_ID as string;
const reactRedirectUri: string = process.env
  .REACT_APP_REACT_REDIRECT_URI as string;

const tenantSubdomain = tenant.split('.')[0];
const instance = `https://${tenantSubdomain}.b2clogin.com/tfp/`;
const signInAuthority = `${instance}${tenant}/${signInPolicy}`;

// Msal Configurations
export const signInConfig: Configuration = {
	auth: {
		authority: signInAuthority,
		clientId: applicationID,
		redirectUri: reactRedirectUri,
		validateAuthority: false,
	},
	cache: {
		cacheLocation: 'sessionStorage',
		storeAuthStateInCookie: false,
	
	},

};

// Authentication Parameters
const authenticationParameters: AuthenticationParameters = {
	// Ref. https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-permissions-and-consent
	scopes: [
		// 'https://graph.microsoft.com/Directory.Read.All',
		// With this permission, an app can receive a unique identifier for the user in the form of the sub claim.
		// It also gives the app access to the UserInfo endpoint.
		// The openid scope can be used at the Microsoft identity platform token endpoint to acquire ID tokens,
		// which can be used by the app for authentication.
		// 'openid',
		// The profile scope can be used with the openid scope and any others.
		// It gives the app access to a substantial amount of information about the user.
		// The information it can access includes, but isn't limited to, the user's given name, surname, preferred username, and object ID.
		// 'profile'
	],
};

// Options
const options: IMsalAuthProviderConfig = {
	loginType: LoginType.Redirect,
	tokenRefreshUri: window.location.origin + '/auth.html',
};

export const signInAuthProvider = new MsalAuthProvider(signInConfig, authenticationParameters, options);