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
  question: {
    id: string;
    name: string;
    type: string;
    tags: string[];
    content: {
      answerOptions?: any;
    };
    authorId: string;
    difficultyLevel: string;
  };
  removeQuestion: (question: { id: string }) => void;
  editMode: boolean;
};

function Preview({ question, removeQuestion, editMode }: QuestionData) {
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
            removeQuestion(question);
          }}
        >
          <X size={24} />
        </ActionIcon>
      ) : (
        <></>
      )}

      <Title order={3}>{question.name}</Title>
      <Group spacing="xs" style={{ paddingTop: 5 }}>
        {question.tags?.map((i: any) => {
          return (
            <Badge key={i} color="pink" variant="light">
              {i}
            </Badge>
          );
        })}
      </Group>
      <Divider my="xs" color="blue" />

      <div data-color-mode="light">
        <MDEditor.Markdown source={question.content as string} />
      </div>

      {question.type === "MultipleChoiceQuestion" ? (
        question.content.answerOptions.map((e: any) => (
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
          {question.authorId}
        </Text>
        <CalendarTime size={18} />
        <Text size="xs" weight={700}>
          Creation Date
        </Text>
        <Stars size={18} />
        <Text size="xs" weight={700}>
          Difficulty: {question.difficultyLevel}
        </Text>
      </Group>
    </Card>
  );
}
export default Preview;
