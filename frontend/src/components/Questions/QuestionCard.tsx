import {
  Card,
  Text,
  Badge,
  Button,
  Group,
  Title,
  Divider,
} from "@mantine/core";
import MDEditor from "@uiw/react-md-editor";
import { Link } from "react-router-dom";
import { CalendarTime, UserCircle, Link as LinkIcon } from "tabler-icons-react";

type QuestionData = {
  id: string;
  title: string;
  description: string;
  author: any;
  tags: any;
  status: string;
  version: string;
};

function QuestionCard({
  id,
  title,
  description,
  author,
  tags,
  status,
  version,
}: QuestionData) {
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
        {title}
      </Title>
      <Divider my="sm" color="blue" />

      <Card.Section style={{ margin: 5, height: 80 }}>
        <Text lineClamp={3} size="md" weight={500}>
          <MDEditor.Markdown
            style={{ backgroundColor: "white", color: "black" }}
            source={description}
          />
        </Text>
      </Card.Section>

      <Group spacing="xs" style={{ marginTop: 10, marginBottom: 20 }}>
        {tags.map((i: any) => {
          return (
            <Badge key={i} color="green" variant="light">
              {i}
            </Badge>
          );
        })}
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
          {`${author.firstName} ${author.lastName}`}
        </Text>
      </Group>

      <Group position="apart" spacing="xs">
        <Button
          radius="lg"
          variant="light"
          color="blue"
          style={{ marginTop: 14, width: 120 }}
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
  );
}

export default QuestionCard;
