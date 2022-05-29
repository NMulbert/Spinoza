import React from "react";
import { useState } from "react";
import {
  TextInput,
  Group,
  Button,
  SimpleGrid,
  MultiSelect,
} from "@mantine/core";
import axios from "axios";
import { InputLabel } from "@mui/material";
import MDEditor from "@uiw/react-md-editor";
import { v4 as uuidv4 } from "uuid";
import { useSelector } from "react-redux";
interface TagsState {
  tags: { tags: [] };
}
function CreateTest() {
  let tags = useSelector((s: TagsState) => s.tags.tags);

  const [validateTitle, setValidateTitle] = useState("");
  const [dataHash, setHashData]: any = useState([]);
  const [testValues, setTestsValues] = useState({
    MessageType: "CreateTest",
    id: uuidv4().toUpperCase(),
    title: "",
    description: "",
    status: "Draft",
    authorId: "alonf@zion-net.co.il",
    tags: [],
    questionsRefs: [],
    schemaVersion: "1.0",
    testVersion: "1.0",
    previousVersionId: "none",
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
            error={validateTitle}
            style={{ width: "50%", textAlign: "left" }}
            placeholder="Test title"
            radius="xs"
            maxLength={40}
            onChange={(e: any) => {
              setTestsValues({ ...testValues, title: e.target.value });
            }}
          />
        </div>
        <div data-color-mode="light">
          <InputLabel>Description:</InputLabel>
          <MDEditor
            value={testValues.description}
            style={{ width: "50%" }}
            onChange={(e: any) => {
              setTestsValues({ ...testValues, description: e });
            }}
          />
        </div>
        <div>
          <InputLabel>Tags:</InputLabel>
          <MultiSelect
            data={[...dataHash, ...tags]}
            style={{ width: "50%", textAlign: "left" }}
            placeholder="#Tags"
            radius="xs"
            searchable
            creatable
            getCreateLabel={(query) => `+ Create ${query}`}
            onCreate={(query) =>
              setHashData((current: any) => [...current, query])
            }
            onChange={(e: any) => {
              setTestsValues({
                ...testValues,
                tags: e,
              });
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
                if (testValues.title.trim().length !== 0) {
                  try {
                    let url = `${process.env.REACT_APP_BACKEND_URI}/test`;
                    const response = await axios.post(
                      url,
                      JSON.stringify({ ...testValues })
                    );
                    setTestsValues({
                      ...testValues,
                      id: uuidv4().toUpperCase(),
                    });
                  } catch (err) {
                    console.log(err);
                  }
                } else {
                  setValidateTitle("Title is a required field");
                }
              }}
            >
              Save
            </Button>
          </Group>
        </div>
      </SimpleGrid>
    </>
  );
}

export default CreateTest;
