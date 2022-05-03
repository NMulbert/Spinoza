import React, { useState } from "react";
import { Group, Button, Text, Badge, Card, Space } from "@mantine/core";

type QuestionData = {
  id: string;
  title: string;
  description: string;
  author: any;
  tags: any;
  status: string;
  version: string;
  selectedQuestions: string[];
  setSelectedQuestions: (questions: any) => void;
};

function ExistingQuestion({
  id,
  title,
  description,
  author,
  tags,
  status,
  version,
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
              }
            : {}
        }
      >
        <Text weight="bold">{title}</Text>
        <Text
          size="sm"
          weight="500"
        >{`${author.firstName} ${author.lastName}`}</Text>
        <Text size="sm" weight="500">
          Date
        </Text>
        <Space h="md" />
        <Group position="center" spacing="xs">
          {tags.map((i: any) => {
            return (
              <Badge key={i} color="pink" variant="light">
                {i}
              </Badge>
            );
          })}
        </Group>
        <Space h="xs" />
        <Text size="sm" lineClamp={3}>
          {description}
        </Text>
        <Space h="md" />
        <Button
          onClick={() => {
            setSelected(!selected);
            selected
              ? setSelectedQuestions(
                  selectedQuestions.filter((question) => question !== id)
                )
              : setSelectedQuestions([...selectedQuestions, id]);
            console.log(selectedQuestions.length);
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
