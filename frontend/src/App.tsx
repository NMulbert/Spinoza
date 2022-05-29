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
import { loadTags } from "./redux/Reducers/tags/tags-actions";
import OpenQuestion from "./components/Questions/OpenQuestion";

function App() {
  const dispatch = useDispatch();
  useEffect(() => {
    let url = `${process.env.REACT_APP_BACKEND_URI}/tags`;
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        dispatch(loadTags(result));
      });
  }, []);

  return (
    <div className="App">
      <Header />
      <Routes>
        <Route path="/" element={<Sidebar />}>
          <Route index element={<AllTestsPage />} />
          <Route path="/tests" element={<AllTestsPage />} />
          <Route path={`/tests/:id`} element={<ViewTest />} />
          <Route path={`/questions/:id`} element={<OpenQuestion />} />
          <Route path="/create-test" element={<CreateTest />} />
          <Route path="/questions" element={<AllQuestionsPage />} />
          <Route path="*" element={<PageNotFound />} />
        </Route>
      </Routes>
    </div>
  );
}

export default App;
