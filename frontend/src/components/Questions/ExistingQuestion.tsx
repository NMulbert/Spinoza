import React, { useState } from "react";
import { Group, Button, Text, Badge, Card, Space } from "@mantine/core";
import MDEditor from "@uiw/react-md-editor";
import { CalendarTime, Stars, UserCircle } from "tabler-icons-react";

type QuestionData = {
  question: {
    id: string;
    name: string;
    content: any;
    authorId: any;
    difficultyLevel: string;
    type: string;
    tags: any;
  };

  selectedQuestions: string[];
  setSelectedQuestions: (questions: any) => void;
};

function ExistingQuestion({
  question,
  setSelectedQuestions,
  selectedQuestions,
}: QuestionData) {
  const [selected, setSelected] = useState(false);
  return (
    <div>
      <Card
        withBorder
        shadow="sm"
        p="lg"
        style={
          selected
            ? {
                border: "1px solid rgb(34, 139, 230)",
                backgroundColor: "rgb(231, 245, 255)",
                height: "100%",
                margin: 10,
              }
            : { height: "100%", margin: 10 }
        }
      >
        <Text size="lg" weight="bold">
          {question.name}
        </Text>
        <Group style={{ color: "#444848" }} spacing="xs" position="center">
          <UserCircle size={18} />
          <Text size="xs" weight={700}>
            {question.authorId}
          </Text>
          <Stars size={18} />
          <Text size="xs" weight={700}>
            Difficulty: {question.difficultyLevel}
          </Text>
        </Group>

        <Space h="md" />
        <Group position="center" spacing="xs">
          {question.tags.map((i: any) => {
            return (
              <Badge key={i} color="pink" variant="light">
                {i}
              </Badge>
            );
          })}
        </Group>
        <Space h="xs" />
        <Card.Section style={{ margin: 5, height: 80 }}>
          <Text lineClamp={3} size="md" weight={500}>
            {question.type === "MultipleChoiceQuestion" ? (
              question.content.questionText
            ) : question.type === "OpenTextQuestion" ? (
              question.content
            ) : (
              <></>
            )}
          </Text>
        </Card.Section>
        <Space h="md" />
        <Button
          onClick={() => {
            setSelected(!selected);
            selected
              ? setSelectedQuestions(
                  selectedQuestions.filter(
                    (filteredQuestion: any) =>
                      filteredQuestion.id !== question.id
                  )
                )
              : setSelectedQuestions([...selectedQuestions, question]);
          }}
          variant="outline"
          radius="lg"
          size="xs"
        >
          {selected ? "Unselect" : "Select"}
        </Button>
      </Card>
    </div>
  );
}

export default ExistingQuestion;
