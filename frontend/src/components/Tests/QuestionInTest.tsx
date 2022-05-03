import { Button, Card, Title } from "@mantine/core";
import React from "react";

type QuestionData = {
  id: string;
  removeQuestion: (question: string) => void;
  editMode: boolean;
};

function QuestionInTest({ id, removeQuestion, editMode }: QuestionData) {
  return (
    <Card
      withBorder
      shadow="xl"
      p="lg"
      radius="xl"
      style={{
        height: 309,
        minHeight: 300,
        width: "90%",
        minWidth: "90%",
        margin: "auto",
        display: "inline-block",
      }}
    >
      <Title order={4} style={{ textAlign: "center" }}>
        {id}
      </Title>
      {editMode ? (
        <Button
          onClick={() => {
            removeQuestion(id);
          }}
        >
          X
        </Button>
      ) : (
        <></>
      )}
    </Card>
  );
}
export default QuestionInTest;
