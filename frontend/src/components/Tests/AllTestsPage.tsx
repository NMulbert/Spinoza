import React, { useEffect, useState } from "react";
import { Grid, Card, Text, Loader, Button } from "@mantine/core";
import { useDispatch, useSelector } from "react-redux";
import TestCard from "./TestCard";
import { loadTests } from "../../redux/Reducers/tests/tests-actions";
import { Link } from "react-router-dom";
import { Writing } from "tabler-icons-react";

interface TestsState {
  tests: { tests: [] };
}

function AllTestsPage() {
  const dispatch = useDispatch();
  let tests = useSelector((s: TestsState) => s.tests.tests);

  useEffect(() => {
    let url = "http://localhost:50000/v1.0/invoke/catalogmanager/method/tests";
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        dispatch(loadTests(result));
      });
  }, []);

  return (
    <div
      style={{
        height: "100%",
        paddingLeft: "270px",
        paddingTop: "30px",
        paddingBottom: "30px",
        backgroundColor: "#f0f0f0",
      }}
    >
      <Grid>
        <Grid.Col md={6} lg={4} xl={3}>
          <Card
            withBorder
            shadow="xl"
            p="lg"
            radius="xl"
            style={{
              height: 309,
              width: "90%",
              minWidth: "90%",
              margin: "auto",
              padding: 0,
              display: "inline-block",
            }}
          >
            <Link to={"/create-test"} style={{ textDecoration: "none" }}>
              <Button
                radius="xl"
                variant="light"
                color="blue"
                style={{
                  fontSize: 30,
                  width: "100%",
                  height: "100%",
                }}
              >
                NEW TEST
                <Writing size={50} />
              </Button>
            </Link>
          </Card>
        </Grid.Col>

        {tests ? (
          tests.map((i: any) => {
            return (
              <Grid.Col key={i.id} md={6} lg={4} xl={3}>
                <TestCard
                  id={i.id}
                  title={i.title}
                  description={i.description}
                  author={i.authorId}
                  tags={i.tags}
                  status={i.status}
                  version={i.version}
                  creationTimeUTC={i.creationTimeUTC}
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
    </div>
  );
}

export default AllTestsPage;
