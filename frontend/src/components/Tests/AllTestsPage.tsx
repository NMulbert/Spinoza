import React, { useEffect, useState } from "react";
import { Grid, Card, Text, Loader } from "@mantine/core";
import { useDispatch, useSelector } from "react-redux";

import TestCard from "./TestCard";
import { loadTests } from "../../redux/Reducers/tests/tests-actions";

type AllTestsPageProps = {
  active: string;
  setActive: (string: string) => void;
};
interface TestsState {
  tests: { tests: [] };
}

function AllTestsPage({ active, setActive }: AllTestsPageProps) {
  const dispatch = useDispatch();
  let tests = useSelector((s: TestsState) => s.tests.tests);

  useEffect(() => {
    let url = "./TestObject.json";
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        dispatch(loadTests(result));
      });
  }, []);

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
      {tests ? (
        tests.map((i: any) => {
          return (
            <Grid.Col md={6} lg={3} key={i.Id}>
              <TestCard
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
    </Grid>
  );
}

export default AllTestsPage;
