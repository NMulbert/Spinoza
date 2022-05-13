import React from "react";
import "./App.css";
import { Route, Routes } from "react-router-dom";
import { PageNotFound } from "./components/PageNotFound";
import { useSelector } from "react-redux";
import ViewTest from "./components/Tests/ViewTest";
import { Sidebar } from "./components/AppShell/SideBar";
import Header from "./components/AppShell/Header";
import AllTestsPage from "./components/Tests/AllTestsPage";
import AllQuestionsPage from "./components/Questions/AllQuestionsPage";
import CreateTest from "./components/Tests/CreateTest";
interface TestsState {
  tests: { tests: [] };
}

function App() {
  let tests = useSelector((s: TestsState) => s.tests.tests);

  return (
    <div className="App">
      <Header />
      <Routes>
        <Route path="/" element={<Sidebar />}>
          <Route index element={<AllTestsPage />} />
          <Route path="/tests" element={<AllTestsPage />} />
          <Route path={`/tests/:id`} element={<ViewTest />} />
          <Route path="/create-test" element={<CreateTest />} />
          <Route path="/questions" element={<AllQuestionsPage />} />
          <Route path="*" element={<PageNotFound />} />
        </Route>
      </Routes>
    </div>
  );
}

export default App;
