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
} from "@mantine/core";
import { CalendarTime, Edit, UserCircle } from "tabler-icons-react";
import QuestionCard from "../Questions/QuestionCard";
import NewQuestion from "../Questions/NewQuestion";
import ChooseQuestion from "../Questions/ChooseQuestion";
import MDEditor from "@uiw/react-md-editor";
import { useParams } from "react-router-dom";
import axios from "axios";

function ViewTest() {
  let { id } = useParams();
  let [test, setTest] = useState({
    title: "",
    description: "",
    tags: [],
    authorId: "",
    creationTimeUTC: "",
    questions: [],
  });
  useEffect(() => {
    let url = `http://localhost:50000/v1.0/invoke/catalogmanager/method/test/${id}`;
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setTest(result);
      });
  }, []);

  const [editMode, setEditMode] = useState(false);
  const [dataHash, setHashData] = useState([
    "React",
    "C#",
    "JavaScript",
    "Python",
  ]);
  const [openedNQ, setOpenedNQ] = useState(false);
  const [openedEQ, setOpenedEQ] = useState(false);

  return (
    <div>
      <Modal
        opened={openedNQ}
        onClose={() => setOpenedNQ(false)}
        title="New Question"
        size="75%"
      >
        {<NewQuestion />}
      </Modal>
      <Modal
        opened={openedEQ}
        onClose={() => setOpenedEQ(false)}
        title="Questions Catalog"
        size="75%"
      >
        {<ChooseQuestion />}
      </Modal>

      <Grid style={{ paddingLeft: "250px", paddingTop: "25px" }}>
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
              label="Test title:"
              style={{ width: "90%" }}
              value={test.title}
              onChange={(e) => {
                setTest({ ...test, title: e.target.value });
              }}
            />
          ) : (
            <Title style={{ textDecoration: "underline" }} order={1}>
              {test.title}
            </Title>
          )}

          <br />
          <Group spacing="xs">
            <CalendarTime />
            <Title order={5}>
              {test.creationTimeUTC.replace(/T/g, " | ").split(".")[0]}
            </Title>
          </Group>

          <Group spacing="xs">
            <UserCircle />
            <Title order={5}>{test.authorId}</Title>
          </Group>
        </Grid.Col>

        <Grid.Col span={12}>
          <Title order={5}>Description:</Title>
          {editMode ? (
            <MDEditor
              style={{ width: "90%" }}
              value={test.description}
              onChange={(e: any) => {
                setTest({ ...test, description: e });
              }}
            />
          ) : (
            <MDEditor.Markdown
              style={{ backgroundColor: "white", color: "black" }}
              source={test.description}
            />
          )}
        </Grid.Col>

        <Grid.Col span={12}>
          {editMode ? (
            <MultiSelect
              data={dataHash}
              label="Tags:"
              style={{ width: "90%", textAlign: "left" }}
              placeholder="#Tags"
              radius="xs"
              searchable
              creatable
              getCreateLabel={(query) => `+ Create ${query}`}
              onCreate={(query) =>
                setHashData((current) => [...current, query])
              }
              value={test.tags}
              onChange={(e: any) => {
                setTest({ ...test, tags: e });
              }}
            />
          ) : (
            <Group spacing="xs">
              {test.tags?.map((i: any) => {
                return (
                  <Badge key={i} color="green" variant="light">
                    {i}
                  </Badge>
                );
              })}
            </Group>
          )}
        </Grid.Col>

        {editMode ? (
          <Grid.Col span={12}>
            <Title order={5}>Questions:</Title>
            <br />
            <Group spacing="sm">
              <Button
                variant="outline"
                radius="lg"
                onClick={() => {
                  setOpenedNQ(true);
                }}
              >
                ADD NEW QUESTION
              </Button>
              <Button
                variant="outline"
                radius="lg"
                color="dark"
                onClick={() => {
                  setOpenedEQ(true);
                }}
              >
                CHOOSE FROM CATALOG
              </Button>
            </Group>
          </Grid.Col>
        ) : (
          <></>
        )}

        {/* {test.questions.length !== 0 ? (
          (console.log(test.questions.length),
          test.questions.map((i: any) => {
            return (
              <Grid.Col md={6} lg={3} key={i.id}>
                <QuestionCard
                  id={i.id}
                  title={i.Title}
                  description={i.Description}
                  author={i.Author}
                  tags={i.Tags}
                  status={i.Status}
                  version={i.Version}
                />
              </Grid.Col>
            );
          }))
        ) : (
          <></>
        )} */}

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

export default ViewTest;
