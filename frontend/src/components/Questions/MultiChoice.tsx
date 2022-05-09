import {
  Group,
  Text,
  Code,
  Button,
  Radio,
  ActionIcon,
  Textarea,
  Card,
} from "@mantine/core";
import { useForm, formList } from "@mantine/form";
import { useState } from "react";
import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";
import { GripVertical, X } from "tabler-icons-react";

function MultiChoice() {
  const [chooseMode, setChooseMode] = useState(false);

  const form = useForm({
    initialValues: {
      answers: formList([{ description: "", isCorrect: false }]),
    },
  });

  const fields = form.values.answers.map((_, index) => (
    <Draggable key={index} index={index} draggableId={index.toString()}>
      {(provided) => (
        <Group ref={provided.innerRef} mt="xs" {...provided.draggableProps}>
          <div {...provided.dragHandleProps}>
            <GripVertical size={18} />{" "}
          </div>

          {chooseMode ? (
            <Radio
              name="Answer"
              checked={_.isCorrect}
              value={_.description}
              onChange={() => {
                {
                  form.setListItem("answers", index, {
                    description: _.description,
                    isCorrect: true,
                  });
                }
                {
                  form.values.answers.map((e: any) => (e.isCorrect = false));
                }
              }}
            />
          ) : (
            <Radio
              disabled
              checked={_.isCorrect}
              name="Answer"
              value={_.description}
            />
          )}
          <Textarea
            placeholder="Add option"
            autosize
            minRows={1}
            style={{ width: "80%" }}
            autoFocus={true}
            {...form.getListInputProps("answers", index, "description")}
          />
          <ActionIcon
            color="red"
            variant="hover"
            onClick={() => form.removeListItem("answers", index)}
          >
            <X size={18} />
          </ActionIcon>
        </Group>
      )}
    </Draggable>
  ));

  return (
    <Card withBorder sx={{ maxWidth: "auto" }} mx="auto">
      <DragDropContext
        onDragEnd={({ destination, source }) =>
          form.reorderListItem("answers", {
            from: source.index,
            to: destination!.index,
          })
        }
      >
        <Droppable droppableId="dnd-list" direction="vertical">
          {(provided) => (
            <div {...provided.droppableProps} ref={provided.innerRef}>
              {fields}
              {provided.placeholder}
            </div>
          )}
        </Droppable>
      </DragDropContext>

      <Group mt="md">
        <Button
          radius="xl"
          variant="subtle"
          compact
          onClick={() =>
            form.addListItem("answers", {
              description: "",
              isCorrect: false,
            })
          }
        >
          + Add option
        </Button>
        <Button
          radius="xl"
          variant="subtle"
          compact
          onClick={() => {
            setChooseMode(!chooseMode);
          }}
        >
          {chooseMode ? "Done" : "Correct answer"}
        </Button>
      </Group>
    </Card>
  );
}

export default MultiChoice;
