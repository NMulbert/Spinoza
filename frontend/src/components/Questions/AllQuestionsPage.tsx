import React, { useState, useEffect } from "react";
import {
  Modal,
  Grid,
  Card,
  Text,
  Loader,
  Button,
  Center,
  Pagination,
  MultiSelect,
} from "@mantine/core";
import QuestionCard from "../Questions/QuestionCard";
import NewQuestion from "./NewQuestion";
import { useDispatch, useSelector } from "react-redux";
import { loadQuestions } from "../../redux/Reducers/questions/questions-actions";
import { Hash, Writing } from "tabler-icons-react";
interface QuestionsState {
  questions: { questions: [] };
}

interface TagsState {
  tags: { tags: [] };
}

function AllQuestionsPage() {
  let tags = useSelector((s: TagsState) => s.tags.tags);
  let tagsData = tags || [];
  // Pagination
  const [limit] = useState(11);
  const [offset, setOffset] = useState(0);
  const [pageCount, setPageCount] = useState(0);
  const [questionsTags, setQuestionsTags] = useState<string[]>([]);

  const dispatch = useDispatch();
  let questions = useSelector((s: QuestionsState) => s.questions.questions);

  // Get limited quantity of tests
  useEffect(() => {
    let url = `${process.env.REACT_APP_BACKEND_URI}/allquestions?offset=${offset}&limit=${limit}`;
    if (questionsTags.length > 0) {
      for (let tag of questionsTags) {
        url = url + `&tag=${tag}`;
      }
    }
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        dispatch(loadQuestions(result));
      });
  }, [offset, questionsTags]);

  useEffect(() => {
    let url = `${process.env.REACT_APP_BACKEND_URI}/questions/count`;
    if (questionsTags.length > 0) {
      for (let tag of questionsTags) {
        url = url + `?tag=${tag}`;
      }
    }
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setPageCount(Math.ceil(result / limit));
        setOffset(0);
      });
  }, [questionsTags]);

  const handlePageClick = (e: any) => {
    setOffset((e - 1) * limit);
  };

  const [openedNQ, setOpenedNQ] = useState(false);

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
      <Modal
        opened={openedNQ}
        onClose={() => setOpenedNQ(false)}
        title="New Question"
        size="100%"
      >
        {<NewQuestion UpdateQuestions={null} setOpenedNQ={setOpenedNQ} />}
      </Modal>
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
            value={questionsTags}
            onChange={setQuestionsTags}
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
            <Button
              radius="xl"
              variant="light"
              color="blue"
              onClick={() => setOpenedNQ(true)}
              style={{
                fontSize: 30,
                width: "100%",
                height: "100%",
              }}
            >
              NEW QUESTION
              <Writing size={50} />
            </Button>
          </Card>
        </Grid.Col>

        {questions ? (
          questions.map((i: any) => {
            return (
              <Grid.Col md={6} lg={6} xl={3} key={i.id}>
                <QuestionCard question={i} />
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

export default AllQuestionsPage;
