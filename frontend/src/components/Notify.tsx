import * as signalR from "@microsoft/signalr";
import React, { useEffect, useState } from "react";
import Snackbar from "@mui/material/Snackbar";
import MuiAlert from "@mui/material/Alert";

export const Notify = () => {
  const hubConnection = new signalR.HubConnectionBuilder()
    // .withUrl("https://signalr-management.azurewebsites.net/api")
    .withUrl("http://localhost:80/api")
    .configureLogging(signalR.LogLevel.Information)
    .build();

  hubConnection.start();

  const Alert: any = React.forwardRef(function Alert(props, ref: any) {
    return <MuiAlert elevation={6} variant="filled" ref={ref} {...props} />;
  });

  const SignalRClient: React.FC = () => {
    const [isPublish, setIsPublish] = useState(false);
    const [message, setMessage] = useState("");
    const [severity, setIsSeverity] = useState("");
    const [open, setOpen] = useState(true);
    const handleClose = () => {
      setOpen(false);
      setIsPublish(false);
      setIsSeverity("");
      setMessage("");
    };

    const notify = (message: string, severity: string) => {
      setOpen(true);
      setIsPublish(true);
      setIsSeverity(severity);
      setMessage(message);
    };

    useEffect(() => {
      hubConnection.on("SendMessage", (message) => {
        console.log(message);
        notify(message.text.reason, message.text.actionResult.toLowerCase());
      });
    }, []);

    return (
      <div>
        {isPublish && (
          <Snackbar open={open} autoHideDuration={6000} onClose={handleClose}>
            <Alert
              onClose={handleClose}
              severity={severity}
              sx={{ width: "100%" }}
            >
              {message}
            </Alert>
          </Snackbar>
        )}
      </div>
    );
  };
  return (
    <>
      <SignalRClient />
    </>
  );
};
