import React, { useEffect, useState } from "react";
import { Grid, Card, Text, Loader } from "@mantine/core";
import { useDispatch, useSelector } from "react-redux";

import TestCard from "./TestCard";
import { loadTests } from "../../redux/Reducers/tests/tests-actions";
import { Link } from "react-router-dom";

interface TestsState {
  tests: { tests: [] };
}

function AllTestsPage() {
  const dispatch = useDispatch();
  let tests = useSelector((s: TestsState) => s.tests.tests);

  useEffect(() => {
    let url =
      "http://localhost:50000/v1.0/invoke/catalogmanager/method/alltests";
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        dispatch(loadTests(result));
      });
  }, []);

  return (
    <Grid style={{ paddingLeft: "250px", paddingTop: "50px" }}>
      <Grid.Col md={6} lg={3}>
        <Card
          shadow="sm"
          p="lg"
          style={{ width: 340, margin: "auto", height: 307.23 }}
        >
          <Link to={"/create-test"} style={{ textDecoration: "none" }}>
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
            >
              New Test <br /> +
            </Text>
          </Link>
        </Card>
      </Grid.Col>
      {tests ? (
        tests.map((i: any) => {
          return (
            <Grid.Col md={6} lg={3} key={i.id}>
              <TestCard
                id={i.id}
                title={i.title}
                description={i.description}
                author={i.author}
                tags={i.tags}
                status={i.status}
                version={i.version}
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
