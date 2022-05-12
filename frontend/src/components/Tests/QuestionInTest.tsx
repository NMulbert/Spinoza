import { Button, ActionIcon, Card, Grid, Title, Text, Group, Divider } from "@mantine/core";
import React, { useEffect, useState } from "react";
import { CalendarTime, Stars, UserCircle, X } from "tabler-icons-react";

type QuestionData = {
  id: string;
  removeQuestion: (question: string) => void;
  editMode: boolean;
};

function Preview({
  id,
  removeQuestion,
  editMode,
}: QuestionData) {
  const [questionTest, setQuestionTest] = useState({
    name: "",
    authorId: "",
    type: "",
    difficultyLevel: "",
    content: "" || {questionText: ""}
  });

  useEffect(() => {
    let url = `http://localhost:50000/v1.0/invoke/catalogmanager/method/question/${id}`;
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setQuestionTest(result);
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
      <Divider my="sm" color="blue" />
      <Text lineClamp={2}>
        {questionTest.type === "MultipleChoiceQuestion" ? (
          questionTest.content.questionText
        ) : questionTest.type === "OpenTextQuestion" ? (
          questionTest.content
        ) : (
          <></>
        )}
      </Text>

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
