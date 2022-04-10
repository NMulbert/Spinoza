import SideBar from "../SideBar/SideBar";
import React, { useState } from "react";
import "./NewTest.css";
import NewQuestion from "../NewQuestion/NewQuestion";
import ChooseFromCatalog from "../ChooseFromCatalog/ChooseFromCatalog";
import { AppShell, Navbar, Header } from "@mantine/core";

const NewTest = () => {
  const [newQuestion, setNewQuestion] = useState(false);
  const [chooseFromCatalog, setChooseFromCatalog] = useState(false);

  return (
    <AppShell
      padding="md"
      navbar={
        <Navbar height={"100vh"} width={{ sm: 300 }} p="md">
          <SideBar />
        </Navbar>
      }
      styles={(theme) => ({
        main: {
          backgroundColor:
            theme.colorScheme === "dark"
              ? theme.colors.dark[8]
              : theme.colors.gray[0],
        },
      })}
    >
      <div className="row">
        <div>
          <h1>New Test</h1>
          <div className="row">
            <button
              className="add-new-question-btn"
              onClick={() => {
                setNewQuestion(!newQuestion);
              }}
            >
              ADD NEW QUESTION
            </button>
            <button
              className="choose-from-catalog-btn"
              onClick={() => {
                setChooseFromCatalog(!chooseFromCatalog);
              }}
            >
              Choose From Catalog
            </button>
          </div>
          <br />
          <div className="row">
            <button className="publish-btn">Publish</button>
            <button className="save-as-draft-btn">Save As Draft</button>
          </div>
          <NewQuestion
            trigger={newQuestion}
            setNewQuestion={setNewQuestion}
          ></NewQuestion>
          <ChooseFromCatalog
            trigger={chooseFromCatalog}
            setChooseFromCatalog={setChooseFromCatalog}
          ></ChooseFromCatalog>
        </div>
      </div>
    </AppShell>
  );
};

export default NewTest;
