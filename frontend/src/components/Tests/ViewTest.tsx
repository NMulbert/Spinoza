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
  TypographyStylesProvider,
} from "@mantine/core";
import { CalendarTime, Edit, UserCircle } from "tabler-icons-react";
import NewQuestion from "../Questions/NewQuestion";
import ChooseQuestion from "../Questions/ChooseQuestion";
import MDEditor from "@uiw/react-md-editor";
import { useParams } from "react-router-dom";
import axios from "axios";
import QuestionInTest from "./QuestionInTest";

function ViewTest() {
  let { id } = useParams();
  let [test, setTest] = useState({
    title: "",
    description: "",
    tags: [],
    authorId: "",
    creationTimeUTC: "",
    questions: [],
  });

  useEffect(() => {
    let url = `http://localhost:50000/v1.0/invoke/catalogmanager/method/test/${id}`;
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setTest(result);
      });
  }, []);

  const [editMode, setEditMode] = useState(false);
  const [dataHash, setHashData] = useState([
    "React",
    "C#",
    "JavaScript",
    "Python",
  ]);
  const [openedNQ, setOpenedNQ] = useState(false);
  const [openedEQ, setOpenedEQ] = useState(false);

  let date = new Date(
    `${test?.creationTimeUTC
      .toString()
      .replace(/T/g, " ")
      .replace(/-/g, "/")} UTC`
  )
    .toString()
    .split(" ");
  let creationTimeUTC = `${date[2]}/ ${date[1]}/ ${date[3]} | ${date[4]}`;

  const UpdateQuestions = (questions: []) => {
    setTest({
      ...test,
      questions: Array.from(new Set([...test.questions, ...questions])),
    });
  };

  const removeQuestion = (removedQuestion: string) => {
    setTest({
      ...test,
      questions: test.questions.filter(
        (question) => question !== removedQuestion
      ),
    });
  };
  return (
    <div>
      <Modal
        opened={openedNQ}
        onClose={() => setOpenedNQ(false)}
        title="New Question"
        size="100%"
      >
        {<NewQuestion />}
      </Modal>
      <Modal
        opened={openedEQ}
        onClose={() => setOpenedEQ(false)}
        title="Questions Catalog"
        size="100%"
      >
        {
          <ChooseQuestion
            setOpenedEQ={setOpenedEQ}
            UpdateQuestions={UpdateQuestions}
          />
        }
      </Modal>

      <Grid
        style={{
          paddingLeft: "250px",
          paddingTop: "25px",
          backgroundColor: "#f0f0f0",
        }}
      >
        <Grid.Col span={12}>
          <Edit
            size={60}
            strokeWidth={2}
            style={{ float: "right", cursor: "pointer" }}
            onClick={(e) => {
              setEditMode(!editMode);
            }}
          />

          {editMode ? (
            <TextInput
              style={{ width: "90%" }}
              value={test.title}
              onChange={(e) => {
                setTest({ ...test, title: e.target.value });
              }}
            />
          ) : (
            <Title style={{ textDecoration: "underline" }} order={1}>
              {test.title}
            </Title>
          )}

          <br />
          <Group spacing="xs">
            <CalendarTime />
            <Title order={5}>{creationTimeUTC}</Title>
          </Group>

          <Group spacing="xs">
            <UserCircle />
            <Title order={5}>{test.authorId}</Title>
          </Group>
        </Grid.Col>

        <Grid.Col span={12} data-color-mode="light">
          <Title
            order={5}
            style={{ textDecoration: "underline", marginBottom: 5 }}
          >
            Description:
          </Title>
          {editMode ? (
            <MDEditor
              style={{ width: "90%" }}
              value={test.description}
              onChange={(e: any) => {
                setTest({ ...test, description: e });
              }}
            />
          ) : (
            <MDEditor.Markdown
              style={{ backgroundColor: "#f0f0f0", color: "black" }}
              source={test.description}
            />
          )}
        </Grid.Col>

        <Grid.Col span={12}>
          <Title
            order={5}
            style={{ textDecoration: "underline", marginBottom: 5 }}
          >
            Tags:
          </Title>
          {editMode ? (
            <MultiSelect
              data={dataHash}
              style={{ width: "90%", textAlign: "left" }}
              placeholder="#Tags"
              radius="xs"
              searchable
              creatable
              getCreateLabel={(query) => `+ Create ${query}`}
              onCreate={(query) =>
                setHashData((current) => [...current, query])
              }
              value={test.tags}
              onChange={(e: any) => {
                setTest({ ...test, tags: e });
              }}
            />
          ) : (
            <Group spacing="xs">
              {test.tags?.map((i: any) => {
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
            <Title
              order={5}
              style={{ textDecoration: "underline", marginBottom: 5 }}
            >
              Questions:
            </Title>
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

        {test.questions.length !== 0 ? (
          test.questions.map((i: any) => {
            return (
              <Grid.Col md={12} key={i}>
                <QuestionInTest
                  removeQuestion={removeQuestion}
                  id={i}
                  editMode={editMode}
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
              onClick={async () => {
                try {
                  const response = await axios.put(
                    "http://localhost:50000/v1.0/invoke/catalogmanager/method/test",
                    JSON.stringify({
                      messageType: "UpdateTest",
                      ...test,
                    })
                  );
                } catch (err) {
                  alert(err);
                }
              }}
            >
              Save as Draft
            </Button>
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#217ad2", to: "#4fbaee" }}
              onClick={() => {
                console.log({ messageType: "UpdateTest", ...test });
              }}
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
