import React from "react";
import {
  Group,
  Button,
  Text,
  Badge,
  Card,
  Space,
  useMantineTheme,
} from "@mantine/core";

type QuestionData = {
  Id: string;
  Title: string;
  Description: string;
  Author: any;
  Tags: any;
  Status: string;
  Version: string;
};

function QuestionCard({
  Id,
  Title,
  Description,
  Author,
  Tags,
  Status,
  Version,
}: QuestionData) {
  const theme = useMantineTheme();

  const secondaryColor =
    theme.colorScheme === "dark" ? theme.colors.dark[1] : theme.colors.gray[7];

  return (
    <div style={{ width: 340, margin: "auto" }}>
      <Card shadow="sm" p="lg">
        <Card.Section>
          <h3>{Title}</h3>
        </Card.Section>

        <Text weight={500}>{Description}</Text>

        <Group style={{ marginBottom: 5, marginTop: theme.spacing.sm }}>
          <Badge color="pink" variant="light">
            {`${Author.FirstName} ${Author.LastName}`}
          </Badge>
          <Badge color="yellow" variant="light">
            Date
          </Badge>
        </Group>

        <Text size="sm" style={{ color: secondaryColor, lineHeight: 1.5 }}>
          Here would go a preview of the test. <br />
          Here would go a preview of the test. <br />
          Here would go a preview of the test. <br />
          Here would go a preview of the test. <br />
        </Text>

        <Group style={{ marginBottom: 5, marginTop: theme.spacing.sm }}>
          {Tags.map((i: any) => {
            return (
              <Badge key={i} color="green" variant="light">
                {i}
              </Badge>
            );
          })}
        </Group>

        <Button
          variant="light"
          color="blue"
          fullWidth
          style={{ marginTop: 14 }}
        >
          Open
        </Button>
      </Card>
    </div>
  );
}

export default QuestionCard;
