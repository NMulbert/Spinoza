import React, { useState, useEffect } from "react";
import { Badge, Text, Grid, Group, Button, Title, Loader } from "@mantine/core";
import { CalendarTime, Edit, UserCircle } from "tabler-icons-react";
import QuestionCard from "../Questions/QuestionCard";
type ViewTestProps = {
  test: any;
};

function ViewTest({ test }: ViewTestProps) {
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
      <Grid style={{ paddingLeft: "250px", paddingTop: "25px" }}>
        <Grid.Col span={12}>
          <Edit size={60} strokeWidth={2} style={{ float: "right" }} />
          <Title style={{ textDecoration: "underline" }} order={1}>
            {test.title}
          </Title>

          <Group style={{ color: "#444848" }} spacing="xs">
            <CalendarTime />
            <Title order={5}>Creation Date</Title>
          </Group>

          <Group style={{ color: "#444848" }} spacing="xs">
            <UserCircle />
            <Title order={5}>
              {" "}
              {`${test.author.firstName} ${test.author.lastName}`}
            </Title>
          </Group>
        </Grid.Col>

        {/* Tags grid */}
        <Grid.Col span={12}>
          <Group spacing="xs">
            {test.tags.map((i: any) => {
              return (
                <Badge key={i} color="green" variant="light">
                  {i}
                </Badge>
              );
            })}
          </Group>
        </Grid.Col>

        <Grid.Col span={12}>
          <Title order={5}>Description:</Title>
          <Text size="md">{test.description}</Text>
        </Grid.Col>

        {/* Questions grid */}
        <Grid.Col span={12}>
          <Title order={5}>Questions:</Title>
          <br />
          <Group spacing="sm">
            <Button variant="outline" radius="lg">
              ADD NEW QUESTION
            </Button>
            <Button variant="outline" radius="lg" color="dark">
              CHOOSE FROM CATALOG
            </Button>
          </Group>
        </Grid.Col>

        {questions &&
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
          })}
        {/* Buttons grid */}
        <Grid.Col span={12}>
          <Group spacing="sm">
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#838685", to: "#cfd0d0" }}
            >
              Save as Draft
            </Button>
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#217ad2", to: "#4fbaee" }}
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
