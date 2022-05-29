import React, { useEffect, useState } from "react";
import {
  RadioGroup,
  Radio,
  MultiSelect,
  TextInput,
  SimpleGrid,
  Group,
  Button,
  Text,
  Space,
  Divider,
  SegmentedControl,
} from "@mantine/core";
import { Hash } from "tabler-icons-react";
import MDEditor from "@uiw/react-md-editor";
import MultiChoice from "./MultiChoice";
import { v4 as uuidv4 } from "uuid";
import axios from "axios";
import { useSelector } from "react-redux";
interface TagsState {
  tags: { tags: [] };
}

function NewQuestion({ UpdateQuestions, setOpenedNQ }: any) {
  let tags = useSelector((s: TagsState) => s.tags.tags);

  const [dataHash, setHashData]: any = useState([]);
  const [answerType, setAnswerType] = useState("OpenTextQuestion");
  const [multiArr, setMultiArr] = useState([]);
  const [questionTxt, setQuestionTxt] = useState("");
  const [validateName, setValidateName] = useState("");

  let protoType: any =
    answerType === "OpenTextQuestion"
      ? questionTxt
      : { questionText: questionTxt, answerOptions: multiArr };

  const [questionValues, setQuestionValues] = useState({
    MessageType: "AddQuestion",
    id: uuidv4().toUpperCase(),
    name: "",
    difficultyLevel: "1",
    type: "OpenTextQuestion",
    status: "Draft",
    authorId: "alonf@zion-net.co.il",
    tags: [],
    schemaVersion: "1.0",
    testVersion: "1.0",
    previousVersionId: "none",
    content: protoType,
  });

  useEffect(() => {
    answerType === "MultipleChoiceQuestion"
      ? setQuestionValues({
          ...questionValues,
          content: { questionText: questionTxt, answerOptions: multiArr },
        })
      : setQuestionValues({ ...questionValues, content: questionTxt });
  }, [questionTxt, multiArr]);

  return (
    <div>
      <SimpleGrid cols={1}>
        <div>
          <TextInput
            label="Question Name:"
            error={validateName}
            style={{ width: "50%", textAlign: "left" }}
            placeholder="Question Name"
            radius="xs"
            maxLength={40}
            onChange={(e: any) => {
              setQuestionValues({ ...questionValues, name: e.target.value });
            }}
          />
          <MultiSelect
            data={[...tags, ...dataHash]}
            label="Tags:"
            placeholder="Select tags"
            icon={<Hash />}
            radius="xs"
            style={{ width: "50%" }}
            searchable
            creatable
            getCreateLabel={(query) => `+ Create ${query}`}
            onCreate={(query) =>
              setHashData((current: any) => [...current, query])
            }
            onChange={(e: any) => {
              setQuestionValues({
                ...questionValues,
                tags: e,
              });
            }}
          />
        </div>
        <div className="container" data-color-mode="light">
          <MDEditor
            value={questionTxt}
            onChange={(e: any) => {
              setQuestionTxt(e);
            }}
          />
        </div>
        <div>
          <Text weight="500" size="sm">
            Answer Type:
          </Text>
          <Space h="xs" />
          <SegmentedControl
            size="xl"
            radius="sm"
            color="green"
            data={[
              { value: "OpenTextQuestion", label: "Text" },
              { value: "MultipleChoiceQuestion", label: "Multiple Choice" },
            ]}
            onChange={(value: any) => {
              setAnswerType(value);
              setQuestionValues({
                ...questionValues,
                type: value,
              });
            }}
          />
          <Space h="xs" />

          {answerType === "MultipleChoiceQuestion" ? (
            <MultiChoice setMultiArr={setMultiArr} answerOptions={multiArr} />
          ) : (
            <></>
          )}

          <Space h="xs" />
          <RadioGroup
            value={questionValues.difficultyLevel}
            onChange={(value: any) => {
              setQuestionValues({
                ...questionValues,
                difficultyLevel: value,
              });
            }}
            label="Select level:"
            required
          >
            <Radio value="1" label="1" />
            <Radio value="2" label="2" />
            <Radio value="3" label="3" />
            <Radio value="4" label="4" />
            <Radio value="5" label="5" />
          </RadioGroup>
          <Space h="xs" />
          <Divider size="xs" variant="dotted" />
        </div>
        <div>
          <Group spacing="sm">
            <Button
              radius="lg"
              variant="gradient"
              gradient={{ from: "#838685", to: "#cfd0d0" }}
              onClick={async () => {
                if (questionValues.name.trim().length !== 0) {
                  try {
                    let url = `${process.env.REACT_APP_BACKEND_URI}/question`;
                    const response = await axios.post(
                      url,
                      JSON.stringify({ ...questionValues })
                    );
                    setQuestionValues({
                      ...questionValues,
                      id: uuidv4().toUpperCase(),
                    });
                    UpdateQuestions([questionValues]);
                    setOpenedNQ(false);
                  } catch (err) {
                    console.log(err);
                  }
                } else {
                  setValidateName("Question name is a required field");
                }
              }}
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
        </div>
      </SimpleGrid>
    </div>
  );
}

export default NewQuestion;
