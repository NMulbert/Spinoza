import {
  Button,
  ActionIcon,
  Card,
  Radio,
  Title,
  Text,
  Group,
  Divider,
  Badge,
  Checkbox,
} from "@mantine/core";
import MDEditor from "@uiw/react-md-editor";
import React, { useEffect, useState } from "react";
import { CalendarTime, Stars, UserCircle, X } from "tabler-icons-react";

type QuestionData = {
  id: string;
  removeQuestion: (question: string) => void;
  editMode: boolean;
};

function Preview({ id, removeQuestion, editMode }: QuestionData) {
  const [answerType, setAnswerType] = useState();
  const [questionTxt, setQuestionTxt] = useState("");
  const [correctAnswers, setCorrectAnswers] = useState(0);
  const [answers, setAnswers]: any = useState([]);

  let protoType: any =
    answerType === "OpenTextQuestion"
      ? questionTxt
      : { questionText: questionTxt, answerOptions: [] };

  const [questionTest, setQuestionTest] = useState({
    name: "",
    authorId: "",
    type: "",
    difficultyLevel: "",
    tags: [],
    content: protoType,
  });

  useEffect(() => {
    let url = `http://localhost:50000/v1.0/invoke/catalogmanager/method/question/${id}`;
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setQuestionTest(result);
        setAnswerType(result.type);
        setQuestionTxt(
          result.type === "MultipleChoiceQuestion"
            ? result.content.questionText
            : result.content
        );
      });
  }, []);

  return (
    <Card
      withBorder
      shadow="xl"
      p="lg"
      radius="xl"
      style={{
        height: "100%",
        minHeight: "100%",
        width: "90%",
        minWidth: "90%",
        margin: "auto",
        display: "inline-block",
      }}
    >
{console.log("hi")}
      {editMode ? (
        <ActionIcon
          color="red"
          variant="hover"
          radius="lg"
          style={{ float: "right", cursor: "pointer" }}
          onClick={() => {
            removeQuestion(id);
          }}
        >
          <X size={24} />
        </ActionIcon>
      ) : (
        <></>
      )}

      <Title order={3}>{questionTest.name}</Title>
      <Group spacing="xs" style={{ paddingTop: 5 }}>
        {questionTest.tags?.map((i: any) => {
          return (
            <Badge key={i} color="pink" variant="light">
              {i}
            </Badge>
          );
        })}
      </Group>
      <Divider my="xs" color="blue" />

      <div data-color-mode="light">
        <MDEditor.Markdown source={questionTxt} />
      </div>

      {questionTest.type === "MultipleChoiceQuestion" ? (
        questionTest.content.answerOptions.map((e: any) => (
          <Radio
            key={e.description}
            disabled
            checked={e.isCorrect}
            label={e.description}
            value={e.description}
            style={{ padding: 5 }}
          />
        ))
      ) : (
        <></>
      )}

      {/* {questionTest.type === "MultipleChoiceQuestion" ? (
        (questionTest.content.answerOptions.map(
          (i: any) => (
            i.isCorrect
              ? setCorrectAnswers(correctAnswers + 1)
              : console.log(""),
            setAnswers([...answers, i])
          )
        ),
        correctAnswers > 1
          ? answers.map((e: any) => (
              <Checkbox
                key={e.description}
                disabled
                checked={e.isCorrect}
                label={e.description}
                value={e.description}
                style={{ padding: 5 }}
              />
            ))
          : answers.map((e: any) => (
              <Radio
                key={e.description}
                disabled
                checked={e.isCorrect}
                label={e.description}
                value={e.description}
                style={{ padding: 5 }}
              />
            )))
      ) : questionTest.type === "OpenTextQuestion" ? (
        <></>
      ) : (
        <></>
      )} */}

      <Button
        size="xs"
        radius="lg"
        variant="light"
        color="blue"
        style={{ marginTop: 14, width: 120 }}
      >
        Open
      </Button>
      <Group
        style={{ marginTop: 14, color: "#444848", float: "right" }}
        spacing="xs"
        position="center"
      >
        <UserCircle size={18} />
        <Text size="xs" weight={700}>
          {questionTest.authorId}
        </Text>
        <CalendarTime size={18} />
        <Text size="xs" weight={700}>
          Creation Date
        </Text>
        <Stars size={18} />
        <Text size="xs" weight={700}>
          Difficulty: {questionTest.difficultyLevel}
        </Text>
      </Group>
    </Card>
  );
}
export default Preview;
