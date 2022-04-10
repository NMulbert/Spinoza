import React from "react";
import {
  AppShell,
  Header,
  Image,
  SimpleGrid,
  Space,
  Grid,
  Card,
  Text,
} from "@mantine/core";
import CreateTest from "./CreateTest";
import { Sidebar } from "../AppShell/SideBar";
import TestCard from "./TestCard";

type AllTestsPageProps = {
  active: string;
  setActive: (string: string) => void;
};

function AllTestsPage({ active, setActive }: AllTestsPageProps) {
  return (
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
            onClick={() => {
              setActive("CreateTest");
            }}
          >
            New Test <br /> +
          </Text>
        </Card>
      </Grid.Col>
      <Grid.Col md={6} lg={3}>
        <TestCard />
      </Grid.Col>
      <Grid.Col md={6} lg={3}>
        <TestCard />
      </Grid.Col>
      <Grid.Col md={6} lg={3}>
        <TestCard />
      </Grid.Col>
      <Grid.Col md={6} lg={3}>
        <TestCard />
      </Grid.Col>
      <Grid.Col md={6} lg={3}>
        <TestCard />
      </Grid.Col>
    </Grid>
  );
}

export default AllTestsPage;
