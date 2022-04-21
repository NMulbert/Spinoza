import React, { useState, useEffect } from "react";
import { Badge, Text, Grid, Group, Button, Title, Loader, SimpleGrid } from "@mantine/core";
import { CalendarTime, Edit, UserCircle } from "tabler-icons-react";
import ExistingQuestion from "../Questions/ExistingQuestion";
import QuestionCard from "../Questions/QuestionCard";

function ViewTest() {
  const [questions, setQuestions] = useState([]);

  useEffect(() => {
    let url = "./QuestionObject.json";
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setQuestions(result);
      });
  }, []);

  return (
    <div>
      <Grid>
        <Grid.Col span={12}>
          <Edit size={60} strokeWidth={2} style={{ float:"right"}}/>
          <Title style={{ textDecoration: "underline" }} order={1}>
            Test title
          </Title>

          <Group style={{ color: "#444848" }} spacing="xs">
            <CalendarTime />
            <Title order={5}>Creation Date</Title>
          </Group>

          <Group style={{ color: "#444848" }} spacing="xs">
            <UserCircle />
            <Title order={5}>Author</Title>
          </Group>
        </Grid.Col>

        {/* Tags grid */}
        <Grid.Col span={12}>
          <Group spacing="xs">
            <Badge size="lg" variant="light">
              #JavaScript
            </Badge>
            <Badge size="lg" variant="light">
              #React
            </Badge>
            <Badge size="lg" variant="light">
              #CSS
            </Badge>
          </Group>
        </Grid.Col>

        <Grid.Col span={12}>
          <Title order={5}>Description:</Title>
          <Text size="md">Text area for test description.</Text>
        </Grid.Col>

        {/* Questions grid */}
        <Grid.Col span={12}>
          <Title order={5}>Questions:</Title>
          {/* Will be visible only on edit mode
            <Group spacing="sm">
              <Button
                variant="outline"
                radius="lg"
              >
                ADD NEW QUESTION
              </Button>
              <Button
                variant="outline"
                radius="lg"
                color="dark"
              >
                CHOOSE FROM CATALOG
              </Button>
            </Group> */}
        </Grid.Col>

        {questions ? (
          questions.map((i: any) => {
            return (
              <Grid.Col md={6} lg={3} key={i.Id}>
                <QuestionCard
                  Id={i.Id}
                  Title={i.Title}
                  Description={i.Description}
                  Author={i.Author}
                  Tags={i.Tags}
                  Status={i.Status}
                  Version={i.Version}
                />
              </Grid.Col>
            );
          })
        ) : (
          <>
            <Loader />
          </>
        )}

        {/* Buttons grid */}
        <Grid.Col span={12}>
          <Group spacing="xs">
            <Button
              variant="gradient"
              radius="lg"
              size="lg"
              gradient={{ from: "gray", to: "dark" }}
            >
              Save as Draft
            </Button>
            <Button
              variant="gradient"
              radius="lg"
              size="lg"
              gradient={{ from: "blue", to: "#68b6d7" }}
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
