import {
  ActionIcon,
  Badge,
  Group,
  Radio,
  RadioGroup,
  Title,
} from "@mantine/core";
import React, { useState } from "react";
import { CalendarTime, Edit, UserCircle, Stars } from "tabler-icons-react";

function OpenQuestion() {
  const [editMode, setEditMode] = useState(false);
  const [dataHash, setHashData] = useState([
    "React",
    "C#",
    "JavaScript",
    "Python",
  ]);

  return (
    <div>
      <Edit
        size={60}
        strokeWidth={2}
        style={{ float: "right", cursor: "pointer" }}
        onClick={(e) => {
          setEditMode(!editMode);
        }}
      />
      <Title style={{ textDecoration: "underline" }} order={1}>
        C# WinForm Question
      </Title>
      <Group spacing="xs">
        <CalendarTime />
        <Title order={5}>2 / May / 2022</Title>
      </Group>
      <Group spacing="xs">
        <UserCircle />
        <Title order={5}>alonf@zion-net.co.il</Title>
      </Group>
      <Group spacing="xs">
        <Stars />
        <Title order={5}>Difficulty: 2</Title>
      </Group>
      <br />
      <Group spacing="xs">
        <Badge color="green" variant="light">
          Cloud
        </Badge>
        <Badge color="green" variant="light">
          Dapr
        </Badge>
      </Group>
      <br />
      <Title order={3}>What is Dapr?</Title>
      <RadioGroup orientation="vertical" value="2">
        <Radio disabled value="1" label="Dinozauors and Peaple Reserve" />
        <Radio
          disabled
          value="2"
          label="Distributed Application Runtime"
        />
        <Radio disabled value="3" label="Demos Are Pure Rabish" />
        <Radio
          disabled
          value="4"
          label="Document Availablity Protocol Response"
        />
      </RadioGroup>
    </div>
  );
}

export default OpenQuestion;
