import {
  Card,
  Text,
  Badge,
  Button,
  Group,
  useMantineTheme,
} from "@mantine/core";
import { Link } from "react-router-dom";

type TestCardProps = {
  id: any;
  title: string;
  description: string;
  author: any;
  tags: any;
  status: string;
  version: number;
};

const TestCard = ({ title, description, author, tags, id }: TestCardProps) => {
  const theme = useMantineTheme();
  const secondaryColor =
    theme.colorScheme === "dark" ? theme.colors.dark[1] : theme.colors.gray[7];

  return (
    <div style={{ width: 340, margin: "auto" }}>
      <Card shadow="sm" p="lg">
        <Card.Section>
          <h3>{title} </h3>
        </Card.Section>

        <Text weight={500}>{description}</Text>

        <Group style={{ marginBottom: 5, marginTop: theme.spacing.sm }}>
          <Badge color="pink" variant="light">
            {`${author.firstName} ${author.lastName}`}
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
          {tags.map((i: any) => {
            return (
              <Badge key={i} color="green" variant="light">
                {i}
              </Badge>
            );
          })}
        </Group>
        <Link to={`/tests/${id}`} style={{ textDecoration: "none" }}>
          <Button
            variant="light"
            color="blue"
            fullWidth
            style={{ marginTop: 14 }}
          >
            Open
          </Button>
        </Link>
      </Card>
    </div>
  );
};

export default TestCard;
