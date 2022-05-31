import {
  Card,
  Text,
  Badge,
  Button,
  Group,
  Title,
  Divider,
  Modal,
  ActionIcon,
} from "@mantine/core";
import MDEditor from "@uiw/react-md-editor";
import { Link } from "react-router-dom";
import {
  CalendarTime,
  UserCircle,
  Link as LinkIcon,
  Trash,
} from "tabler-icons-react";
import { Copy } from "tabler-icons-react";
import { CopyToClipboard } from "react-copy-to-clipboard";
import { useState } from "react";
import axios from "axios";

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
  const [deleteModal, setDeleteModal] = useState(false);

  return (
    <div>
      <Modal
        centered
        opened={deleteModal}
        onClose={() => setDeleteModal(false)}
        title="Are you sure you want to delete this test?"
        withCloseButton={false}
      >
        {
          <Group position="center">
            <Button
              radius="lg"
              color="red"
              onClick={async () => {
                try {
                  let url = `${process.env.REACT_APP_BACKEND_URI}/test/${id}`;
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
          {title}
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
              source={description}
            />
          </Text>
        </Card.Section>

        <Card.Section style={{ margin: 5 }}>
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
              {creationTimeUTC.slice(0, 19).replace("T", " | ")}
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
                style={{ marginTop: 14, width: 100 }}
              >
                Open
              </Button>
            </Link>
            <Group position="right" spacing="xs">
              <CopyToClipboard
                text={`${process.env.REACT_APP_WEBSITE_URI}/tests/${id}`}
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
                style={{ float: "right", cursor: "pointer", marginTop: 14 }}
                onClick={() => {
                  setDeleteModal(true);
                }}
              >
                <Trash />
              </Button>
            </Group>
          </Group>
        </Card.Section>
      </Card>
    </div>
  );
};

export default TestCard;
