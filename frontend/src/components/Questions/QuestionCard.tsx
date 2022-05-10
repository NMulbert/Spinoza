import {
  Card,
  Text,
  Badge,
  Button,
  Group,
  Title,
  Divider,
  Modal,
} from "@mantine/core";
import MDEditor from "@uiw/react-md-editor";
import { useState } from "react";
import { Link } from "react-router-dom";
import { CalendarTime, UserCircle, Link as LinkIcon, Stars } from "tabler-icons-react";
import OpenQuestion from "./OpenQuestion";

type QuestionData = {
  id: string;
  name: string;
  content: any;
  authorId: any;
  difficultyLevel: string;
  type: string
  tags: any;

};

function QuestionCard({
  id,
  name,
  content,
  authorId,
  difficultyLevel,
  type,
  tags
}: QuestionData) {
  const [openedQuestion, setOpenedQuestion] = useState(false);

  return (
    <>
      <Modal
        opened={openedQuestion}
        onClose={() => setOpenedQuestion(false)}
        title={name}
        size="100%"
      >
        {<OpenQuestion />}
      </Modal>
      <Card
        withBorder
        shadow="xl"
        p="lg"
        radius="xl"
        style={{
          height: "100%",
          width: "90%",
          minWidth: "90%",
          margin: "auto",
          display: "inline-block",
        }}
      >
        <Title order={4} style={{ height: 45, textAlign: "center" }}>
          {name}
        </Title>
        <Divider my="sm" color="blue" />

        <Card.Section style={{ margin: 5, height: 80 }}>
          <Text lineClamp={3} size="md" weight={500}>
            <MDEditor.Markdown
              style={{ backgroundColor: "white", color: "black" }}
              source={
                type === "MultipleChoiceQuestion" ? (
                  content.questionText
                ) : type === "OpenTextQuestion" ? (
                  content
                ) : (
                  <></>
                )
              }
            />
          </Text>
        </Card.Section>

        <Group
          spacing="xs"
          style={{ marginTop: 10, marginBottom: 20, height: 40 }}
        >
          {tags.map((i: any) => {
            return (
              <Badge key={i} color="green" variant="light">
                {i}
              </Badge>
            );
          })}
        </Group>

        <Group style={{ color: "#444848" }} spacing="xs">
          <Stars size={18} />
          <Text size="xs" weight={700}>
            Difficulty: {difficultyLevel}
          </Text>
        </Group>

        <Group style={{ color: "#444848" }} spacing="xs">
          <CalendarTime size={18} />
          <Text size="xs" weight={700}>
            Creation Date
          </Text>
        </Group>

        <Group style={{ color: "#444848" }} spacing="xs">
          <UserCircle size={18} />
          <Text size="xs" weight={700}>
            {authorId}
          </Text>
        </Group>

        <Group position="apart" spacing="xs">
          <Button
            radius="lg"
            variant="light"
            color="blue"
            style={{ marginTop: 14, width: 120 }}
            onClick={() => {
              setOpenedQuestion(true);
            }}
          >
            Open
          </Button>
          <Button
            radius="lg"
            variant="light"
            color="gray"
            style={{ marginTop: 14 }}
          >
            <LinkIcon />
          </Button>
        </Group>
      </Card>
    </>
  );
}

export default QuestionCard;
