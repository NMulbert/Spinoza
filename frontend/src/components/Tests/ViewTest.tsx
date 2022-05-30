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
  ActionIcon,
} from "@mantine/core";
import { CalendarTime, Edit, UserCircle } from "tabler-icons-react";
import NewQuestion from "../Questions/NewQuestion";
import ChooseQuestion from "../Questions/ChooseQuestion";
import MDEditor from "@uiw/react-md-editor";
import { useParams } from "react-router-dom";
import axios from "axios";
import QuestionInTest from "./QuestionInTest";
import { useSelector } from "react-redux";
interface TagsState {
  tags: { tags: [] };
}

function ViewTest() {
  let { id } = useParams();
  const [validateTitle, setValidateTitle] = useState("");
  let [questions, setQuestions] = useState([]);
  useEffect(() => {
    let url = `${process.env.REACT_APP_BACKEND_URI}/testquestions/${id}`;
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setQuestions(result);
      });
  }, []);

  let [test, setTest]: any = useState({
    title: "",
    description: "",
    tags: [],
    authorId: "",
    creationTimeUTC: "",
    questionsRefs: [],
    questions: questions,
  });

  useEffect(() => {
    let url = `${process.env.REACT_APP_BACKEND_URI}/test/${id}`;
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setTest({ ...result, questions: [...questions] });
      });
  }, []);

  const [editMode, setEditMode] = useState(false);
  let tags = useSelector((s: TagsState) => s.tags.tags);
  const [dataHash, setHashData]: any = useState([]);
  const [openedNQ, setOpenedNQ] = useState(false);
  const [openedEQ, setOpenedEQ] = useState(false);

  const UpdateQuestions = (updatedQuestions: []) => {
    let resArr: any = [];
    questions.length > 0
      ? [...questions, ...updatedQuestions].filter((item: any) => {
          var i = resArr.findIndex((x: any) => x.id === item.id);
          if (i <= -1) {
            resArr.push({ ...item, id: item.id });
          }
          return null;
        })
      : updatedQuestions.filter((item: any) => {
          var i = resArr.findIndex((x: any) => x.id === item.id);
          if (i <= -1) {
            resArr.push({ ...item, id: item.id });
          }
          return null;
        });
    setQuestions(resArr);
    setTest({
      ...test,
      questionsRefs: [
        ...test.questionsRefs,
        ...updatedQuestions.map((question: { id: string }) => {
          return question.id;
        }),
      ],
    });
  };

  const removeQuestion = (removedQuestion: { id: string }) => {
    setQuestions(
      questions.filter(
        (question: { id: string }) => question.id !== removedQuestion.id
      )
    );

    setTest({
      ...test,
      questionsRefs: [
        ...test.questionsRefs.filter(
          (question: string) => question !== removedQuestion.id
        ),
      ],
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
        {
          <NewQuestion
            UpdateQuestions={UpdateQuestions}
            setOpenedNQ={setOpenedNQ}
          />
        }
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
          <ActionIcon
            size="xl"
            color="blue"
            variant="hover"
            radius="lg"
            style={{ float: "right", cursor: "pointer" }}
            onClick={() => {
              setEditMode(!editMode);
            }}
          >
            <Edit size={50} />
          </ActionIcon>

          {editMode ? (
            <TextInput
              error={validateTitle}
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
            <Title order={5}>
              {test.creationTimeUTC.slice(0, 19).replace("T", " | ")}
            </Title>
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
              data={[...tags, ...dataHash]}
              style={{ width: "90%", textAlign: "left" }}
              placeholder="#Tags"
              radius="xs"
              searchable
              creatable
              getCreateLabel={(query) => `+ Create ${query}`}
              onCreate={(query) =>
                setHashData((current: any) => [...current, query])
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

        {questions.length > 0 &&
          questions.map((question: any) => {
            return (
              <Grid.Col md={12} key={question.id}>
                <QuestionInTest
                  removeQuestion={removeQuestion}
                  question={question}
                  editMode={editMode}
                />
              </Grid.Col>
            );
          })}
        <Grid.Col span={12}>
          <Group spacing="sm">
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#838685", to: "#cfd0d0" }}
              onClick={async () => {
                if (test.title.trim().length !== 0) {
                  try {
                    let url = `${process.env.REACT_APP_BACKEND_URI}/test`;

                    const response = await axios.put(
                      url,
                      JSON.stringify({
                        messageType: "UpdateTest",
                        ...test,
                      })
                    );
                  } catch (err) {
                    console.log(err);
                  }
                } else {
                  setValidateTitle("Title is a required field");
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
