import React, { useEffect, useState } from "react";
import { Grid, Card, Pagination, Loader, Button, Center } from "@mantine/core";
import { useDispatch, useSelector } from "react-redux";
import TestCard from "./TestCard";
import { loadTests } from "../../redux/Reducers/tests/tests-actions";
import { Link } from "react-router-dom";
import { Writing } from "tabler-icons-react";
import { ConsoleLogger } from "@microsoft/signalr/dist/esm/Utils";

interface TestsState {
  tests: { tests: [] };
}

function AllTestsPage() {
  // Pagination
  const [limit] = useState(11);
  const [offset, setOffset] = useState(0);
  const [pageCount, setPageCount] = useState(0);

  const dispatch = useDispatch();
  let tests = useSelector((s: TestsState) => s.tests.tests);

  // Get limited quantity of tests
  useEffect(() => {
    let url2 = `http://localhost:50000/v1.0/invoke/catalogmanager/method/tests?offset=${offset}&limit=${limit}`;
    fetch(url2)
      .then((res) => res.json())
      .then((result) => {
        dispatch(loadTests(result));
      });
  }, [offset]);

  useEffect(() => {
    let url =
      "http://localhost:50000/v1.0/invoke/catalogmanager/method/tests/count";
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setPageCount(Math.ceil(result / limit));
      });
  }, []);

  const handlePageClick = (e: any) => {
    setOffset((e - 1) * limit);
  };

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
        <Grid.Col md={6} lg={6} xl={3}>
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
            let date = new Date(
              `${i.creationTimeUTC
                .toString()
                .replace(/T/g, " ")
                .replace(/-/g, "/")} UTC`
            )
              .toString()
              .split(" ");
            let creationTimeUTC = `${date[2]}/ ${date[1]}/ ${date[3]} | ${date[4]}`;
            return (
              <Grid.Col key={i.id} md={6} lg={6} xl={3}>
                <TestCard
                  id={i.id}
                  title={i.title}
                  description={i.description}
                  author={i.authorId}
                  tags={i.tags}
                  status={i.status}
                  version={i.version}
                  creationTimeUTC={creationTimeUTC}
                />
              </Grid.Col>
            );
          })
        ) : (
          <>
            <Loader />
          </>
        )}
        <Grid.Col>
          <Center>
            <Pagination
              onChange={handlePageClick}
              total={pageCount}
              withEdges
            />
          </Center>
        </Grid.Col>
      </Grid>
    </div>
  );
}

export default AllTestsPage;
