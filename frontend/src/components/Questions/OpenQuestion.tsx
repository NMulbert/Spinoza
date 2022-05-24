import React, { useState, useEffect } from "react";
import {
  Badge,
  Grid,
  Group,
  Button,
  Title,
  TextInput,
  MultiSelect,
  Modal,
  TypographyStylesProvider,
} from "@mantine/core";
import { CalendarTime, Edit, UserCircle } from "tabler-icons-react";
import NewQuestion from "../Questions/NewQuestion";
import ChooseQuestion from "../Questions/ChooseQuestion";
import MDEditor from "@uiw/react-md-editor";
import { useParams } from "react-router-dom";
import axios from "axios";
import { useSelector } from "react-redux";
interface TagsState {
  tags: { tags: [] };
}

function OpenQuestion() {
  let { id } = useParams();
  let [question, setQuestion]: any = useState({
    name: "",
    content: "",
    tags: [],
    type: "",
    authorId: "",
    difficultyLevel: "",
    creationTimeUTC: "",
  });

  useEffect(() => {
    let url = `http://localhost:50000/v1.0/invoke/catalogmanager/method/question/${id}`;
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setQuestion(result);
      })
  }, []);

  const [editMode, setEditMode] = useState(false);
  let tags = useSelector((s: TagsState) => s.tags.tags);
  const [dataHash, setHashData]: any = useState([]);
  const [openedNQ, setOpenedNQ] = useState(false);
  const [openedEQ, setOpenedEQ] = useState(false);

  let date = new Date(
    `${question?.creationTimeUTC
      .toString()
      .replace(/T/g, " ")
      .replace(/-/g, "/")} UTC`
  )
    .toString()
    .split(" ");
  let creationTimeUTC = `${date[2]}/ ${date[1]}/ ${date[3]} | ${date[4]}`;

  return (
    <div>
      <Grid
        style={{
          paddingLeft: "250px",
          paddingTop: "25px",
          backgroundColor: "#f0f0f0",
        }}
      >
        <Grid.Col span={12}>
          <Edit
            size={60}
            strokeWidth={2}
            style={{ float: "right", cursor: "pointer" }}
            onClick={(e) => {
              setEditMode(!editMode);
            }}
          />

          {editMode ? (
            <TextInput
              style={{ width: "90%" }}
              value={question.name}
              onChange={(e) => {
                setQuestion({ ...question, name: e.target.value });
              }}
            />
          ) : (
            <Title style={{ textDecoration: "underline" }} order={1}>
              {question.name}
            </Title>
          )}

          <br />
          <Group spacing="xs">
            <CalendarTime />
            <Title order={5}>{creationTimeUTC}</Title>
          </Group>

          <Group spacing="xs">
            <UserCircle />
            <Title order={5}>{question.authorId}</Title>
          </Group>
        </Grid.Col>

        <Grid.Col span={12} data-color-mode="light">
          <Title
            order={5}
            style={{ textDecoration: "underline", marginBottom: 5 }}
          >
            Description:
          </Title>
          {editMode ? (
            <MDEditor
              style={{ width: "90%" }}
              value={question.description}
              onChange={(e: any) => {
                setQuestion({ ...question, content: e });
              }}
            />
          ) : (
            <MDEditor.Markdown
              style={{ backgroundColor: "#f0f0f0", color: "black" }}
              source={question.content}
            />
          )}
        </Grid.Col>

        <Grid.Col span={12}>
          <Title
            order={5}
            style={{ textDecoration: "underline", marginBottom: 5 }}
          >
            Tags:
          </Title>
          {editMode ? (
            (console.log(tags),
            (
              <MultiSelect
                data={[...tags, ...dataHash]}
                style={{ width: "90%", textAlign: "left" }}
                placeholder="#Tags"
                radius="xs"
                searchable
                creatable
                getCreateLabel={(query) => `+ Create ${query}`}
                onCreate={(query) =>
                  setHashData((current: any) => [...current, query])
                }
                value={question.tags}
                onChange={(e: any) => {
                  setQuestion({ ...question, tags: e });
                }}
              />
            ))
          ) : (
            <Group spacing="xs">
              {question.tags?.map((i: any) => {
                return (
                  <Badge key={i} color="green" variant="light">
                    {i}
                  </Badge>
                );
              })}
            </Group>
          )}
        </Grid.Col>
     
        <Grid.Col span={12}>
          <Group spacing="sm">
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#838685", to: "#cfd0d0" }}
              onClick={async () => {
                try {
                  const response = await axios.put(
                    "http://localhost:50000/v1.0/invoke/catalogmanager/method/test",
                    JSON.stringify({
                      messageType: "UpdateTest",
                      ...test,
                    })
                  );
                } catch (err) {
                  alert(err);
                }
              }}
            >
              Save as Draft
            </Button>
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#217ad2", to: "#4fbaee" }}
              onClick={() => {
                console.log({ messageType: "UpdateTest", ...test });
              }}
            >
              Publish
            </Button>
          </Group>
        </Grid.Col>
      </Grid>
    </div>
  );
}

export default OpenQuestion;
