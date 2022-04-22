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
import { Notify } from "./Notify";

function CreateTest() {
  const [openedNQ, setOpenedNQ] = useState(false);
  const [openedEQ, setOpenedEQ] = useState(false);
  const [dataHash, setHashData] = useState([
    "React",
    "C#",
    "JavaScript",
    "Python",
  ]);
  const [testValues, setTestsValues] = useState({
    title: "",
    description: "",
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
      <Modal
        opened={openedNQ}
        onClose={() => setOpenedNQ(false)}
        title="New Question"
        size="75%"
      >
        {<NewQuestion />}
      </Modal>
      <Modal
        opened={openedEQ}
        onClose={() => setOpenedEQ(false)}
        title="Questions Catalog"
        size="75%"
      >
        {<ChooseQuestion />}
      </Modal>

      <SimpleGrid
        cols={1}
        spacing="xs"
        style={{ paddingLeft: "250px", paddingTop: "50px" }}
      >
        <h1>New Test</h1>
        <div>
          <TextInput
            label="Test Name:"
            style={{ width: "40%", textAlign: "left" }}
            placeholder="Test Name"
            radius="xs"
            onChange={(e) => {
              let temp = Object.assign(testValues);
              temp.title = e.target.value;
              setTestsValues(temp);
            }}
          />
        </div>
        <div>
          <TextInput
            label="Subject:"
            style={{ width: "40%", textAlign: "left" }}
            placeholder="Subject"
            radius="xs"
            onChange={(e) => {
              let temp = Object.assign(testValues);
              temp.description = e.target.value;
              setTestsValues(temp);
            }}
          />
        </div>
        <div>
          <MultiSelect
            data={dataHash}
            label="Tags:"
            style={{ width: "40%", textAlign: "left" }}
            placeholder="#Tags"
            radius="xs"
            searchable
            creatable
            getCreateLabel={(query) => `+ Create ${query}`}
            onCreate={(query) => setHashData((current) => [...current, query])}
            onChange={(e) => {
              let temp = Object.assign(testValues);
              temp.tags = e;
              setTestsValues(temp);
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
        <Notify />
      </SimpleGrid>
    </>
  );
}

export default CreateTest;
