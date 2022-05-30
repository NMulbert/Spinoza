import {
  Card,
  Text,
  Badge,
  Button,
  Group,
  Title,
  Divider,
  Modal,
  Grid,
} from "@mantine/core";
import MDEditor from "@uiw/react-md-editor";
import { useState } from "react";
import CopyToClipboard from "react-copy-to-clipboard";
import { Link } from "react-router-dom";
import {
  CalendarTime,
  UserCircle,
  Link as LinkIcon,
  Stars,
  Copy,
  Trash,
} from "tabler-icons-react";
import OpenQuestion from "./OpenQuestion";
import axios from "axios";

type QuestionData = {
  question: {
    id: string;
    name: string;
    content: any;
    authorId: any;
    difficultyLevel: string;
    type: string;
    tags: any;
    lastUpdateCreationTimeUTC: string;
  };
};

function QuestionCard({ question }: QuestionData) {
  const [openedQuestion, setOpenedQuestion] = useState(false);
  const [copied, setCopied] = useState(false);
  const [deleteModal, setDeleteModal] = useState(false);

  return (
    <>
      <Modal
        opened={openedQuestion}
        onClose={() => setOpenedQuestion(false)}
        title={question.name}
        size="100%"
      >
        {<OpenQuestion />}
      </Modal>
      <Modal
        centered
        opened={deleteModal}
        onClose={() => setDeleteModal(false)}
        title="Are you sure you want to delete this question?"
        withCloseButton={false}
      >
        {
          <Group position="center">
            <Button
              radius="lg"
              color="red"
              onClick={async () => {
                try {
                  let url = `${process.env.REACT_APP_BACKEND_URI}/question/${question.id}`;
                  const response = await axios.delete(url);
                } catch (err) {
                  console.log(err);
                }
              }}
            >
              Yes
            </Button>
            <Button
              radius="lg"
              onClick={() => {
                setDeleteModal(false);
              }}
            >
              No
            </Button>
          </Group>
        }
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
          {question.name}
        </Title>
        <Divider my="sm" color="blue" />

        <Card.Section style={{ margin: 5, minHeight: 100 }}>
          <Text lineClamp={3} size="md" weight={500}>
            <MDEditor.Markdown
              style={{
                backgroundColor: "white",
                color: "black",
                width: "100%",
                maxHeight: 100,
              }}
              source={
                question.type === "MultipleChoiceQuestion" ? (
                  question.content.questionText
                ) : question.type === "OpenTextQuestion" ? (
                  question.content
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
          {question.tags.map((i: any) => {
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
            Difficulty: {question.difficultyLevel}
          </Text>
        </Group>

        <Group style={{ color: "#444848" }} spacing="xs">
          <CalendarTime size={18} />
          <Text size="xs" weight={700}>
            {question.lastUpdateCreationTimeUTC
              .slice(0, 19)
              .replace("T", " | ")}
          </Text>
        </Group>

        <Group style={{ color: "#444848" }} spacing="xs">
          <UserCircle size={18} />
          <Text size="xs" weight={700}>
            {question.authorId}
          </Text>
        </Group>

        <Group position="apart" spacing="xs">
          <Link
            to={`/questions/${question.id}`}
            style={{ textDecoration: "none" }}
          >
            <Button
              radius="lg"
              variant="light"
              color="blue"
              style={{ marginTop: 14, width: 120 }}
            >
              Open
            </Button>
          </Link>
          <Group position="right" spacing="xs">
            <CopyToClipboard
              text={`${process.env.REACT_APP_WEBSITE_URI}/questions/${question.id}`}
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
            <Button
              color="red"
              variant="light"
              radius="lg"
              style={{ cursor: "pointer", marginTop: 14 }}
              onClick={() => {
                setDeleteModal(true);
              }}
            >
              <Trash />
            </Button>
          </Group>
        </Group>
      </Card>
    </>
  );
}

export default QuestionCard;
