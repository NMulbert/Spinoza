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
import { Hash } from "tabler-icons-react";

function CreateTest() {
  const [openedNQ, setOpenedNQ] = useState(false);
  const [openedEQ, setOpenedEQ] = useState(false);
  const [dataHash, setHashData] = useState([
    "React",
    "C#",
    "JavaScript",
    "Python",
  ]);
  const [demoEmailsData, setDemoEmailsData] = useState([
    "demo1@gmail.com",
    "demo2@gmail.com",
    "demo3@gmail.com",
    "demo4@gmail.com",
  ]);

  return (
    <>
      <Modal
        opened={openedNQ}
        onClose={() => setOpenedNQ(false)}
        title="New Question"
        size="75%"
      >
        {"<NewQuestion/>"}
      </Modal>
      <Modal
        opened={openedEQ}
        onClose={() => setOpenedEQ(false)}
        title="Questions Catalog"
        size="75%"
      >
        {<ChooseQuestion />}
      </Modal>

      <SimpleGrid cols={1} spacing="xs">
        <h1>New Test</h1>
        <div>
          <TextInput
            label="Test Name:"
            style={{ width: "40%", textAlign: "left" }}
            placeholder="Test Name"
            radius="xs"
          />
        </div>
        <div>
          <TextInput
            label="Subject:"
            style={{ width: "40%", textAlign: "left" }}
            placeholder="Subject"
            radius="xs"
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
          />
        </div>
        <div>
          <Group spacing="sm">
            <Button
              variant="outline"
              radius="lg"
              onClick={() => setOpenedNQ(true)}
            >
              ADD NEW QUESTION
            </Button>
            <Button
              variant="outline"
              radius="lg"
              onClick={() => setOpenedEQ(true)}
              color="dark"
            >
              CHOOSE FROM CATALOG
            </Button>
          </Group>
        </div>
        <div>
          <MultiSelect
            data={demoEmailsData}
            label="Asigned to:"
            style={{ width: "40%", textAlign: "left" }}
            placeholder="Students emails"
            radius="xs"
            searchable
            creatable
            getCreateLabel={(query) => `+ Add ${query}`}
            onCreate={(query) =>
              setDemoEmailsData((current) => [...current, query])
            }
          />
        </div>
        <div>
          <Group spacing="sm">
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#838685", to: "#cfd0d0" }}
            >
              Save as Draft
            </Button>
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#217ad2", to: "#4fbaee" }}
            >
              Publish
            </Button>
          </Group>
        </div>
      </SimpleGrid>
    </>
  );
}

export default CreateTest;
