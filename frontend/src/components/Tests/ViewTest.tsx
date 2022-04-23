import React, { useState, useEffect } from "react";
import {
  Badge,
  Grid,
  Group,
  Button,
  Title,
  TextInput,
  MultiSelect,
  Modal,
} from "@mantine/core";
import { CalendarTime, Edit, UserCircle } from "tabler-icons-react";
import QuestionCard from "../Questions/QuestionCard";
import NewQuestion from "../Questions/NewQuestion";
import ChooseQuestion from "../Questions/ChooseQuestion";
import MDEditor from "@uiw/react-md-editor";
type ViewTestProps = {
  test: any;
};

function ViewTest({ test }: ViewTestProps) {
  const [testValus, setTsetValues] = useState({
    title: test.title,
    description: test.description,
    tags: [...test.tags],
    questions: [],
  });
  const [editMode, setEditMode] = useState(false);
  const [dataHash, setHashData] = useState([
    "React",
    "C#",
    "JavaScript",
    "Python",
  ]);
  const [openedNQ, setOpenedNQ] = useState(false);
  const [openedEQ, setOpenedEQ] = useState(false);

  return (
    <div>
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

      <Grid style={{ paddingLeft: "250px", paddingTop: "25px" }}>
        <Grid.Col span={12}>
          <Edit
            size={60}
            strokeWidth={2}
            style={{ float: "right", cursor: "pointer" }}
            onClick={(e) => {
              setEditMode(!editMode);
              console.log(editMode);
            }}
          />

          {editMode ? (
            <TextInput
              label="Test title:"
              style={{ width: "90%" }}
              value={testValus.title}
              onChange={(e) => {
                setTsetValues({ ...testValus, title: e.target.value });
              }}
            />
          ) : (
            <Title style={{ textDecoration: "underline" }} order={1}>
              {testValus.title}
            </Title>
          )}

          <br />
          <Group spacing="xs">
            <CalendarTime />
            <Title order={5}>Creation Date</Title>
          </Group>

          <Group spacing="xs">
            <UserCircle />
            <Title
              order={5}
            >{`${test.author.firstName} ${test.author.lastName}`}</Title>
          </Group>
        </Grid.Col>

        <Grid.Col span={12}>
          <Title order={5}>Description:</Title>
          {editMode ? (
            <MDEditor
              style={{ width: "90%" }}
              value={testValus.description}
              onChange={(e) => {
                setTsetValues({ ...testValus, description: e });
              }}
            />
          ) : (
            <MDEditor.Markdown source={testValus.description} />
          )}
        </Grid.Col>

        <Grid.Col span={12}>
          {editMode ? (
            <MultiSelect
              data={dataHash}
              label="Tags:"
              style={{ width: "90%", textAlign: "left" }}
              placeholder="#Tags"
              radius="xs"
              searchable
              creatable
              getCreateLabel={(query) => `+ Create ${query}`}
              onCreate={(query) =>
                setHashData((current) => [...current, query])
              }
              value={testValus.tags}
              onChange={(e) => {
                setTsetValues({ ...testValus, tags: e });
              }}
            />
          ) : (
            <Group spacing="xs">
              {testValus.tags.map((i: any) => {
                return (
                  <Badge key={i} color="green" variant="light">
                    {i}
                  </Badge>
                );
              })}
            </Group>
          )}
        </Grid.Col>

        {editMode ? (
          <Grid.Col span={12}>
            <Title order={5}>Questions:</Title>
            <br />
            <Group spacing="sm">
              <Button
                variant="outline"
                radius="lg"
                onClick={() => {
                  setOpenedNQ(true);
                }}
              >
                ADD NEW QUESTION
              </Button>
              <Button
                variant="outline"
                radius="lg"
                color="dark"
                onClick={() => {
                  setOpenedEQ(true);
                }}
              >
                CHOOSE FROM CATALOG
              </Button>
            </Group>
          </Grid.Col>
        ) : (
          <></>
        )}

        {testValus.questions.length !== 0 ? (
          testValus.questions.map((i: any) => {
            return (
              <Grid.Col md={6} lg={3} key={i.id}>
                <QuestionCard
                  Id={i.id}
                  Title={i.Title}
                  Description={i.Description}
                  Author={i.Author}
                  Tags={i.Tags}
                  Status={i.Status}
                  Version={i.Version}
                />
              </Grid.Col>
            );
          })
        ) : (
          <></>
        )}

        <Grid.Col span={12}>
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
        </Grid.Col>
      </Grid>
    </div>
  );
}

export default ViewTest;
