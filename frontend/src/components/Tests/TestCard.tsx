import { Card, Image, Text, Badge, Button, Group, useMantineTheme } from '@mantine/core';

export default function TestCard() {
  const theme = useMantineTheme();

const secondaryColor = theme.colorScheme === 'dark'
? theme.colors.dark[1]
: theme.colors.gray[7];

return (
<div style={{ width: 340, margin: 'auto' }}>
  <Card shadow="sm" p="lg">
    <Card.Section>
      <h3>Test </h3>
    </Card.Section>

     <Text weight={500}>this is test</Text>

    <Group  style={{ marginBottom: 5, marginTop: theme.spacing.sm }}>
      
      <Badge color="pink" variant="light">
      Author 
      </Badge>
      <Badge color="yellow" variant="light">
      Date
      </Badge>
      
    </Group>

    <Text size="sm" style={{ color: secondaryColor, lineHeight: 1.5 }}>
            Here would go a preview of the test. <br/>
            Here would go a preview of the test. <br/>
            Here would go a preview of the test. <br/>
            Here would go a preview of the test. <br/>
    </Text>

    <Group  style={{ marginBottom: 5, marginTop: theme.spacing.sm }}>
      
      <Badge color="green" variant="light">
      #React 
      </Badge>
      <Badge color="green" variant="light">
      #JAVASCRIPT
      </Badge>
      
    </Group>


    <Button variant="light" color="blue" fullWidth style={{ marginTop: 14 }}>
      open
    </Button>
  </Card>
</div>
);
}