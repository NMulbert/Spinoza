import React, { useEffect, useState } from "react";
import {
  Grid,
  Card,
  Pagination,
  Loader,
  Button,
  Center,
  MultiSelect,
} from "@mantine/core";
import { useDispatch, useSelector } from "react-redux";
import TestCard from "./TestCard";
import { loadTests } from "../../redux/Reducers/tests/tests-actions";
import { Link } from "react-router-dom";
import { Hash, Writing } from "tabler-icons-react";
import { ConsoleLogger } from "@microsoft/signalr/dist/esm/Utils";

interface TestsState {
  tests: { tests: [] };
}

interface TagsState {
  tags: { tags: [] };
}

function AllTestsPage() {
  let tags = useSelector((s: TagsState) => s.tags.tags);
  let tagsData = tags || [];

  // Pagination
  const [limit] = useState(11);
  const [offset, setOffset] = useState(0);
  const [pageCount, setPageCount] = useState(0);
  const [testTags, setTestTags] = useState<string[]>([]);

  const dispatch = useDispatch();
  let tests = useSelector((s: TestsState) => s.tests.tests);

  // Get limited quantity of tests with option to get by specific tag.
  useEffect(() => {
    let url = `${process.env.REACT_APP_BACKEND_URI}/tests?offset=${offset}&limit=${limit}`;
    if (testTags.length > 0) {
      for (let tag of testTags) {
        url = url + `&tag=${tag}`;
      }
    }
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        dispatch(loadTests(result));
      });
  }, [offset, testTags]);

  useEffect(() => {
    let url = `${process.env.REACT_APP_BACKEND_URI}/tests/count`;
    if (testTags.length > 0) {
      for (let tag of testTags) {
        url = url + `?tag=${tag}`;
      }
    }
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setPageCount(Math.ceil(result / limit));
        setOffset(0);
      });
  }, [testTags]);

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
        <Grid.Col span={12}>
          <MultiSelect
            size="lg"
            data={tagsData}
            placeholder="Select tags"
            icon={<Hash />}
            radius="xl"
            style={{ width: "97%" }}
            styles={{
              value: {
                backgroundColor: "#EEFAEF",
                color: "#48B440",
                fontWeight: "700",
              },
              defaultValueRemove: { color: "#48B440" },
            }}
            searchable
            limit={20}
            nothingFound="Nothing found"
            clearButtonLabel="Clear selection"
            clearable
            value={testTags}
            onChange={setTestTags}
            maxDropdownHeight={100}
          />
        </Grid.Col>
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
