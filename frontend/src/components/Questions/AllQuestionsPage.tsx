import React, { useState, useEffect } from "react";
import { Modal, Grid, Card, Text, Loader, Button, Center, Pagination } from "@mantine/core";
import QuestionCard from "../Questions/QuestionCard";
import NewQuestion from "./NewQuestion";
import { useDispatch, useSelector } from "react-redux";
import { loadQuestions } from "../../redux/Reducers/questions/questions-actions";
import { Writing } from "tabler-icons-react";
interface QuestionsState {
  questions: { questions: [] };
}

function AllQuestionsPage() {
  // Pagination
  const [limit] = useState(11);
  const [offset, setOffset] = useState(0);
  const [pageCount, setPageCount] = useState(0);

  const dispatch = useDispatch();
  let questions = useSelector((s: QuestionsState) => s.questions.questions);

  // Get limited quantity of tests
  useEffect(() => {
    let url2 = `http://localhost:50000/v1.0/invoke/catalogmanager/method/allquestions?offset=${offset}&limit=${limit}`;
    fetch(url2)
      .then((res) => res.json())
      .then((result) => {
        dispatch(loadQuestions(result));
      });
  }, [offset]);

  useEffect(() => {
    let url =
      "http://localhost:50000/v1.0/invoke/catalogmanager/method/questions/count";
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setPageCount(Math.ceil(result / limit));
      });
  }, []);

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
