import { Button, ActionIcon, Card, Grid, Title, Text } from "@mantine/core";
import React from "react";
import { X } from "tabler-icons-react";

type QuestionData = {
  id: string;
  removeQuestion: (question: string) => void;
  editMode: boolean;
};

function Preview({ id, removeQuestion, editMode }: QuestionData) {
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

      <Title order={3}>Question name {id}</Title>
      <Text lineClamp={2}>
        From Bulbapedia: Bulbasaur is a small, quadrupedal Pokémon that has
        blue-green skin with darker patches. It has red eyes with white pupils,
        pointed, ear-like structures on top of its head, and a short, blunt
        snout with a wide mouth. A pair of small, pointed teeth are visible in
        the upper jaw when its mouth is open.From Bulbapedia: Bulbasaur is a
        small, quadrupedal Pokémon that has blue-green skin with darker patches.
        It has red eyes with white pupils, pointed, ear-like structures on top
        of its head, and a short, blunt snout with a wide mouth. A pair of
        small, pointed teeth are visible in the upper jaw when its mouth is
        open.
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
    </Card>
  );
}
export default Preview;
