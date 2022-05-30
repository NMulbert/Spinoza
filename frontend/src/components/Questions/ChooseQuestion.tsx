import { useState, useEffect } from "react";
import { Search, Hash, ArrowRight } from "tabler-icons-react";
import {
  MultiSelect,
  TextInput,
  SimpleGrid,
  Group,
  Button,
  Text,
  ScrollArea,
  ActionIcon,
  Divider,
  Loader,
} from "@mantine/core";
import { useDispatch, useSelector } from "react-redux";
import ExistingQuestion from "./ExistingQuestion";
interface QuestionsState {
  UpdateQuestions: (questions: any) => any;
  setOpenedEQ: (boolean: boolean) => void;
}

interface TagsState {
  tags: { tags: [] };
}

function ChooseQuestion({ UpdateQuestions, setOpenedEQ }: QuestionsState) {
  let tags = useSelector((s: TagsState) => s.tags.tags);
  let tagsData = tags || [];
  const [questionsTags, setQuestionsTags] = useState<string[]>([]);

  const [selectedQuestions, setSelectedQuestions] = useState([]);
  const [questions, setQuestions] = useState([]);
  useEffect(() => {
    let url = `${process.env.REACT_APP_BACKEND_URI}/allquestions`;
    if (questionsTags.length > 0) {
      for (let tag of questionsTags) {
        url = url + `?tag=${tag}`;
      }
    }
    fetch(url)
      .then((res) => res.json())
      .then((result) => {
        setQuestions(result);
      });
  }, [questionsTags]);

  return (
    <div>
      <SimpleGrid cols={1}>
        <div>
          <Group position="center">
            <TextInput
              icon={<Search />}
              style={{ width: "90%" }}
              placeholder="Search"
              radius="lg"
              rightSection={
                <ActionIcon size={32} radius="xl" variant="filled" color="blue">
                  <ArrowRight />
                </ActionIcon>
              }
            />
          </Group>
        </div>

        <div>
          <Group position="center" spacing="xs">
            <MultiSelect
              data={tagsData}
              placeholder="Select tags"
              icon={<Hash />}
              radius="lg"
              style={{ width: "70%" }}
              searchable
              limit={20}
              nothingFound="Nothing found"
              clearButtonLabel="Clear selection"
              clearable
              value={questionsTags}
              onChange={setQuestionsTags}
              maxDropdownHeight={100}
            />
          </Group>
        </div>
        <div>
          <ScrollArea
            style={{ height: 450, width: "100%", backgroundColor: "#f0f0f0" }}
          >
            {
              <SimpleGrid style={{ textAlign: "center" }} cols={3}>
                {questions.length !== 0 ? (
                  questions.map((i: any) => {
                    return (
                      <ExistingQuestion
                        setSelectedQuestions={setSelectedQuestions}
                        selectedQuestions={selectedQuestions}
                        key={i.id}
                        question={i}
                      />
                    );
                  })
                ) : (
                  <Loader />
                )}
              </SimpleGrid>
            }
          </ScrollArea>
          <Divider size="xs" variant="dotted" />
        </div>
        <div>
          <Group position="center" spacing="xs">
            <Text size="md">Selected:</Text>
            <Text weight="bold" size="md">
              {selectedQuestions.length}
            </Text>
            <Button
              onClick={() => {
                UpdateQuestions([...selectedQuestions]);
                setOpenedEQ(false);
              }}
              radius="lg"
            >
              Done
            </Button>
          </Group>
        </div>
      </SimpleGrid>
    </div>
  );
}

export default ChooseQuestion;
