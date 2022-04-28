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
import { Copy } from "tabler-icons-react";
import { CopyToClipboard } from "react-copy-to-clipboard";
import { useState } from "react";

type TestCardProps = {
  id: any;
  title: string;
  description: string;
  author: any;
  tags: any;
  status: string;
  version: number;
  creationTimeUTC: string;
};

const TestCard = ({
  title,
  description,
  author,
  tags,
  id,
  creationTimeUTC,
}: TestCardProps) => {
  const [copied, setCopied] = useState(false);
  return (
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
        <CalendarTime size={18} />
        <Text size="xs" weight={700}>
          {creationTimeUTC}
        </Text>
      </Group>

      <Group style={{ color: "#444848" }} spacing="xs">
        <UserCircle size={18} />
        <Text size="xs" weight={700}>
          {author}
        </Text>
      </Group>

      <Group position="apart" spacing="xs">
        <Link to={`/tests/${id}`} style={{ textDecoration: "none" }}>
          <Button
            radius="lg"
            variant="light"
            color="blue"
            style={{ marginTop: 14, width: 120 }}
          >
            Open
          </Button>
        </Link>
        <CopyToClipboard
          text={`http://localhost:3000/tests/${id}`}
          onCopy={() => {
            setCopied(true);
            setTimeout(() => setCopied(false), 1000);
          }}
        >
          <Button
            radius="lg"
            variant="light"
            color="gray"
            style={{ marginTop: 14 }}
          >
            {copied ? <Copy color="#40c057" /> : <LinkIcon />}
          </Button>
        </CopyToClipboard>
      </Group>
    </Card>
  );
};

export default TestCard;
