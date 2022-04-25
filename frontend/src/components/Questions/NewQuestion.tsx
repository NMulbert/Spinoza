import React, { useState } from "react";
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
  CheckboxGroup,
  Checkbox,
} from "@mantine/core";
import { Hash } from "tabler-icons-react";
import MDEditor from "@uiw/react-md-editor";
import MultiChoice from "./MultiChoice";

function NewQuestion() {
  const [dataHash, setHashData] = useState([
    "React",
    "C#",
    "JavaScript",
    "Python",
  ]);
  const [questionValue, setQuestionValue] = useState(
    "**Write question here...**"
  );

    const [answerType, setAnswerType] = useState("Text");

  return (
    <div>
      <SimpleGrid cols={1}>
        <div>
          <TextInput
            label="Question Name:"
            style={{ width: "50%", textAlign: "left" }}
            placeholder="Question Name"
            radius="xs"
          />
          <MultiSelect
            data={dataHash}
            label="Tags:"
            placeholder="Select tags"
            icon={<Hash />}
            radius="xs"
            style={{ width: "50%" }}
            searchable
            creatable
            getCreateLabel={(query) => `+ Create ${query}`}
            onCreate={(query) => setHashData((current) => [...current, query])}
          />
        </div>
        <div className="container">
          <MDEditor
            value={questionValue}
            onChange={(val) => {
              setQuestionValue(val!);
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
              { value: "Text", label: "Text" },
              { value: "MultipleChoice", label: "Multiple Choice" },
              { value: "Checkboxes", label: "Checkboxes" },
            ]}
            onChange={(value) => {
              setAnswerType(value);
            }}
          />
          <Space h="xs" />
          {answerType === "MultipleChoice" ? (
            <MultiChoice />
          ) : answerType === "Checkboxes" ? (
            <></>
          ) : (
            <></>
          )}
          <Space h="xs" />
          <RadioGroup label="Select level:" required>
            <Radio value="level1" label="1" />
            <Radio value="level2" label="2" />
            <Radio value="level3" label="3" />
            <Radio value="level4" label="4" />
            <Radio value="level5" label="5" />
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
