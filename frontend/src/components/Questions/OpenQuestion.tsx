import React, { useState, useEffect } from "react";
import {
  Badge,
  Grid,
  Group,
  Button,
  Title,
  TextInput,
  MultiSelect,
  Radio,
  SegmentedControl,
  RadioGroup,
  ActionIcon,
} from "@mantine/core";
import { CalendarTime, Edit, UserCircle } from "tabler-icons-react";
import MDEditor from "@uiw/react-md-editor";
import { useParams } from "react-router-dom";
import axios from "axios";
import { useSelector } from "react-redux";
import MultiChoice from "./MultiChoice";
interface TagsState {
  tags: { tags: [] };
}

function OpenQuestion() {
  let { id } = useParams();
  const [answerType, setAnswerType] = useState("");
  const [questionTxt, setQuestionTxt] = useState("");
  const [multiArr, setMultiArr] = useState([]);
  const [validateName, setValidateName] = useState("");

  let protoType: any =
    answerType === "OpenTextQuestion"
      ? questionTxt
      : { questionText: questionTxt, answerOptions: multiArr };

  let [question, setQuestion]: any = useState({
    name: "",
    content: protoType,
    tags: [],
    type: "",
    authorId: "",
    difficultyLevel: "",
    lastUpdateCreationTimeUTC: "",
  });

  useEffect(() => {
    let url = `${process.env.REACT_APP_BACKEND_URI}/question/${id}`;
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setQuestion(result);
        setAnswerType(result.type);
        setMultiArr(
          result.type === "MultipleChoiceQuestion" &&
            result.content.answerOptions
        );
        setQuestionTxt(
          result.type === "MultipleChoiceQuestion"
            ? result.content.questionText
            : result.content
        );
      });
  }, []);

  useEffect(() => {
    answerType === "MultipleChoiceQuestion"
      ? setQuestion({
          ...question,
          content: {
            questionText: questionTxt,
            answerOptions: multiArr,
          },
        })
      : setQuestion({ ...question, content: questionTxt });
  }, [questionTxt, multiArr, answerType]);

  const [editMode, setEditMode] = useState(false);
  let tags = useSelector((s: TagsState) => s.tags.tags);
  const [dataHash, setHashData]: any = useState([]);

  return (
    <div>
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
              style={{ width: "90%" }}
              error={validateName}
              value={question.name}
              onChange={(e) => {
                setQuestion({ ...question, name: e.target.value });
              }}
            />
          ) : (
            <Title style={{ textDecoration: "underline" }} order={1}>
              {question.name}
            </Title>
          )}

          <br />
          <Group spacing="xs">
            <CalendarTime />
            <Title order={5}>
              {question.lastUpdateCreationTimeUTC
                .slice(0, 19)
                .replace("T", " | ")}
            </Title>
          </Group>

          <Group spacing="xs">
            <UserCircle />
            <Title order={5}>{question.authorId}</Title>
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
              value={questionTxt}
              onChange={(e: any) => {
                setQuestionTxt(e);
              }}
            />
          ) : (
            <MDEditor.Markdown
              style={{ width: "30%", backgroundColor: "#f0f0f0", color: "black" }}
              source={questionTxt}
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
              value={question.tags}
              onChange={(e: any) => {
                setQuestion({ ...question, tags: e });
              }}
            />
          ) : (
            <Group spacing="xs">
              {question.tags?.map((i: any) => {
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
            <SegmentedControl
              size="xl"
              radius="sm"
              color="green"
              value={answerType}
              data={[
                { value: "OpenTextQuestion", label: "Text" },
                { value: "MultipleChoiceQuestion", label: "Multiple Choice" },
              ]}
              onChange={(value: any) => {
                setAnswerType(value);
                setQuestion({
                  ...question,
                  type: value,
                });
              }}
            />
          </Grid.Col>
        ) : (
          <></>
        )}

        {editMode ? (
          answerType === "MultipleChoiceQuestion" ? (
            <Grid.Col span={12}>
              <MultiChoice setMultiArr={setMultiArr} answerOptions={multiArr} />
            </Grid.Col>
          ) : (
            <></>
          )
        ) : (
          <></>
        )}

        {editMode ? (
          <></>
        ) : answerType === "MultipleChoiceQuestion" ? (
          <Grid.Col span={12}>
            <Title
              order={5}
              style={{ textDecoration: "underline", marginBottom: 5 }}
            >
              Answers:
            </Title>
            {multiArr.map((e: any) => (
              <Radio
                disabled
                key={e.description}
                checked={e.isCorrect}
                label={e.description}
                value={e.description}
                style={{ padding: 5 }}
              />
            ))}
          </Grid.Col>
        ) : (
          <></>
        )}

        {editMode ? (
          <Grid.Col span={12}>
            <RadioGroup
              value={question.difficultyLevel}
              onChange={(value: any) => {
                setQuestion({
                  ...question,
                  difficultyLevel: value,
                });
              }}
            >
              <Radio value="1" label="1" />
              <Radio value="2" label="2" />
              <Radio value="3" label="3" />
              <Radio value="4" label="4" />
              <Radio value="5" label="5" />
            </RadioGroup>
          </Grid.Col>
        ) : (
          <Grid.Col span={12}>
            <Title
              order={5}
              style={{ textDecoration: "underline", marginBottom: 5 }}
            >
              Difficulty:
            </Title>
            <RadioGroup value={question.difficultyLevel}>
              <Radio disabled value="1" label="1" />
              <Radio disabled value="2" label="2" />
              <Radio disabled value="3" label="3" />
              <Radio disabled value="4" label="4" />
              <Radio disabled value="5" label="5" />
            </RadioGroup>
          </Grid.Col>
        )}

        <Grid.Col span={12}>
          <Group spacing="sm">
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#838685", to: "#cfd0d0" }}
              onClick={async () => {
                if (question.name.trim().length !== 0) {
                  try {
                    let url = `${process.env.REACT_APP_BACKEND_URI}/question`;
                    const response = await axios.put(
                      url,
                      JSON.stringify({
                        messageType: "UpdateQuestion",
                        ...question,
                      })
                    );
                  } catch (err) {
                    alert(err);
                  }
                } else {
                  setValidateName("Question name is a required field");
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
                console.log({ messageType: "UpdateQuestion", ...question });
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

export default OpenQuestion;
