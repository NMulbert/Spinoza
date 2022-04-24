import React, { useEffect } from "react";
import "./App.css";
import { Route, Routes } from "react-router-dom";
import { PageNotFound } from "./components/PageNotFound";
import { useDispatch, useSelector } from "react-redux";
import ViewTest from "./components/Tests/ViewTest";
import { Sidebar } from "./components/AppShell/SideBar";
import Header from "./components/AppShell/Header";
import AllTestsPage from "./components/Tests/AllTestsPage";
import AllQuestionsPage from "./components/Questions/AllQuestionsPage";
import CreateTest from "./components/Tests/CreateTest";
import { loadQuestions } from "./redux/Reducers/questions/questions-actions";
import { loadTests } from "./redux/Reducers/tests/tests-actions";
interface TestsState {
  tests: { tests: [] };
}

function App() {
  let tests = useSelector((s: TestsState) => s.tests.tests);
  useEffect(() => {
    let testsUrl =
      "http://localhost:50000/v1.0/invoke/catalogmanager/method/alltests";
    fetch(testsUrl)
      .then((res) => res.json())
      .then((result) => {
        console.log(result);
        dispatch(loadTests(result));
      });
    let questionsUrl = "./QuestionObject.json";
    fetch(questionsUrl)
      .then((res) => res.json())
      .then((result) => {
        dispatch(loadQuestions(result));
      });
  }, []);
  const dispatch = useDispatch();

  return (
    <div className="App">
      <Header />
      <Routes>
        <Route path="/" element={<Sidebar />}>
          <Route index element={<AllTestsPage />} />
          <Route path="/tests" element={<AllTestsPage />} />
          <Route path="/create-test" element={<CreateTest />} />
          {tests?.map((e: any) => {
            return (
              <Route path={`/tests/${e?.id}`} element={<ViewTest test={e} />} />
            );
          })}
          <Route path="/questions" element={<AllQuestionsPage />} />
          <Route path="*" element={<PageNotFound />} />
        </Route>
      </Routes>
    </div>
  );
}

export default App;
