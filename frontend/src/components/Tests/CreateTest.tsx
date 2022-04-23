import React from "react";
import { useState } from "react";
import {
  TextInput,
  Group,
  Button,
  SimpleGrid,
  Modal,
  MultiSelect,
} from "@mantine/core";
import ChooseQuestion from "../Questions/ChooseQuestion";
import NewQuestion from "../Questions/NewQuestion";
import axios from "axios";
import { TestNotify } from "./TestNotify";
import { InputLabel } from "@mui/material";
import MDEditor from "@uiw/react-md-editor";

function CreateTest() {
  const [dataHash, setHashData] = useState([
    "React",
    "C#",
    "JavaScript",
    "Python",
  ]);
  const [testValues, setTestsValues] = useState({
    title: "",
    description: "**Write question here...**",
    author: {
      firstName: "Haim",
      lastName: "Gilboa",
    },
    tags: [],
    status: "Draft",
    version: 0.0,
  });

  return (
    <>
      <SimpleGrid
        cols={1}
        spacing="xs"
        style={{ paddingLeft: "250px", paddingTop: "50px" }}
      >
        <h1>New Test</h1>
        <div>
          <InputLabel>Test title:</InputLabel>
          <TextInput
            style={{ width: "40%", textAlign: "left" }}
            placeholder="Test title"
            radius="xs"
            onChange={(e: any) => {
              setTestsValues({ ...testValues, title: e.target.value });
            }}
          />
        </div>
        <div>
          <InputLabel>Description:</InputLabel>
          <MDEditor
            value={testValues.description}
            style={{ width: "40%" }}
            onChange={(e: any) => {
              setTestsValues({ ...testValues, description: e });
            }}
          />
        </div>
        <div>
          <InputLabel>Tags:</InputLabel>
          <MultiSelect
            data={dataHash}
            style={{ width: "40%", textAlign: "left" }}
            placeholder="#Tags"
            radius="xs"
            searchable
            creatable
            getCreateLabel={(query) => `+ Create ${query}`}
            onCreate={(query) => setHashData((current) => [...current, query])}
            onChange={(e: any) => {
              setTestsValues({ ...testValues, tags: e });
            }}
          />
        </div>
        <div>
          <Group spacing="sm">
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#217ad2", to: "#4fbaee" }}
              onClick={async () => {
                try {
                  const response = await axios.post(
                    "http://localhost:50000/v1.0/invoke/catalogmanager/method/addtest",
                    JSON.stringify({ ...testValues })
                  );
                } catch (err) {
                  console.log(err);
                }
              }}
            >
              Save
            </Button>
          </Group>
        </div>
        <TestNotify />
      </SimpleGrid>
    </>
  );
}

export default CreateTest;
