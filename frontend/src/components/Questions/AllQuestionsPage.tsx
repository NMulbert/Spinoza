import React, { useState } from "react";
import { Modal, Grid, Card, Text } from "@mantine/core";
import QuestionCard from "../Questions/QuestionCard";
import NewQuestion from "./NewQuestion";

function AllQuestionsPage() {
  const [openedNQ, setOpenedNQ] = useState(false);

  return (
    <>
      <Modal
        opened={openedNQ}
        onClose={() => setOpenedNQ(false)}
        title="New Question"
        size="75%"
      >
        {<NewQuestion />}
      </Modal>

      <Grid>
        <Grid.Col md={6} lg={3}>
          <Card
            shadow="sm"
            p="lg"
            style={{ width: 340, margin: "auto", height: 307.23 }}
          >
            <Text
              align="center"
              text-align="center"
              variant="gradient"
              gradient={{ from: "indigo", to: "cyan", deg: 45 }}
              size="xl"
              weight={700}
              style={{
                fontFamily: "Greycliff CF, sans-serif",
                padding: "90px",
              }}
              onClick={() => setOpenedNQ(true)}
            >
              New Question <br /> +
            </Text>
          </Card>
        </Grid.Col>
        <Grid.Col md={6} lg={3}>
          <QuestionCard />
        </Grid.Col>
        <Grid.Col md={6} lg={3}>
          <QuestionCard />
        </Grid.Col>
        <Grid.Col md={6} lg={3}>
          <QuestionCard />
        </Grid.Col>
        <Grid.Col md={6} lg={3}>
          <QuestionCard />
        </Grid.Col>
        <Grid.Col md={6} lg={3}>
          <QuestionCard />
        </Grid.Col>
      </Grid>
    </>
  );
}

export default AllQuestionsPage;
