import { Button, Group, Radio, Textarea, Card, Checkbox } from "@mantine/core";
import { useState } from "react";
import { X } from "tabler-icons-react";

function MultiChoice() {
  const [inputList, setInputList] = useState([
    { answerOption: "", correctAnswer: false },
  ]);
  const [chooseMode, setChooseMode] = useState(false);
  const [correctAnswer, setCorrectAnswer] = useState("");

  const handleInputChange = (e: any, index: number) => {
    const { name, value } = e.target;
    const list: any = [...inputList];
    list[index][name] = value;
    setInputList(list);
  };

  const handleAddClick = () => {
    setInputList([...inputList, { answerOption: "", correctAnswer: false }]);
  };

  const handleRemoveClick = (index: any) => {
    const list = [...inputList];
    list.splice(index, 1);
    setInputList(list);
  };

  return (
    <div>
      <Card withBorder shadow="xs" p="lg" radius="sm">
        {inputList.map((x, i) => {
          return (
            <div key={i}>
              <Group>
                {chooseMode ? (
                  <Radio
                    name="answerOption"
                    value={x.answerOption}
                    onChange={(e) => {
                      handleInputChange(e, i);
                      setCorrectAnswer(e.target.value);
                    }}
                  />
                ) : (
                  <Radio
                    disabled
                    name="answerOption"
                    value={x.answerOption}
                    onChange={(e) => handleInputChange(e, i)}
                  />
                )}
                <Textarea
                  name="answerOption"
                  value={x.answerOption}
                  onChange={(e) => handleInputChange(e, i)}
                  style={{ width: "80%" }}
                  autosize
                  variant="unstyled"
                  placeholder="Add option"
                />
                {inputList.length !== 1 && (
                  <Button
                    variant="subtle"
                    radius="xl"
                    color="gray"
                    style={{}}
                    onClick={() => handleRemoveClick(i)}
                  >
                    <X size={26} />
                  </Button>
                )}
              </Group>

              <div>
                {inputList.length - 1 === i && (
                  <Button
                    radius="xl"
                    variant="subtle"
                    compact
                    onClick={handleAddClick}
                  >
                    + Add option
                  </Button>
                )}
                {inputList.length - 1 === i && (
                  <Button
                    radius="xl"
                    variant="subtle"
                    compact
                    onClick={() => {
                      setChooseMode(!chooseMode);
                    }}
                  >
                    Correct answer
                  </Button>
                )}
              </div>
            </div>
          );
        })}
      </Card>
      {console.log(inputList)}
      {console.log(correctAnswer)}
    </div>
  );
}

export default MultiChoice;
