import * as signalR from "@microsoft/signalr";
import React, { useEffect, useState } from "react";
import Snackbar from "@mui/material/Snackbar";
import MuiAlert from "@mui/material/Alert";
import { useDispatch, useSelector } from "react-redux";
import {
  addTest,
  updateTest,
  deleteTest,
} from "../redux/Reducers/tests/tests-actions";
import { addTags } from "../redux/Reducers/tags/tags-actions";
import {
  addQuestion,
  updateQuestion,
  deleteQuestion,
} from "../redux/Reducers/questions/questions-actions";

export const Notify = () => {
  var tests = useSelector((s: any) => s.tests.tests);
  var questions = useSelector((s: any) => s.questions.questions);
  var tags = useSelector((s: any) => s.tags.tags);
  const dispatch = useDispatch();
  const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`${process.env.REACT_APP_SIGNALR_NEGOTIATE_URI}`)
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

    const fetchTestById = async (id: string) => {
      try {
        let url = `${process.env.REACT_APP_BACKEND_URI}/test/${id}`;
        let res = await fetch(url);
        return res.json();
      } catch (exception) {
        console.log(exception);
        return null;
      }
    };

    const fetchQuestionById = async (id: string) => {
      try {
        let url = `${process.env.REACT_APP_BACKEND_URI}/question/${id}`;
        let res = await fetch(url);
        return res.json();
      } catch (exception) {
        console.log(exception);
        return null;
      }
    };

    const hendleTestUpdate = async (testInfo: any) => {
      if (tests === undefined) return;
      if (testInfo.actionResult !== "Success") {
        return;
      }
      switch (testInfo.messageType) {
        case "Update":
          let test = await fetchTestById(testInfo.id);
          dispatch(updateTest(test));
          break;
        case "Create":
          test = await fetchTestById(testInfo.id);
          dispatch(addTest([...tests, test]));
          break;
        case "Deleted":
          let deletedTest = { id: testInfo.id };
          dispatch(deleteTest(deletedTest));
          break;
        default:
          console.log("Hendle test update: Unknown Message Type");
      }
    };

    const hendleQuestionUpdate = async (questionInfo: any) => {
      if (questions === undefined) return;
      if (questionInfo.actionResult !== "Success") {
        return;
      }
      switch (questionInfo.messageType) {
        case "Update":
                let question = await fetchQuestionById(questionInfo.id);

          dispatch(updateQuestion(question));
          break;
        case "Create":
                 question = await fetchQuestionById(questionInfo.id);

          dispatch(addQuestion([...questions, question]));
          break;
        case "Deleted":
          let deletedQuesiton = { id: questionInfo.id}
          dispatch(deleteQuestion(deletedQuesiton));
          break;
        default:
          console.log("Hendle question update: Unknown Message Type");
      }
    };

    const hendleTagUpdate = (tagList: string[]) => {
      function onlyUnique(value: any, index: any, self: any) {
        return self.indexOf(value) === index;
      }
      var unique = tagList.filter(onlyUnique);
      dispatch(addTags([...tags, ...unique]));
    };

    const notify = (message: string, severity: string) => {
      setOpen(true);
      setIsPublish(true);
      setIsSeverity(severity);
      setMessage(message);
    };

    useEffect(() => {
      hubConnection.on("SendMessage", (message) => {
        setTimeout(() => {
          notify(message.text.reason, message.text.actionResult.toLowerCase());
        }, 100);
        setTimeout(() => {
          switch (message.text.resourceType) {
            case "Tag":
              hendleTagUpdate(message.text.reason.split(","));
              break;
            case "Test":
              hendleTestUpdate(message.text);
              break;
            case "Question":
              hendleQuestionUpdate(message.text);
              break;
            default:
              console.log("Signal R unknown message");
          }
        }, 1000);
      });
    }, []);

    return (
      <div>
        {isPublish && (
          <Snackbar open={open} autoHideDuration={6000} onClose={handleClose}>
            <Alert
              onClose={handleClose}
              severity={severity !== "" ? severity : "success"}
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
