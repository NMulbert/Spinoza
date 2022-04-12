import React from 'react';
import { Group, Button, Text, Badge, Card, Space } from '@mantine/core';

type QuestionData = {
    Id: string;
    Title: string;
    Description: string;
    Author: any;
    Tags: any;
    Status: string;
    Version: string;
}

function ExistingQuestion({Id, Title, Description, Author, Tags, Status, Version}: QuestionData) {
    return (
        <div>
        <Card withBorder shadow="sm" p="lg">
            <Text weight="bold">{Title}</Text>
            <Text size="sm" weight="500">{`${Author.FirstName} ${Author.LastName}`}</Text>
            <Text size="sm" weight="500">Date</Text>
            <Space h="md"/>
            <Group position="center" spacing="xs">
            {Tags.map((i: any) => {
            return (
                <Badge key={i} color="pink" variant="light">
                {i}
              </Badge>
            );
          })}
            </Group>
            <Space h="xs"/>
            <Text size="sm" lineClamp={3}>
                {Description}
            </Text>
            <Space h="md"/>
            <Button variant="outline" radius="lg" size="xs">
            Select
            </Button>
        </Card>
        </div>
    );
}

export default ExistingQuestion;